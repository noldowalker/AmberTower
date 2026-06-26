using AuthService.Application;
using AuthService.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Infrastructure;

public sealed class SecureRefreshTokenService : IRefreshTokenService
{
    private const int TokenByteLength = 64;

    private readonly RefreshTokenOptions _refreshTokenOptions;

    public SecureRefreshTokenService(IOptions<RefreshTokenOptions> refreshTokenOptions)
    {
        _refreshTokenOptions = refreshTokenOptions.Value;
    }

    public RefreshTokenIssueResult Issue()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(TokenByteLength);
        var token = Convert.ToBase64String(tokenBytes);

        return new RefreshTokenIssueResult
        {
            Token = token,
            TokenHash = Hash(token),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_refreshTokenOptions.LifetimeDays)
        };
    }

    public string Hash(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
