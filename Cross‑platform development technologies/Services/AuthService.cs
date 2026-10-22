using Cross_platform_development_technologies.Data;
using Cross_platform_development_technologies.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cross_platform_development_technologies.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AuthService(
            AppDbContext context,
            IPasswordHasher<AppUser> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<AppUser?> ValidateCredentialsAsync(
            string login,
            string password)
        {
            if (string.IsNullOrWhiteSpace(login)
                || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            string normalizedLogin = login.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(
                    user => user.Login == normalizedLogin);

            if (user is null)
            {
                return null;
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (verificationResult ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(
                    user,
                    password);

                await _context.SaveChangesAsync();
            }

            return user;
        }
    }
}