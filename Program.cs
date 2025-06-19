using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;

using UniversityAffairs.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using UniversityAffairs.Services;


var builder = WebApplication.CreateBuilder(args);

// ⭐ DbContext
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UniversityDbContext")));

// ⭐ Identity + Role desteği
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UniversityDbContext>()
    .AddDefaultTokenProviders();

// ⭐ Cookie ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// DinkToPdf PDF converter servisi
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<SeatingPlanService>();

System.Runtime.InteropServices.NativeLibrary.Load(
    Path.Combine(Directory.GetCurrentDirectory(), "DinkToPdf", "libwkhtmltox.dll"));

var app = builder.Build();

// ⭐ Geliştirme ortamı dışı ise exception handler
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⭐ Authentication ve Authorization
app.UseAuthentication(); // mutlaka authorization'dan önce gelmeli
app.UseAuthorization();

// ⭐ Varsayılan rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// ⭐ Rolleri otomatik ekleyen fonksiyon
async Task SeedRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "DepartmentHead", "Secretary", "Instructor" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// ⭐ Admin kullanıcısı ekleniyor
async Task SeedAdminUser(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string username = "admin";
    string password = "Admin123!";

    var existingUser = await userManager.FindByNameAsync(username);
    if (existingUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = username,
            FullName = "Bölüm Başkanı",
            Email = "admin@example.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "DepartmentHead");
            Console.WriteLine("Admin kullanıcısı başarıyla eklendi.");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Hata: {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("Admin zaten kayıtlı.");
    }
}

// ⭐ Uygulama başlatılırken roller ve admin kullanıcı eklenir
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRoles(services);
    await SeedAdminUser(services);
}

app.Run();
