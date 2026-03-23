using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using UniversityAdmission.Data;
using UniversityAdmission.Data.Seed;
using UniversityAdmission.Models.Identity;
using UniversityAdmission.Services;
using UniversityAdmission.Services.Implementations;
using UniversityAdmission.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1?? Добавяне на MVC и Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 2?? Настройка на DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3?? Настройка на Identity с ApplicationUser и роли
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 4?? Dummy Email Sender
builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();

// 5?? Custom Services
builder.Services.AddScoped<IAdmissionService, AdmissionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRoles.SeedAsync(roleManager);
    await SeedAdmin.SeedAsync(userManager, roleManager);
    await SeedSpecialities.SeedAsync(context);
    await SeedData.Initialize(context, userManager);
}

// 7?? Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 8?? Endpoints
// Razor Pages (Identity)
app.MapRazorPages();

// Areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
