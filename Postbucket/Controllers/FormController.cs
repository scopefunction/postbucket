using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                form.SubmissionValues.Add(key, value);
            }

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