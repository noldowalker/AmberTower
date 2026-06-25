using AuthService.Application;
using AuthService.Domain;

namespace AuthService.Tests.Application;

public sealed class AuthApplicationServiceTests
{
    [Test]
    public async Task RegisterAsync_ShouldReturnDuplicateEmail_WhenUserAlreadyExists()
    {
        var existingUser = new AuthUser
        {
            Id = Guid.NewGuid(),
            Email = "player@example.com",
            PasswordHash = "existing-hash",
            CreatedAtUtc = DateTime.UtcNow
        };
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
        var user = new AuthUser
        {
            Id = Guid.NewGuid(),
            Email = "player@example.com",
            PasswordHash = "hashed:secret123",
            CreatedAtUtc = DateTime.UtcNow
        };
        var service = CreateService(new InMemoryAuthUserRepository(user));

        var result = await service.LoginAsync("player@example.com", "wrong-password", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(AuthErrorCodes.InvalidCredentials));
    }

    [Test]
    public async Task LoginAsync_ShouldReturnAccessToken_WhenCredentialsAreValid()
    {
        var user = new AuthUser
        {
            Id = Guid.NewGuid(),
            Email = "player@example.com",
            PasswordHash = "hashed:secret123",
            CreatedAtUtc = DateTime.UtcNow
        };
        var service = CreateService(new InMemoryAuthUserRepository(user));

        var result = await service.LoginAsync("player@example.com", "secret123", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.AccessToken, Is.EqualTo("test-token"));
    }

    private static AuthApplicationService CreateService(InMemoryAuthUserRepository repository)
    {
        return new AuthApplicationService(repository, new FakePasswordHashService(), new FakeTokenService());
    }

    private sealed class InMemoryAuthUserRepository : IAuthUserRepository
    {
        public InMemoryAuthUserRepository(params AuthUser[] users)
        {
            StoredUsers = users.ToList();
        }

        public List<AuthUser> StoredUsers { get; }

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
}
