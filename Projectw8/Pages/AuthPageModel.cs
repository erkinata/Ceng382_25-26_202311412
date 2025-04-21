// Ai Promt 4 : Create a C# file named AuthPageModel.cs with the help of the code I gave. Let this class be derived from the PageModel class and used as the base class for protected pages. 
// Before opening the page, check if the information in the session and cookie match. If the information does not match, direct the user to the Login page.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Projectw8.Pages
{
    public class AuthPageModel : PageModel
    {
        protected bool IsAuthenticated { get; private set; }
        protected string Username { get; private set; }
        protected string Role { get; private set; }

        public virtual void OnPageHandlerExecuting()
        {
            // Check if user is authenticated
            VerifyAuthentication();

            // If not authenticated, redirect to login page
            if (!IsAuthenticated)
            {
                HttpContext.Response.Redirect("/Index");
                return;
            }
        }

        private void VerifyAuthentication()
        {
            // Get values from session
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");
            var sessionRole = HttpContext.Session.GetString("role");

            // Check if session values exist
            if (string.IsNullOrEmpty(sessionUsername) ||
                string.IsNullOrEmpty(sessionToken) ||
                string.IsNullOrEmpty(sessionId))
            {
                IsAuthenticated = false;
                return;
            }

            // Check if cookies match session values
            if (Request.Cookies.TryGetValue("username", out string cookieUsername) &&
                Request.Cookies.TryGetValue("token", out string cookieToken) &&
                Request.Cookies.TryGetValue("session_id", out string cookieSessionId))
            {
                IsAuthenticated = sessionUsername == cookieUsername &&
                                 sessionToken == cookieToken &&
                                 sessionId == cookieSessionId;

                if (IsAuthenticated)
                {
                    Username = sessionUsername;
                    Role = sessionRole ?? "User";
                }
            }
            else
            {
                IsAuthenticated = false;
            }
        }

        public IActionResult OnGetLogout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Remove cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            // Redirect to login page
            return RedirectToPage("/Index");
        }
    }
}