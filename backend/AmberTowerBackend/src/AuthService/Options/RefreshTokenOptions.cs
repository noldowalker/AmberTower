namespace AuthService.Options;

public sealed class RefreshTokenOptions
{
    public const string SectionName = "RefreshTokens";

    public int LifetimeDays { get; init; } = 30;
}
