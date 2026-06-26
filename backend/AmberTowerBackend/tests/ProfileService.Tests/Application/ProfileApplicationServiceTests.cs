using ProfileService.Application;
using ProfileService.Domain;

namespace ProfileService.Tests.Application;

public sealed class ProfileApplicationServiceTests
{
    [Test]
    public async Task GetMyProfileAsync_ShouldCreateProfile_WhenMissing()
    {
        var repository = new InMemoryPlayerProfileRepository();
        var service = new ProfileApplicationService(repository);

        var result = await service.GetMyProfileAsync(
            Guid.NewGuid().ToString(),
            "player@example.com",
            CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Nickname, Is.EqualTo("player"));
        Assert.That(repository.StoredProfiles, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task UpdateMyProfileAsync_ShouldReturnValidationError_WhenNicknameIsEmpty()
    {
        var service = new ProfileApplicationService(new InMemoryPlayerProfileRepository());

        var result = await service.UpdateMyProfileAsync(
            Guid.NewGuid().ToString(),
            "player@example.com",
            "   ",
            CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(ProfileErrorCodes.ValidationError));
    }

    [Test]
    public async Task UpdateMyProfileAsync_ShouldUpdateNickname_WhenProfileExists()
    {
        var authUserId = Guid.NewGuid();
        var profile = new PlayerProfile
        {
            PlayerId = Guid.NewGuid(),
            AuthUserId = authUserId,
            Email = "player@example.com",
            Nickname = "player",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5),
            UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-5)
        };
        var repository = new InMemoryPlayerProfileRepository(profile);
        var service = new ProfileApplicationService(repository);

        var result = await service.UpdateMyProfileAsync(
            authUserId.ToString(),
            "player@example.com",
            "HeroName",
            CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Nickname, Is.EqualTo("HeroName"));
        Assert.That(repository.StoredProfiles[0].Nickname, Is.EqualTo("HeroName"));
    }

    private sealed class InMemoryPlayerProfileRepository : IPlayerProfileRepository
    {
        public InMemoryPlayerProfileRepository(params PlayerProfile[] profiles)
        {
            StoredProfiles = profiles.ToList();
        }

        public List<PlayerProfile> StoredProfiles { get; }

        public Task<PlayerProfile?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken)
        {
            return Task.FromResult(StoredProfiles.SingleOrDefault(profile => profile.AuthUserId == authUserId));
        }

        public Task AddAsync(PlayerProfile profile, CancellationToken cancellationToken)
        {
            StoredProfiles.Add(profile);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
