using Microsoft.AspNetCore.Mvc;

namespace PortalSystemProject.Areas.admin.Controllers
{
    public class DashboardController : Controller
    {
        [Area("admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
