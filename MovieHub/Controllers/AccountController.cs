using System.Net.WebSockets;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieHub.Areas.Identity;
using MovieHub.ViewModels;

namespace MovieHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<MHUser> _userManager;
        private readonly SignInManager<MHUser> _signInManager;
        private readonly ILogger<RegisterViewModel> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<MHUser> userManager, 
            SignInManager<MHUser> signInManager,
            ILogger<RegisterViewModel> logger,
            IEmailSender emailSender
            )
        {
            userManager = _userManager;
            signInManager = _signInManager;
            logger = _logger;
            emailSender = _emailSender;
        }

        public IActionResult NotFound()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new MHUser
                {
                    UserName = model.Email, 
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var token= await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var conformationLink = Url.Action("VerifyEmail", "Account",
                        new {userId = user.Id, token = token}, Request.Scheme);
                    
                    var link = HtmlEncoder.Default.Encode(conformationLink);
                    
                    await _emailSender.SendEmailAsync(model.Email, "Verify your email",
                        $"Please confirm your account by <a href='{link}'>clicking here</a>.");
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} not valid";
                return View("NotFound");
            }

            return View();
        }
        
        
    }
}