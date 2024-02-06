using Microsoft.AspNetCore.Mvc;

namespace TravelPortal.Controllers
{
    public class signout_oidc : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
