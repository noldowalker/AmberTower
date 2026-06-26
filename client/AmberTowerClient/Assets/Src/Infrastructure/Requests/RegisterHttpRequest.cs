using System;

namespace AmberTower.Client.Infrastructure.Requests
{
    [Serializable]
    public sealed class RegisterHttpRequest
    {
        public string email;
        public string password;

        public RegisterHttpRequest(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}
