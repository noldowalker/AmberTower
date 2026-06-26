using System;

namespace AmberTower.Client.Infrastructure.Responses
{
    [Serializable]
    public sealed class AuthTokenHttpResponse
    {
        public string userId;
        public string email;
        public string accessToken;
        public long accessTokenExpiresAtUnixSeconds;
        public string refreshToken;
        public long refreshTokenExpiresAtUnixSeconds;
    }
}
