namespace AmberTower.Client.Infrastructure.Responses
{
    public sealed class BackendApiResult<TResponse>
    {
        private BackendApiResult(bool isSuccess, long statusCode, string message, TResponse response)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
            Response = response;
        }

        public bool IsSuccess { get; }

        public long StatusCode { get; }

        public string Message { get; }

        public TResponse Response { get; }

        public static BackendApiResult<TResponse> Ok(long statusCode, TResponse response)
        {
            return new BackendApiResult<TResponse>(true, statusCode, string.Empty, response);
        }

        public static BackendApiResult<TResponse> Fail(long statusCode, string message)
        {
            return new BackendApiResult<TResponse>(false, statusCode, message, default);
        }
    }
}
