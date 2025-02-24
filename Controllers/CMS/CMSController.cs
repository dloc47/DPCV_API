using Microsoft.AspNetCore.Mvc;

namespace DPCV_API.Controllers.CMS
{
    public class CMSController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
