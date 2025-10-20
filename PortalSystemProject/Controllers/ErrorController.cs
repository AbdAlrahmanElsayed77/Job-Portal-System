using Microsoft.AspNetCore.Mvc;

namespace PortalSystemProject.Controllers
{
    public class ErrorController : Controller
    {
            [Route("Error/{statusCode}")]
            public IActionResult HttpStatusCodeHandler(int statusCode)
            {
                switch (statusCode)
                {
                    case 404:
                        return View("NotFound");
                    case 403:
                        return View("AccessDenied");
                    case 500:
                        return View("ServerError");
                    default:
                        return View("Error");
                }
            }

            [Route("Error/AccessDenied")]
            public IActionResult AccessDenied()
            {
                return View();
            }

            [Route("Error/NotFound")]
            public new IActionResult NotFound()
            {
                return View();
            }

            [Route("Error")]
            public IActionResult Error()
            {
                return View();
            }
        }
    }

