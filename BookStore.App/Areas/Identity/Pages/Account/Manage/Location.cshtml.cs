using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BookStore.App.Areas.Identity.Pages.Account.Manage
{
    public class LocationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

#pragma warning disable CS8618 // Non-nullable property 'Input' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'StatusMessage' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public LocationModel(
#pragma warning restore CS8618 // Non-nullable property 'StatusMessage' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'Input' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [MaxLength(100)]
            public string? Address { get; set; }

            [MaxLength(20)]
            public string? City { get; set; }

            [MaxLength(20)]
            public string? State { get; set; }

            [MaxLength(20)]
            public string? PostalCode { get; set; }
        }

        private void Load(AppUser user)
        {

            Input = new InputModel
            {
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Load(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }

            user.Address = Input.Address;
            user.City = Input.City;
            user.State = Input.State;
            user.PostalCode = Input.PostalCode;

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
