namespace AmberTower.Client.Infrastructure.Auth
{
    public sealed class AuthSession
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public long AccessTokenExpiresAtUnixSeconds { get; set; }

        public string RefreshToken { get; set; } = string.Empty;

        public long RefreshTokenExpiresAtUnixSeconds { get; set; }

        public bool HasAccessToken => !string.IsNullOrWhiteSpace(AccessToken);

        public bool HasRefreshToken => !string.IsNullOrWhiteSpace(RefreshToken);

        public bool IsAccessTokenValid(long currentUnixSeconds)
        {
            return HasAccessToken && AccessTokenExpiresAtUnixSeconds > currentUnixSeconds;
        }

        public bool IsRefreshTokenValid(long currentUnixSeconds)
        {
            return HasRefreshToken && RefreshTokenExpiresAtUnixSeconds > currentUnixSeconds;
        }
    }
}
