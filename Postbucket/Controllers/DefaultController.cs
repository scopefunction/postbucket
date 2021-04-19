using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Postbucket.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        [Route("")]
        public void Default()
        {
            HttpContext.Response.ContentType = "text/html";
            HttpContext.Response.WriteAsync("<html><h1>Postbucket</h1></html>");
        }
    }
}