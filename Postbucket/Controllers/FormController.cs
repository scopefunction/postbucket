using System;
using System.ComponentModel.Design;
using System.Data.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Postbucket.BLL;
using Postbucket.BLL.Extensions;
using Postbucket.Models;

namespace Postbucket.Controllers
{
    [Controller]
    [Route("form")]
    public class FormController : Controller
    {
        private readonly Context _context;

        public FormController(Context context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("")]
        public IActionResult PostForm(IFormCollection collection)
        {
            var path = HttpContext.Request.Path;
            
            var form = new FormSubmission();
            
            var recipient = collection
                .TryGetValue("recipient", out var recipientValue);

            var redirect = collection.TryGetValue("redirect", out var redirectValue);

            if (!recipient||!redirect)
            {
                return new BadRequestResult();
            }
            
            foreach (var keyValuePair in collection)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                form.AddToSubmissions(key, value);
            }
            
            form.Remove("recipient");
            form.Remove("redirect");
            
            var json = form.Return()
                .Serialize();
            
            EmailService.Send(json, recipientValue);

            var data = new FormData()
            {
                SerializedData = json,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return Redirect(redirectValue);
        }

        [HttpGet]
        [Route("")]
        public void HttpGet()
        {
            HttpContext.Response.ContentType = "text/html";
            HttpContext.Response.WriteAsync("<html><h1>Postbucket</h1></html>");
        }
    }
}
