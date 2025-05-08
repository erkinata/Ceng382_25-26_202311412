//Ai prompt 4:  Write code for the Login.cshtml.cs and file that I gave you from my project, and the login process will be performed. The information from the login form will be compared with the users
//    in the users.json file. If there is a user who matches the information entered and is active, a session and cookie will be created. This information will include the username,
//    session ID and a simple security token. This information will be written to both the session and the cookies. Cookies will be valid for 30 minutes and will be kept in secure settings.
//    If the login is successful, the user will be directed to the data table page. If it is unsuccessful, a warning like "Username or password is incorrect" will be displayed.


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projectw9.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Projectw9.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is already logged in
            if (IsUserLoggedIn())
            {
                return RedirectToPage("/Table");
            }

            return Page();
        }

        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verify user credentials
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => 
                u.Username == Username && 
                u.Password == Password && 
                u.IsActive);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username or password is incorrect.");
                return Page();
            }

            // Generate a simple token
            var token = GenerateToken();
            var sessionId = HttpContext.Session.Id;

            // Store user info in session
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", sessionId);
            HttpContext.Session.SetString("role", user.Role);

            // Set cookies
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", sessionId, cookieOptions);

            // Redirect to the Index page on successful login
            return RedirectToPage("/Table");
        }

        private List<User> LoadUsers()
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
                if (!System.IO.File.Exists(filePath))
                {
                    // If file doesn't exist, create a sample user file
                    var users = new List<User>
                    {
                        new User
                        {
                            Username = "admin",
                            Password = "admin123",
                            Role = "Admin",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new User
                        {
                            Username = "user",
                            Password = "user123",
                            Role = "User",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    // Ensure directory exists
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data"));
                    
                    // Write the sample users to file
                    System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(users, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    }));
                    
                    return users;
                }

                // Read users from file
                string jsonContent = System.IO.File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<User>>(jsonContent) ?? new List<User>();
            }
            catch (Exception)
            {
                // Return empty list if file can't be read
                return new List<User>();
            }
        }

        private string GenerateToken()
        {
            // Generate a simple token using GUID
            return Guid.NewGuid().ToString();
        }

        private bool IsUserLoggedIn()
        {
            // Check if user is logged in by verifying session and cookie
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            if (string.IsNullOrEmpty(sessionUsername) || 
                string.IsNullOrEmpty(sessionToken) || 
                string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            // Check if cookies match session values
            if (Request.Cookies.TryGetValue("username", out string cookieUsername) &&
                Request.Cookies.TryGetValue("token", out string cookieToken) &&
                Request.Cookies.TryGetValue("session_id", out string cookieSessionId))
            {
                return sessionUsername == cookieUsername &&
                       sessionToken == cookieToken &&
                       sessionId == cookieSessionId;
            }

            return false;
        }
    }
}