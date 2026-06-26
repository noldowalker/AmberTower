using System;

namespace AmberTower.Client.Infrastructure.Requests
{
    [Serializable]
    public sealed class LoginHttpRequest
    {
        public string email;
        public string password;

        public LoginHttpRequest(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}
