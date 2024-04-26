using Capstone.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Helpers;

namespace Capstone.Controllers
{
    public class AuthController : Controller
    {
        AniDbContext db = new AniDbContext();

        [HttpPost]
        public IActionResult Register([Bind(include: "Email, Username, Password")] User user)
        {
            user.HashedPassword = Crypto.HashPassword(user.Password);
            ModelState.Remove("HashedPassword");
            if (ModelState.IsValid)
            {
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch
                {
                    TempData["error"] = "Username or Email not available.";
                }

            }

            return Redirect(Request.Headers.Referer.ToString()); ;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, bool Remember)
        {
            try
            {

                var user = db.Users.Where(o => o.Username == Username).FirstOrDefault();
                if (user == null || !Crypto.VerifyHashedPassword(user.HashedPassword, Password))
                {
                    return Redirect(Request.Headers.Referer.ToString());
                }


                // COOKIE
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString() ?? ""));
                claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);                      // delete old cookie if exist
                await HttpContext.SignInAsync(                                                                          // add new cookie
                                              CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity),
                                              new AuthenticationProperties { IsPersistent = Remember }                         // remember me
                );

                return Redirect(Request.Headers.Referer.ToString());
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                Console.WriteLine(e.Message);

                return Redirect(Request.Headers.Referer.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect(Request.Headers.Referer.ToString());
        }
    }
}
