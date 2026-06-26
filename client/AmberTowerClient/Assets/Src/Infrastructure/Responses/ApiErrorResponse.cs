using System;

namespace AmberTower.Client.Infrastructure.Responses
{
    [Serializable]
    public sealed class ApiErrorResponse
    {
        public string code;
        public string message;
    }
}
