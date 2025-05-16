//Ai prompt 4:  Write code for the Login.cshtml.cs and file that I gave you from my project, and the login process will be performed. The information from the login form will be compared with the users
//    in the users.json file. If there is a user who matches the information entered and is active, a session and cookie will be created. This information will include the username,
//    session ID and a simple security token. This information will be written to both the session and the cookies. Cookies will be valid for 30 minutes and will be kept in secure settings.
//    If the login is successful, the user will be directed to the data table page. If it is unsuccessful, a warning like "Username or password is incorrect" will be displayed.


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Projectw9.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        public bool RememberMe { get; set; }

        public void OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Table");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(Username, Password, RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Table");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
