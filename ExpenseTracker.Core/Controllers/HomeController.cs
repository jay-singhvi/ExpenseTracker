using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult<string> Get() => Ok("Hello World!");
    }
}
