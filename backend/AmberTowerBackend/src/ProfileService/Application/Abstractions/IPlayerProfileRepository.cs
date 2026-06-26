using ProfileService.Domain;

namespace ProfileService.Application;

public interface IPlayerProfileRepository
{
    Task<PlayerProfile?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken);

    Task AddAsync(PlayerProfile profile, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
