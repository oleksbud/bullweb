using Bull.DataAccess.Data;
using Bull.Models.Models;
using Bull.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bull.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public DbInitializer(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }
    public void Initialize()
    {
        // migration if they are not applied
        try
        {
            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                _dbContext.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            
        }
        
        // create roles if they are not exist
        var customerRoleExists = _roleManager.RoleExistsAsync(StaticDetails.RoleCustomer)
            .GetAwaiter()
            .GetResult();
        if (!customerRoleExists)
        {
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleCustomer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleCompany)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.RoleEmployee)).GetAwaiter().GetResult();

            // create admin user if roles had been not created
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@test.loc",
                Email = "admin@test.loc",
                Name = 1,
                PhoneNumber = "(555)-12345",
                StreetAddress = "Test Ave 123",
                State = "TX",
                PostalCode = "64739",
                City = "FortWorth"
            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@test.loc");
            _userManager.AddToRoleAsync(user, StaticDetails.RoleAdmin).GetAwaiter().GetResult();
        }
    }
}