using AuthService.Application;
using AuthService.Domain;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure;

public sealed class PasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<AuthUser> _passwordHasher = new();

    public string HashPassword(AuthUser user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(AuthUser user, string password)
    {
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return verificationResult is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
