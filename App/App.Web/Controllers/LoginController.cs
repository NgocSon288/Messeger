using App.Integration.Interfaces;
using App.Utilities.Common;
using App.Utilities.Helpers;
using App.ViewModel.AppUsers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAppUserApiClient _appUserApiClient;
        private readonly IJwtHelper _jwtHelper; 

        public LoginController(IAppUserApiClient appUserApiClient,
            IJwtHelper jwtHelper)
        {
            _appUserApiClient = appUserApiClient;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("/dang-nhap")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost("/dang-nhap")]
        public async Task<IActionResult> Login(UserLoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _appUserApiClient.Authenticate(model);
            if (!result.IsSuccessfully)
            {
                return View(model);
            }

            await AuthenticateClient(result.Data);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Login");
        }

        [HttpGet("/dang-ky")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/dang-ky")]
        public async Task<IActionResult> Register(UserRegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _appUserApiClient.Register(model);

            if (!result.IsSuccessfully)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(result.Data))
            {
                return RedirectToAction("Login", "Login");
            }

            await AuthenticateClient(result.Data);
            return RedirectToAction("Index", "Home");
        }

        #region Methods

        private async Task AuthenticateClient(string token)
        {
            var userPrincipal = await _jwtHelper.TokenToClaimsPrincipal(token);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false
            };

            HttpContext.Session.SetString(SystemConstants.AppSettings.Token, token);
            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);
        }

        #endregion
    }
}
