using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using First.Data;
using First.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace First.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<PlantsistEmployee> _userManager;
        private IUserClaimsPrincipalFactory<PlantsistEmployee> _principalFactory;
        private ILogger<HomeController> _logger;

        public HomeController(
            UserManager<PlantsistEmployee> userManager,
            IUserClaimsPrincipalFactory<PlantsistEmployee> principalFactory,
            ILogger<HomeController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _principalFactory = principalFactory ?? throw new ArgumentNullException(nameof(principalFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "Claim.Level")]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if(user != null)
            {
                if(await _userManager.CheckPasswordAsync(user,password))
                {
                    var principal = await _principalFactory.CreateAsync(user);
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                    return RedirectToAction("Secret");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if(user == null)
            {
                user = new PlantsistEmployee
                { 
                    UserName = username,
                    Department = "Technology",
                    Level = 1
                };
                if((await _userManager.CreateAsync(user, password)).Succeeded)
                {
                    var principal = await _principalFactory.CreateAsync(user);
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                    return RedirectToAction("Secret");
                }
            }

            return RedirectToAction("Index");
        }
    }
}
