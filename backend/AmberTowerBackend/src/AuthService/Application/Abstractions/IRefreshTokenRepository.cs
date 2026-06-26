using AuthService.Domain;

namespace AuthService.Application;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
