using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcWebApp.Data;
using MvcWebApp.Models; // For ApplicationUser
using MvcWebApp.Services; // For IEmailSender and EmailSender, EmailSettings

var builder = WebApplication.CreateBuilder(args);

// 1. Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); // Or UseSqlServer, UseNpgsql, etc.

// 2. Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Set to true if you implement email confirmation
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // For password reset, email confirmation tokens

// 3. Configure Email Settings and Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// If you scaffolded with --auth Individual, Razor Pages for Identity UI might be added.
// If so, you might see: builder.Services.AddRazorPages();
// Ensure it's present if you rely on default Identity UI pages.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Helpful for EF Core migrations during development
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // IMPORTANT: Must come before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// If you added Razor Pages for Identity:
// app.MapRazorPages(); 

app.Run();