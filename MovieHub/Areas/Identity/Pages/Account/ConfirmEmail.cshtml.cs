using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MovieHub.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<MHUser> _userManager;

        public ConfirmEmailModel(UserManager<MHUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            Console.WriteLine("Enter verify page");
            Console.WriteLine(code);
            Console.WriteLine(userId);
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            https://localhost:5001/Identity/Account/ConfirmEmail?
                  //userId=47476852-1348-4ce7-837d-441169f9d172&amp;
                  //code=CfDJ8DNHbGhnpHlDtW8uv7EAzFdEFxPCSoEr36V3ngrheImfupQXbSSbSXihx097vv3h086mpThw%2BQpK%2FUtFKJV6mLa1WMGJ8FDrrpLDDaPwReQVosUFXWmoUmnq4rQrw0%2FIbcCsU9h26e7U87nfzVy00MjWrdyrhMhJSMyKmK8ScU98NAGIH%2Bu9eVavs22FogDb4EXHlzcXidd88EynV8QlKTDeyin5SuhdAlcsiLmPyu7WGujuAnH75w5Azm%2BNuEP6bQ%3D%3D
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            return Page();
        }
    }
}
