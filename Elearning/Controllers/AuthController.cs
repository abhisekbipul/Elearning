using Elearning.Data;
using Elearning.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elearning.Controllers
{
    
    public class AuthController : Controller
    {
        private readonly ElearningContext db;
        public AuthController(ElearningContext db)
        {
            this.db = db;
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [AcceptVerbs("Post", "Get")]
        public IActionResult CheckExistingEmailId(string email)
        {
            var data = db.UserAccounts.Where(x => x.UserEmail == email).SingleOrDefault();
            if (data != null)
            {
                return Json($"Email {email} already in used");
            }
            else
            {
                return Json(true);
            }
        }

        [HttpPost]
        public IActionResult SignUp(UserAccount user)
        {
            user.UserRole = "User";
            db.UserAccounts.Add(user);
            db.SaveChanges();
            return RedirectToAction("SignIn");
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(UserAccount user)
        {
            var data = db.UserAccounts.Where(x => x.UserEmail.Equals(user.UserEmail)).SingleOrDefault();
            if(data!=null)
            {
                bool v = data.UserEmail.Equals(user.UserEmail) && data.UserPass.Equals(user.UserPass);
                if (v)
                {
                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserEmail) },
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Session.SetString("uemail", user.UserEmail);
                    return RedirectToAction("Home", "Dashboard");
                }
                else
                {
                    TempData["errorpasword"] = "Invalid Password";
                }
            }
            else
            {
                TempData["erroremail"] = "Invalid Email";
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var storedcookies = Request.Cookies.Keys;
            foreach (var cookie in storedcookies)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("SignIn");
        }


    }
}
