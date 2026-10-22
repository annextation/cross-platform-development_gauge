using Cross_platform_development_technologies.Data;
using Cross_platform_development_technologies.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cross_platform_development_technologies.Services
{
    public class UserSeeder
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserSeeder(
            AppDbContext context,
            IPasswordHasher<AppUser> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            await EnsureUserAsync("Admin", "Admin");
            await EnsureUserAsync("User", "User");

            await _context.SaveChangesAsync();
        }

        private async Task EnsureUserAsync(
            string sectionName,
            string role)
        {
            string? login =
                _configuration[$"SeedUsers:{sectionName}:Login"];

            if (string.IsNullOrWhiteSpace(login))
            {
                throw new InvalidOperationException(
                    $"Не задан логин SeedUsers:{sectionName}:Login.");
            }

            login = login.Trim().ToLowerInvariant();

            bool exists = await _context.Users
                .AnyAsync(user => user.Login == login);

            if (exists)
            {
                return;
            }

            string? password =
                _configuration[$"SeedUsers:{sectionName}:Password"];

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    $"Не задан пароль SeedUsers:{sectionName}:Password.");
            }

            var user = new AppUser
            {
                Login = login,
                Role = role
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                password);

            _context.Users.Add(user);
        }
    }
}