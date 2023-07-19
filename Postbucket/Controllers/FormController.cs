using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Postbucket.BLL;
using Postbucket.DTO;
using Postbucket.Models;
using Postbucket.Services;

namespace Postbucket.Controllers;

[Controller]
[Route("/submit")]
public class FormController : Controller
{
    private readonly IDocumentService _documentService;
    private readonly IEmailService _emailService;

    public FormController(IDocumentService documentService, IEmailService emailService)
    {
        _documentService = documentService;
        _emailService = emailService;
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> PostForm(
        [FromForm] IFormCollection collection, 
        [FromBody] Dictionary<string, string> fields,
        [FromQuery] string? formId, 
        [FromQuery] string? redirect)
    {
        var validatedForm = await _documentService.ValidateFormId(formId);

        if (formId is null)
        {
            return BadRequest(
                "The formId needs to be provided in the URL as a query parameter following the structure 'https://postbucket-we.azurewebsites.net/submit?formId=63a3c2c2-dd65-49d3-b08b-3a4fd4616ce0'");
        }
        
        if (!validatedForm.IsValid || validatedForm.Form is null)
        {
            return Unauthorized("Your form Id is not recognized by PostBucket. Request a form Id by emailing info@mergedigital.io");
        }

        switch (collection.Count)
        {
            case 0 when fields.Count == 0:
                return BadRequest("No form collection data or json object data was received");
            case > 0:
                foreach (var key in collection.Keys)
                {
                    collection.TryGetValue(key, out var value);
                    fields.Add(key, value);
                }
                
                if (!fields.TryGetValue("email", out _))
                {
                    return BadRequest("There was no required `email` provided in the request and therefore the submission can not be processed");
                }

                await HandleSubmission(fields, validatedForm.Form);
                break;
        }

        if (fields.Count > 0)
        {
            if (!fields.TryGetValue("email", out _))
            {
                return BadRequest("There was no required `email` provided in the request and therefore the submission can not be processed");
            }
            
            await HandleSubmission(fields, validatedForm.Form);
        }

        if (redirect is not null)
        {
            return Redirect(redirect);
        }

        return Ok();
    }

    private async Task HandleSubmission(Dictionary<string, string> fields, FormContext formContext)
    {
        await _emailService.Send(new EmailPayload
        {
            Subject = $"A new submission from {formContext.Title}",
            Recipient = formContext.Email,
            Fields = fields,
            Website = formContext.Website,
            Title = formContext.Title
        });
        
        await _documentService.SaveSubmission(fields);
    }

    [HttpGet]
    [Route("")]
    public void HttpGet()
    {
        HttpContext.Response.ContentType = "text/html";
        HttpContext.Response.WriteAsync("<html><h1>Postbucket</h1></html>");
    }
}