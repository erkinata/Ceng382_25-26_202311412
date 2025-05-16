using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projectw9.Data;

var builder = WebApplication.CreateBuilder(args);

// SchoolDbContext (uygulamaya özgü veri)
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection"))); 


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddRazorPages();

// Session ayarları
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});


builder.Services.AddAuthentication();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();
app.Run();