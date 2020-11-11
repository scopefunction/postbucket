using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Postbucket.BLL.Extensions;
using Postbucket.Models;

namespace Postbucket.Controllers
{
    [Controller]
    [Route("form")]
    public class FormController : Controller
    {
        [HttpPost]
        [Route("")]
        public IActionResult PostForm(IFormCollection collection)
        {
            var form = new FormSubmission();

            foreach (var keyValuePair in collection)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                form.AddToSubmissions(key, value);
            }

            var json = form.Return().Serialize();

            var data = new FormData()
            {
                SerializedData = json,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
            var context = new Context();
            context.FormData.Add(data);
            context.SaveChanges();

            return StatusCode(200);
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Content("This app works.");
        }
    }

}