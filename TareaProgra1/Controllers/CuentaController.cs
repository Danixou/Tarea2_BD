using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TareaProgra1.Data;

namespace TareaProgra1.Controllers
{
    public class CuentaController : Controller
    {
        private readonly BDContext _bdcontext;

        public CuentaController(BDContext bdcontext)
        {
            _bdcontext = bdcontext;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal c = HttpContext.User;
            if(c.Identity != null)
            {
                if(c.Identity.IsAuthenticated) 
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Logi

    }
}
