using Microsoft.EntityFrameworkCore;
using ProfileService.Application;
using ProfileService.Domain;

namespace ProfileService.Persistence;

public sealed class PlayerProfileRepository : IPlayerProfileRepository
{
    private readonly ProfileDbContext _dbContext;

    public PlayerProfileRepository(ProfileDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PlayerProfile?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken)
    {
        return _dbContext.Profiles.SingleOrDefaultAsync(profile => profile.AuthUserId == authUserId, cancellationToken);
    }

    public async Task AddAsync(PlayerProfile profile, CancellationToken cancellationToken)
    {
        await _dbContext.Profiles.AddAsync(profile, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
