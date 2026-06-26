using AuthService.Application;
using AuthService.Domain;

namespace AuthService.Tests.Application;

public sealed class AuthApplicationServiceTests
{
    [Test]
    public async Task RegisterAsync_ShouldReturnDuplicateEmail_WhenUserAlreadyExists()
    {
        var existingUser = CreateUser();
        var service = CreateService(new InMemoryAuthUserRepository(existingUser));

        var result = await service.RegisterAsync("player@example.com", "secret123", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(AuthErrorCodes.DuplicateEmail));
    }

    [Test]
    public async Task RegisterAsync_ShouldCreateUser_WhenDataIsValid()
    {
        var repository = new InMemoryAuthUserRepository();
        var service = CreateService(repository);

        var result = await service.RegisterAsync("player@example.com", "secret123", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Email, Is.EqualTo("player@example.com"));
        Assert.That(repository.StoredUsers, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task LoginAsync_ShouldReturnInvalidCredentials_WhenPasswordDoesNotMatch()
    {
        var user = CreateUser();
        var service = CreateService(new InMemoryAuthUserRepository(user));

        var result = await service.LoginAsync("player@example.com", "wrong-password", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(AuthErrorCodes.InvalidCredentials));
    }

    [Test]
    public async Task LoginAsync_ShouldReturnTokenPair_WhenCredentialsAreValid()
    {
        var user = CreateUser();
        var refreshTokenRepository = new InMemoryRefreshTokenRepository();
        var service = CreateService(
            new InMemoryAuthUserRepository(user),
            refreshTokenRepository,
            new FakeRefreshTokenService("refresh-token-1"));

        var result = await service.LoginAsync("player@example.com", "secret123", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.AccessToken, Is.EqualTo("test-token"));
        Assert.That(result.RefreshToken, Is.EqualTo("refresh-token-1"));
        Assert.That(refreshTokenRepository.StoredTokens, Has.Count.EqualTo(1));
        Assert.That(refreshTokenRepository.StoredTokens[0].TokenHash, Is.EqualTo("hashed:refresh-token-1"));
    }

    [Test]
    public async Task RefreshAsync_ShouldRotateRefreshToken_WhenTokenIsValid()
    {
        var user = CreateUser();
        var oldToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = "hashed:refresh-token-1",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        };
        var refreshTokenRepository = new InMemoryRefreshTokenRepository(oldToken);
        var service = CreateService(
            new InMemoryAuthUserRepository(user),
            refreshTokenRepository,
            new FakeRefreshTokenService("refresh-token-2"));

        var result = await service.RefreshAsync("refresh-token-1", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.RefreshToken, Is.EqualTo("refresh-token-2"));
        Assert.That(oldToken.RevokedAtUtc, Is.Not.Null);
        Assert.That(oldToken.ReplacedByTokenId, Is.EqualTo(refreshTokenRepository.StoredTokens[1].Id));
        Assert.That(refreshTokenRepository.StoredTokens, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task RefreshAsync_ShouldRejectReusedRefreshToken_WhenTokenWasRotated()
    {
        var user = CreateUser();
        var oldToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = "hashed:refresh-token-1",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            RevokedAtUtc = DateTime.UtcNow.AddMinutes(-1),
            ReplacedByTokenId = Guid.NewGuid()
        };
        var service = CreateService(
            new InMemoryAuthUserRepository(user),
            new InMemoryRefreshTokenRepository(oldToken),
            new FakeRefreshTokenService("refresh-token-2"));

        var result = await service.RefreshAsync("refresh-token-1", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(AuthErrorCodes.InvalidRefreshToken));
    }

    [Test]
    public async Task RefreshAsync_ShouldRejectExpiredRefreshToken()
    {
        var user = CreateUser();
        var expiredToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = "hashed:refresh-token-1",
            CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1)
        };
        var service = CreateService(
            new InMemoryAuthUserRepository(user),
            new InMemoryRefreshTokenRepository(expiredToken),
            new FakeRefreshTokenService("refresh-token-2"));

        var result = await service.RefreshAsync("refresh-token-1", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(AuthErrorCodes.InvalidRefreshToken));
    }

    [Test]
    public async Task LogoutAsync_ShouldRevokeRefreshToken_WhenTokenExists()
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TokenHash = "hashed:refresh-token-1",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        };
        var service = CreateService(
            new InMemoryAuthUserRepository(),
            new InMemoryRefreshTokenRepository(token),
            new FakeRefreshTokenService("refresh-token-2"));

        var result = await service.LogoutAsync("refresh-token-1", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(token.RevokedAtUtc, Is.Not.Null);
    }

    private static AuthUser CreateUser()
    {
        return new AuthUser
        {
            Id = Guid.NewGuid(),
            Email = "player@example.com",
            PasswordHash = "hashed:secret123",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static AuthApplicationService CreateService(
        InMemoryAuthUserRepository userRepository,
        InMemoryRefreshTokenRepository? refreshTokenRepository = null,
        FakeRefreshTokenService? refreshTokenService = null)
    {
        return new AuthApplicationService(
            userRepository,
            refreshTokenRepository ?? new InMemoryRefreshTokenRepository(),
            new FakePasswordHashService(),
            new FakeTokenService(),
            refreshTokenService ?? new FakeRefreshTokenService("refresh-token"));
    }

    private sealed class InMemoryAuthUserRepository : IAuthUserRepository
    {
        public InMemoryAuthUserRepository(params AuthUser[] users)
        {
            StoredUsers = users.ToList();
        }

        public List<AuthUser> StoredUsers { get; }

        public Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(StoredUsers.SingleOrDefault(user => user.Id == id));
        }

        public Task<AuthUser?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(StoredUsers.SingleOrDefault(user => user.Email == normalizedEmail));
        }

        public Task AddAsync(AuthUser user, CancellationToken cancellationToken)
        {
            StoredUsers.Add(user);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryRefreshTokenRepository : IRefreshTokenRepository
    {
        public InMemoryRefreshTokenRepository(params RefreshToken[] refreshTokens)
        {
            StoredTokens = refreshTokens.ToList();
        }

        public List<RefreshToken> StoredTokens { get; }

        public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
        {
            return Task.FromResult(StoredTokens.SingleOrDefault(token => token.TokenHash == tokenHash));
        }

        public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            StoredTokens.Add(refreshToken);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHashService : IPasswordHashService
    {
        public string HashPassword(AuthUser user, string password)
        {
            return $"hashed:{password}";
        }

        public bool VerifyPassword(AuthUser user, string password)
        {
            return user.PasswordHash == $"hashed:{password}";
        }
    }

    private sealed class FakeTokenService : ITokenService
    {
        public AccessTokenResult Issue(AuthUser user)
        {
            return new AccessTokenResult
            {
                Token = "test-token",
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1)
            };
        }
    }

    private sealed class FakeRefreshTokenService : IRefreshTokenService
    {
        private readonly Queue<string> _tokens;

        public FakeRefreshTokenService(params string[] tokens)
        {
            _tokens = new Queue<string>(tokens);
        }

        public RefreshTokenIssueResult Issue()
        {
            var token = _tokens.Count == 0
                ? "refresh-token"
                : _tokens.Dequeue();

            return new RefreshTokenIssueResult
            {
                Token = token,
                TokenHash = Hash(token),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
            };
        }

        public string Hash(string token)
        {
            return $"hashed:{token}";
        }
    }
}
