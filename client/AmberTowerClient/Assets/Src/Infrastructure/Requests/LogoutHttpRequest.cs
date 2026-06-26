using System;

namespace AmberTower.Client.Infrastructure.Requests
{
    [Serializable]
    public sealed class LogoutHttpRequest
    {
        public string refreshToken;

        public LogoutHttpRequest(string refreshToken)
        {
            this.refreshToken = refreshToken;
        }
    }
}
