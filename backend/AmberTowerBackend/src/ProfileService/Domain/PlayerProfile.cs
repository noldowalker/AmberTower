namespace ProfileService.Domain;

public sealed class PlayerProfile
{
    public Guid PlayerId { get; set; }

    public Guid AuthUserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Nickname { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
