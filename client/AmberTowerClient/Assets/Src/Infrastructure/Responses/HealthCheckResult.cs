namespace AmberTower.Client.Infrastructure.Responses
{
    public sealed class HealthCheckResult
    {
        private HealthCheckResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }

        public string Message { get; }

        public static HealthCheckResult Ok(string message)
        {
            return new HealthCheckResult(true, message);
        }

        public static HealthCheckResult Fail(string message)
        {
            return new HealthCheckResult(false, message);
        }
    }
}
