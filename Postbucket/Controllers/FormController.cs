using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Postbucket.BLL;
using Postbucket.DTO;
using Postbucket.Services;

namespace Postbucket.Controllers;

[Controller]
[Route("/submit")]
public class FormController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IDocumentService _documentService;
    private readonly IEmailService _emailService;

    public FormController(IConfiguration configuration, IDocumentService documentService, IEmailService emailService)
    {
        _configuration = configuration;
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
        
        if (!validatedForm.IsValid || validatedForm.Form is null)
        {
            return Unauthorized("Invalid form Id provided");
        }

        switch (collection.Count)
        {
            case 0 when fields.Count == 0:
                return BadRequest("No form collection data or json object data was received");
            case > 0:
                HandleFormRequest(collection, validatedForm.Form);
                break;
        }

        if (fields.Count > 0)
        {
            HandleBodyRequest(fields, validatedForm.Form);
        }

        return Ok();
    }

    private void HandleFormRequest(IFormCollection formCollection, FormEntity formEntity)
    {
        var fields = new Dictionary<string, string>();
        
        foreach (var key in formCollection.Keys)
        {
            formCollection.TryGetValue(key, out var value);
            fields.Add(key, value);
        }

        _emailService.Send(new EmailPayload
        {
            Subject = $"A new submission from {formEntity.Title}",
            Recipient = formEntity.Email,
            Fields = fields
        });
    }

    private void HandleBodyRequest(Dictionary<string, string> fields, FormEntity formEntity)
    {
        _emailService.Send(new EmailPayload
        {
            Subject = $"A new submission from {formEntity.Title}",
            Recipient = formEntity.Email,
            Fields = fields
        });
    }
    
    [HttpGet]
    [Route("")]
    public void HttpGet()
    {
        HttpContext.Response.ContentType = "text/html";
        HttpContext.Response.WriteAsync("<html><h1>Postbucket</h1></html>");
    }
}