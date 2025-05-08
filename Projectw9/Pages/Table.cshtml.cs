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

//-

//Ai promt 4:Update my Razor Page file named index.cshtml.cs according to the given information: The class should inherit from AuthPageModel for automatic handling of login checks.
//  This page should be accessible only to logged in users. If session or cookies are invalid, the user should be redirected. 
// The Table page is the main dashboard shown after login and will later include features like filtering, CRUD and export.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projectw9.Models;
using Projectw9.Data;
using System.ComponentModel.DataAnnotations;
using Projectw9.Utils;
using Microsoft.Extensions.Primitives;

namespace Projectw9.Pages
{
    public class TableModel : AuthPageModel
    {

        
        private readonly SchoolDbContext _context;
    
        public TableModel(SchoolDbContext context)
        {
            _context = context;
        }

        public string UserToken { get; private set; }

        [BindProperty]
        public Class NewClass { get; set; } = new Class();

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


        public async Task OnGetAsync()
{
    base.OnPageHandlerExecuting();
    UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

    var query = _context.Classes.AsQueryable();
    query = ApplyFilters(query);

    var totalItems = await query.CountAsync();
    
    
    TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
    TotalPages = TotalPages == 0 ? 1 : TotalPages; 

   
    CurrentPage = CurrentPage < 1 ? 1 : 
                  CurrentPage > TotalPages ? TotalPages : 
                  CurrentPage;

    var dbClasses = await query
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToListAsync();

    ClassTable = dbClasses
        .Select(c => ClassInformationTable.FromClassInformationModel(c))
        .ToList();

    NewClass = new Class();
    IsEditMode = false;
}

      private IQueryable<Class> ApplyFilters(IQueryable<Class> query)
{
    // Class name filter
    if (!string.IsNullOrWhiteSpace(ClassNameFilter))
        query = query.Where(c => c.ClassName.Contains(ClassNameFilter));

    // Student count filters
    if (MinStudentCount.HasValue)
        query = query.Where(c => c.StudentCount >= MinStudentCount.Value);
    
    if (MaxStudentCount.HasValue)
        query = query.Where(c => c.StudentCount <= MaxStudentCount.Value);

    // Description filter
    if (!string.IsNullOrWhiteSpace(DescriptionFilter))
        query = query.Where(c => c.Description != null && c.Description.Contains(DescriptionFilter));

    // SADECE AKTİF KAYITLARI GÖSTER (IsActive == true)
    query = query.Where(c => c.IsActive); 

    return query;
}


public async Task<IActionResult> OnGetEditAsync(int? id)
{
    base.OnPageHandlerExecuting();
    UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

    if (id == null)
        return NotFound();

    // Sadece düzenlenecek kaydı getir, filtre/pagination'ı yeniden yükleme
    NewClass = await _context.Classes.FindAsync(id);

    if (NewClass == null)
        return NotFound();

    IsEditMode = true;


    return Page();
}


        public async Task<IActionResult> OnPostAddAsync()
        {
            base.OnPageHandlerExecuting();
            UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            _context.Classes.Add(NewClass);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { ClassNameFilter, MinStudentCount, MaxStudentCount, DescriptionFilter, CurrentPage });
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            base.OnPageHandlerExecuting();
            UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

            if (!ModelState.IsValid)
            {
                IsEditMode = true;
                await OnGetAsync();
                return Page();
            }

            _context.Attach(NewClass).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { ClassNameFilter, MinStudentCount, MaxStudentCount, DescriptionFilter, CurrentPage });
        }




        public async Task<IActionResult> OnPostDeleteAsync(int? id)
{
    base.OnPageHandlerExecuting();
    UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

    if (id == null)
        return NotFound();

    var classToDelete = await _context.Classes.FindAsync(id);
    if (classToDelete != null)
    {
        
        classToDelete.IsActive = false; 
        await _context.SaveChangesAsync(); 
    }

    return RedirectToPage(new 
    { 
        ClassNameFilter, 
        MinStudentCount, 
        MaxStudentCount, 
        DescriptionFilter, 
        CurrentPage 
    });
}


//veritabanındaki aktifliği sıfır yapmak yerine silen metot(yerine üstteki metot kullanıldı.)

        // public async Task<IActionResult> OnPostDeleteAsync(int? id)
        // {
        //     base.OnPageHandlerExecuting();
        //     UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

        //     if (id == null)
        //         return NotFound();

        //     var classToDelete = await _context.Classes.FindAsync(id);
        //     if (classToDelete != null)
        //     {
        //         _context.Classes.Remove(classToDelete);
        //         await _context.SaveChangesAsync();
        //     }

        //     return RedirectToPage(new { ClassNameFilter, MinStudentCount, MaxStudentCount, DescriptionFilter, CurrentPage });
        // }

public async Task<IActionResult> OnPostExportAllAsync(string selectedColumns)
{
    base.OnPageHandlerExecuting();
    UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

    // Tüm verileri veritabanından çek
    var allData = await _context.Classes.ToListAsync();

    // Sütun filtreleme
    List<string>? columnsToExport = null;
    if (!string.IsNullOrEmpty(selectedColumns))
    {
        columnsToExport = selectedColumns.Split(',').ToList();
    }

    // JSON'a dönüştür
    var jsonData = JsonExportUtil.Instance.ExportToJson(allData, columnsToExport);

    // İndirme dosyası olarak döndür
    return File(
        System.Text.Encoding.UTF8.GetBytes(jsonData),
        "application/json",
        "all_classes.json"
    );
}

public async Task<IActionResult> OnPostExportFilteredAsync(
    string selectedColumns,
    string ClassNameFilter,
    int? MinStudentCount,
    int? MaxStudentCount,
    string DescriptionFilter)
{
    base.OnPageHandlerExecuting();
    UserToken = HttpContext.Session.GetString("token") ?? string.Empty;

    // Filtreleri uygula
    this.ClassNameFilter = ClassNameFilter;
    this.MinStudentCount = MinStudentCount;
    this.MaxStudentCount = MaxStudentCount;
    this.DescriptionFilter = DescriptionFilter;

    // Veritabanı sorgusunu oluştur
    var query = _context.Classes.AsQueryable();
    query = ApplyFilters(query); // Daha önce tanımlanan filtreleme metodu

    // Filtrelenmiş verileri çek
    var filteredData = await query.ToListAsync();

    // Sütun filtreleme
    List<string>? columnsToExport = null;
    if (!string.IsNullOrEmpty(selectedColumns))
    {
        columnsToExport = selectedColumns.Split(',').ToList();
    }

    // JSON'a dönüştür
    var jsonData = JsonExportUtil.Instance.ExportToJson(filteredData, columnsToExport);

    // İndirme dosyası olarak döndür
    return File(
        System.Text.Encoding.UTF8.GetBytes(jsonData),
        "application/json",
        "filtered_classes.json"
    );
}


}}