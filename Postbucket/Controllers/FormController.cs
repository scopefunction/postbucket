﻿using System;
using System.ComponentModel.Design;
using System.Data.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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

        public void ManualInjection()
        {
            var context = Resolver.Get<DbContext>();
        }
        
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
            
            _context.FormData.Add(data);
            _context.SaveChanges();

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