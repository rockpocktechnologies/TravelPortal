using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace TravelPortal.Controllers
{
    public class SignOutController : Controller
    {
        public IActionResult Index()
        {
            HttpContext.SignOutAsync();

            // Clear session data if needed
            HttpContext.Session.Clear();

            // Redirect to a page after sign-out (e.g., home page)
            return RedirectToAction("Index", "NewRequest");
          //  return View();
        }
    }
}
