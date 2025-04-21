//Ai prompt: We will make a task. So we will go step by step i already created an empty project on Visual Code named Projectw8. Keep it on from there. Here is insturctions:
// In this Razor Pages exercise, we will create a simple form on the left side and a data table on the right side of the page using Bootstrap for styling(do not do anything 
// for bootsraps we will already discuss it with next steps). We will not use JavaScript in this project. Everything will be done with C# and Razor Pages only.
//On the left side of the page, there will be a form that collects: o Class Name o Student Count o Description 
// • On the right side, there will be a table that displays all the submitted class data. 
// • The table will have the following columns: o Id o Class Name o Student Count o Description o Actions (Edit and Delete) 
// The data should be validated and added to a static list each time the form is submitted. The data will then be displayed in the table.
using System.ComponentModel.DataAnnotations;

namespace Projectw8.Models
{
    public class ClassInformationModel
    {
        // Static list to serve as in-memory database
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();
        
        // Static method to get the next ID
        public static int GetNextId()
        {
            return ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required")]
        [Display(Name = "Class Name")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required")]
        [Range(1, 100, ErrorMessage = "Student count must be between 1 and 100")]
        [Display(Name = "Number of Students")]
        public int? StudentCount { get; set; }

        [Display(Name = "Class Description")]
        public string Description { get; set; }
    }
}