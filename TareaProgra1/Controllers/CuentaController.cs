using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;
using TareaProgra1.Data;
using TareaProgra1.Models;

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

        public async Task<IActionResult> Login(Usuario usuario)
        {
            try
            {
                using (SqlConnection con = new(_bdcontext.DefaultConnection))
                {
                    using (SqlCommand cmd = new("sp_ValidarUsuario", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = usuario.UserName;
                        cmd.Parameters.Add("@Password", System.Data.SqlDbType.VarChar).Value = usuario.Password;
                        con.Open();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            if (dr["UserName"] != null && usuario.UserName != null)
                            {
                                List<Claim> c = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, usuario.UserName)
                                };
                                ClaimsIdentity ci = new(c, CookieAuthenticationDefaults.AuthenticationScheme);
                                AuthenticationProperties p = new();

                                p.AllowRefresh = true;
                                p.IsPersistent = usuario.MantenerActivo;

                                if (!usuario.MantenerActivo)
                                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1);
                                else
                                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci), p);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.Error = "Credenciales incorrectas o cuenta no registrada.";
                            }
                        }
                        con.Close();
                    }
                    return View();
                }
            }
            catch(System.Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }

        }

    }
}
