using Microsoft.EntityFrameworkCore;
using ProfileService.Domain;

namespace ProfileService.Persistence;

public sealed class ProfileDbContext : DbContext
{
    public const string ConnectionStringName = "ProfileDatabase";

    public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlayerProfile> Profiles => Set<PlayerProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("profile");

        modelBuilder.Entity<PlayerProfile>(entity =>
        {
            entity.ToTable("player_profiles");
            entity.HasKey(profile => profile.PlayerId);

            entity.Property(profile => profile.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(profile => profile.Nickname)
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(profile => profile.CreatedAtUtc)
                .IsRequired();

            entity.Property(profile => profile.UpdatedAtUtc)
                .IsRequired();

            entity.HasIndex(profile => profile.AuthUserId)
                .IsUnique();
        });
    }
}
