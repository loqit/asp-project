using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MovieHub.Areas.Identity.Pages.Account.Manage
{
    public class AddModerator : PageModel
    {
        private readonly UserManager<MHUser> _userManager;
        private readonly SignInManager<MHUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddModerator(UserManager<MHUser> manager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = manager;
            _roleManager = roleManager;
        }
        [TempData]
        public string StatusMessage { get; set; }
        
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            


            var roleResult = _roleManager.CreateAsync(new IdentityRole("Moderator")).Result;
            if (roleResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            _logger.LogInformation("Moderator added successfully.");
            StatusMessage = "Moderator added successfully.";

            return RedirectToPage();
        }
    }
}