using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return StatusCode(200);
        }
    }
    
}