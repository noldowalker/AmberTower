using System;

namespace AmberTower.Client.Infrastructure.Requests
{
    [Serializable]
    public sealed class RefreshHttpRequest
    {
        public string refreshToken;

        public RefreshHttpRequest(string refreshToken)
        {
            this.refreshToken = refreshToken;
        }
    }
}
