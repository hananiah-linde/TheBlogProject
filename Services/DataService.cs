using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services;

public class DataService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<BlogUser> _userManager;
    private readonly IConfiguration _configuration;

    public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task ManageDataAsync()
    {
        //Ensure database is up to date
        await _dbContext.Database.MigrateAsync();

        //Task 1: Seed a few Roles into the system
        await SeedRolesAsync();

        //Task 2: Seed a few Users into the system
        await SeedUsersAsync();

    }

    private async Task SeedRolesAsync()
    {
        if (_dbContext.Roles.Any())
        {
            return;
        }

        foreach (var role in Enum.GetNames(typeof(BlogRole)))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private async Task SeedUsersAsync()
    {
        if (_dbContext.Users.Any())
        {
            return;
        }

        string email = _configuration["SeedUserConfig:Email"];
        string userName = _configuration["SeedUserConfig:Email"];
        string password = _configuration["SeedUserConfig:Password"];

        var adminUser = new BlogUser()
        {
            Email = email,
            UserName = userName,
            FirstName = "Hananiah",
            LastName = "Linde",
            DisplayName = "Administrator",
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(adminUser, password);
        await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

    }

}
