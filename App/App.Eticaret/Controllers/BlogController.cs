using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class BlogController : BaseController
    {
        [HttpGet("blog")]
        public IActionResult Index()
        {
            // The blog is rebuilt with real content in stage 4; until then the page explains that.
            return View();
        }

        [HttpGet("blog/{id}")]
        public IActionResult Detail(int id)
        {
            return View();
        }
    }
}
