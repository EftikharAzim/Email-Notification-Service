using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcWebApp.Models; // Ensure this using statement is present

namespace MvcWebApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Use ApplicationUser
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // If you have other DbSet properties for other entities, they would go here.
    // For Identity, the User (ApplicationUser) DbSet is handled by IdentityDbContext.

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}