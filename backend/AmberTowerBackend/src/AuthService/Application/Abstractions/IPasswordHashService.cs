using AuthService.Domain;

namespace AuthService.Application;

public interface IPasswordHashService
{
    string HashPassword(AuthUser user, string password);

    bool VerifyPassword(AuthUser user, string password);
}
