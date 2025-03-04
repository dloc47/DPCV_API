using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.CMS.Images
{
    public class ImageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
