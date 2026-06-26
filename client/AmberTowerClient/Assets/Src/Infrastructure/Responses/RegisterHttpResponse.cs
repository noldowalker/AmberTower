using System;

namespace AmberTower.Client.Infrastructure.Responses
{
    [Serializable]
    public sealed class RegisterHttpResponse
    {
        public string userId;
        public string email;
    }
}
