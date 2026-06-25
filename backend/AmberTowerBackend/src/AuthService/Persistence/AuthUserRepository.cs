using AuthService.Application;
using AuthService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence;

public sealed class AuthUserRepository : IAuthUserRepository
{
    private readonly AuthDbContext _dbContext;

    public AuthUserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AuthUser?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(AuthUser user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
