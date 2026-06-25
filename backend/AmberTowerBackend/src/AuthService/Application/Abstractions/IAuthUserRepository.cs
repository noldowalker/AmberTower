using AuthService.Domain;

namespace AuthService.Application;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    Task AddAsync(AuthUser user, CancellationToken cancellationToken);
}
