using Microsoft.AspNetCore.Identity;
using SkillNet.Infrastructure.Identity;

namespace SkillNet.Infrastructure.Persistence.Seeders;

public static class UserSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUserAsync(userManager,
            email: "admin@skillnet.vn",
            fullName: "Admin Skill.Net",
            password: "Admin@123456",
            role: "Admin");

        await CreateUserAsync(userManager,
            email: "member@skillnet.vn",
            fullName: "Nguyễn Văn A",
            password: "Member@123456",
            role: "Member");
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email, string fullName, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) is not null) return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);
    }
}
