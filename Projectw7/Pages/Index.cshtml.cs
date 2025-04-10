//Ai prompt: Generate Razor Page button examples that use asp-page-handler attributes for actions like Add, Edit, and Delete. Each button should be correctly 
// linked to C# handler methods in the PageModel
//  (e.g., OnPostAdd(), OnPostDelete(int id)). Additionally, provide C# implementations of these methods inside the .cshtml.cs file, 
// ensuring they return IActionResult. The output should be structured and ready to use in an ASP.NET Core Razor Pages project."

//-

//Ai Prompt 2: Implement filtering and pagination logic in the OnGet method. Filtering must be done on the backend data list, not on the frontend. 
//Store filtered results in a List<ClassInformationTable>. Also, generate at least 100 sample records to test pagination effectively. 
//Implement properties for current page, total pages, page size, and filtered results.

//-

//  Ai promt 3: Use the code I gave for the class named Utils.cs. Write a generic method inside: this
//  method should be able to take any model class and export it in JSON format. In other words, 
//  regardless of the model type, JSON output should be easily obtained with this helper class. In requests coming from the Export button, the data should be processed through 
//  this helper class and returned as JSON. It should create the output appropriately according to the selected columns, with or without filtering.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projectw7.Models;
using System.ComponentModel.DataAnnotations;
using Projectw7.Utils;
using Microsoft.Extensions.Primitives;

namespace Projectw7.Pages
{
    public class IndexModel : PageModel
    {
        // Static constructor to initialize sample data
        static IndexModel()
        {
            // Generate sample data if not already initialized
            if (ClassInformationModel.ClassList.Count == 0)
            {
                GenerateSampleData();
            }
        }

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        public List<ClassInformationTable> ClassTable { get; set; } = new List<ClassInformationTable>();

        public bool IsEditMode { get; set; } = false;

        // Pagination properties
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? ClassNameFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MinStudentCount { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MaxStudentCount { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? DescriptionFilter { get; set; }

        public void OnGet()
        {
            // Apply filters and pagination
            var filteredClasses = FilterClasses(ClassInformationModel.ClassList);
            
            // Calculate total pages
            TotalPages = (int)Math.Ceiling(filteredClasses.Count / (double)PageSize);
            
            // Ensure CurrentPage is valid
            if (CurrentPage < 1)
                CurrentPage = 1;
            if (CurrentPage > TotalPages && TotalPages > 0)
                CurrentPage = TotalPages;
            
            // Apply pagination
            var paginatedClasses = filteredClasses
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            
            // Convert to ClassInformationTable
            ClassTable = paginatedClasses
                .Select(ClassInformationTable.FromClassInformationModel)
                .ToList();
            
            NewClass = new ClassInformationModel();
            IsEditMode = false;
        }

        private List<ClassInformationModel> FilterClasses(List<ClassInformationModel> classes)
        {
            var filteredClasses = classes.AsQueryable();
            
            // Apply class name filter
            if (!string.IsNullOrWhiteSpace(ClassNameFilter))
            {
                filteredClasses = filteredClasses.Where(c => 
                    c.ClassName.Contains(ClassNameFilter, StringComparison.OrdinalIgnoreCase));
            }
            
            // Apply student count min filter
            if (MinStudentCount.HasValue)
            {
                filteredClasses = filteredClasses.Where(c => c.StudentCount >= MinStudentCount.Value);
            }
            
            // Apply student count max filter
            if (MaxStudentCount.HasValue)
            {
                filteredClasses = filteredClasses.Where(c => c.StudentCount <= MaxStudentCount.Value);
            }
            
            // Apply description filter
            if (!string.IsNullOrWhiteSpace(DescriptionFilter))
            {
                filteredClasses = filteredClasses.Where(c => 
                    c.Description != null && 
                    c.Description.Contains(DescriptionFilter, StringComparison.OrdinalIgnoreCase));
            }
            
            return filteredClasses.ToList();
        }

        public IActionResult OnGetEdit(int id)
        {
            // Get and filter classes
            var filteredClasses = FilterClasses(ClassInformationModel.ClassList);
            
            // Apply pagination
            TotalPages = (int)Math.Ceiling(filteredClasses.Count / (double)PageSize);
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;
            
            var paginatedClasses = filteredClasses
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            
            // Convert to ClassInformationTable
            ClassTable = paginatedClasses
                .Select(ClassInformationTable.FromClassInformationModel)
                .ToList();
            
            // Set up edit mode
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
                var filteredClasses = FilterClasses(ClassInformationModel.ClassList);
                TotalPages = (int)Math.Ceiling(filteredClasses.Count / (double)PageSize);
                var paginatedClasses = filteredClasses
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                ClassTable = paginatedClasses
                    .Select(ClassInformationTable.FromClassInformationModel)
                    .ToList();
                return Page();
            }

            // Set the ID and add to the list
            NewClass.Id = ClassInformationModel.GetNextId();
            ClassInformationModel.ClassList.Add(NewClass);

            // Redirect to refresh the page
            return RedirectToPage(new 
            { 
                ClassNameFilter, 
                MinStudentCount, 
                MaxStudentCount, 
                DescriptionFilter, 
                CurrentPage 
            });
        }

        public IActionResult OnPostEdit()
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                var filteredClasses = FilterClasses(ClassInformationModel.ClassList);
                TotalPages = (int)Math.Ceiling(filteredClasses.Count / (double)PageSize);
                var paginatedClasses = filteredClasses
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                ClassTable = paginatedClasses
                    .Select(ClassInformationTable.FromClassInformationModel)
                    .ToList();
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
            return RedirectToPage(new 
            { 
                ClassNameFilter, 
                MinStudentCount, 
                MaxStudentCount, 
                DescriptionFilter, 
                CurrentPage 
            });
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
            return RedirectToPage(new 
            { 
                ClassNameFilter, 
                MinStudentCount, 
                MaxStudentCount, 
                DescriptionFilter, 
                CurrentPage 
            });
        }

        // Export all data to JSON
        public IActionResult OnPostExportAll(string selectedColumns)
        {
            // Parse selected columns
            List<string>? columnsToExport = null;
            if (!string.IsNullOrEmpty(selectedColumns))
            {
                columnsToExport = selectedColumns.Split(',').ToList();
            }

            // Export all data to JSON using our singleton utility
            var jsonData = JsonExportUtil.Instance.ExportToJson(ClassInformationModel.ClassList, columnsToExport);
            
            // Return as file download
            return File(System.Text.Encoding.UTF8.GetBytes(jsonData), "application/json", "all_classes.json");
        }

        // Export filtered data to JSON
public IActionResult OnPostExportFiltered(
    string selectedColumns, 
    string ClassNameFilter, 
    int? MinStudentCount, 
    int? MaxStudentCount, 
    string DescriptionFilter)
{
    // Apply the filters
    this.ClassNameFilter = ClassNameFilter;
    this.MinStudentCount = MinStudentCount;
    this.MaxStudentCount = MaxStudentCount;
    this.DescriptionFilter = DescriptionFilter;
    
    // Get filtered data
    var filteredData = FilterClasses(ClassInformationModel.ClassList);
    
    // Parse selected columns
    List<string>? columnsToExport = null;
    if (!string.IsNullOrEmpty(selectedColumns))
    {
        columnsToExport = selectedColumns.Split(',').ToList();
    }
    
    // Export filtered data to JSON using our singleton utility
    var jsonData = JsonExportUtil.Instance.ExportToJson(filteredData, columnsToExport);
    
    // Return as file download
    return File(System.Text.Encoding.UTF8.GetBytes(jsonData), "application/json", "filtered_classes.json");
}

        private static void GenerateSampleData()
        {
            string[] classNames = { "Mathematics", "Physics", "Chemistry", "Biology", "Computer Science", 
                                   "English", "History", "Geography", "Art", "Music" };
            
            string[] descriptions = { 
                "An introductory course covering fundamental concepts",
                "Advanced level course for specialized students",
                "Practical laboratory sessions and theoretical lessons",
                "Interactive learning with group projects",
                "Self-paced learning with regular assessments",
                "Intensive course with daily assignments",
                "Project-based learning approach",
                "Research-oriented course with final thesis",
                "Combination of theory and practical application",
                "Exploratory course with field trips and guest lectures"
            };
            
            Random random = new Random();
            
            for (int i = 1; i <= 100; i++)
            {
                ClassInformationModel.ClassList.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"{classNames[random.Next(classNames.Length)]} {random.Next(101, 500)}",
                    StudentCount = random.Next(5, 100),
                    Description = descriptions[random.Next(descriptions.Length)]
                });
            }
        }
    }
}