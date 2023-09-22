using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TareaProgra1.Data;
using TareaProgra1.Models;

namespace TareaProgra1.Controllers;

[Authorize]


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BDContext _db;


        public HomeController(ILogger<HomeController> logger, BDContext db)
        {
            _logger = logger;
            _db = db;
        }
        
        public async Task<IActionResult> salir() 
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Cuenta");  

        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
