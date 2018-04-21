using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFIDApplication.DAL.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

namespace RFIDApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        protected readonly IConfigurationRoot _config;

        public AccountController(IConfigurationRoot config)
        {
            _config = config;
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                string str_Username = _config.GetValue<string>("SiteLogin:Username");
                string str_P = _config.GetValue<string>("SiteLogin:Password");

                if (model.Username.ToLower().Equals(str_Username.ToLower()) && model.Password.Equals(str_P))
                {

                    //FormsAuthentication.SetAuthCookie(str_Username, model.RememberMe);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, str_Username)
                    };
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

                    await HttpContext.Authentication.SignInAsync("Cookie", principal,
                            new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                                IsPersistent = false,
                                AllowRefresh = false
                            });
                    return RedirectToLocal(returnUrl);
                }
            }
            return View(model);
        }

        //LogOff
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.Authentication.SignOutAsync("Cookie");
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


    }
}