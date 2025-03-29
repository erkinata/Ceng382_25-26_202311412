//Ai prompt: Generate Razor Page button examples that use asp-page-handler attributes for actions like Add, Edit, and Delete. Each button should be correctly 
// linked to C# handler methods in the PageModel
//  (e.g., OnPostAdd(), OnPostDelete(int id)). Additionally, provide C# implementations of these methods inside the .cshtml.cs file, 
// ensuring they return IActionResult. The output should be structured and ready to use in an ASP.NET Core Razor Pages project."
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projectw5.Models;
using System.ComponentModel.DataAnnotations;

namespace Projectw5.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        public List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();

        public bool IsEditMode { get; set; } = false;

        public void OnGet()
        {
            // Initialize the list when the page loads
            Classes = ClassInformationModel.ClassList;
            NewClass = new ClassInformationModel();
            IsEditMode = false;
        }

        public IActionResult OnGetEdit(int id)
        {
            Classes = ClassInformationModel.ClassList;
            NewClass = ClassInformationModel.ClassList.FirstOrDefault(c => c.Id == id);
            
            if (NewClass == null)
            {
                // If class not found, redirect to index
                return RedirectToPage();
            }
            
            IsEditMode = true;
            return Page();
        }

        public IActionResult OnPostAdd()
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                // If validation fails, return to the page with error messages
                Classes = ClassInformationModel.ClassList;
                return Page();
            }

            // Set the ID and add to the list
            NewClass.Id = ClassInformationModel.GetNextId();
            ClassInformationModel.ClassList.Add(NewClass);

            // Redirect to refresh the page
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                Classes = ClassInformationModel.ClassList;
                IsEditMode = true;
                return Page();
            }

            // Find the existing class and update its properties
            var existingClass = ClassInformationModel.ClassList.FirstOrDefault(c => c.Id == NewClass.Id);
            if (existingClass != null)
            {
                existingClass.ClassName = NewClass.ClassName;
                existingClass.StudentCount = NewClass.StudentCount;
                existingClass.Description = NewClass.Description;
            }

            // Redirect to refresh the page and reset edit mode
            IsEditMode = false;
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            // Find and remove the class with the given ID
            var classToRemove = ClassInformationModel.ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                ClassInformationModel.ClassList.Remove(classToRemove);
            }

            // Redirect to refresh the page
            return RedirectToPage();
        }
    }
}