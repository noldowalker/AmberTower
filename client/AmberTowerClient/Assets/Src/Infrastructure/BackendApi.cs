using System;
using System.Collections;
using System.Text;
using AmberTower.Client.Infrastructure.Requests;
using AmberTower.Client.Infrastructure.Responses;
using UnityEngine;
using UnityEngine.Networking;

namespace AmberTower.Client.Infrastructure
{
    public sealed class BackendApi
    {
        private const int REQUEST_TIMEOUT_SECONDS = 10;

        public IEnumerator CheckHealth(string url, Action<HealthCheckResult> onCompleted)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                onCompleted?.Invoke(HealthCheckResult.Fail("URL is empty."));
                yield break;
            }

            var request = UnityWebRequest.Get(url);
            request.timeout = REQUEST_TIMEOUT_SECONDS;

            UnityWebRequestAsyncOperation operation;

            try
            {
                operation = request.SendWebRequest();
            }
            catch (Exception exception)
            {
                request.Dispose();
                onCompleted?.Invoke(HealthCheckResult.Fail(exception.Message));
                yield break;
            }

            yield return operation;

            if (request.result == UnityWebRequest.Result.Success)
            {
                onCompleted?.Invoke(HealthCheckResult.Ok(request.downloadHandler.text));
                request.Dispose();
                yield break;
            }

            var errorMessage = string.IsNullOrWhiteSpace(request.error)
                ? $"Request failed with response code {request.responseCode}."
                : request.error;

            onCompleted?.Invoke(HealthCheckResult.Fail(errorMessage));
            request.Dispose();
        }

        public IEnumerator Register(
            string apiBaseUrl,
            string email,
            string password,
            Action<BackendApiResult<RegisterHttpResponse>> onCompleted)
        {
            var url = BuildApiUrl(apiBaseUrl, "/api/auth/register");
            var payload = new RegisterHttpRequest(email, password);

            yield return PostJson(url, payload, onCompleted);
        }

        public IEnumerator Login(
            string apiBaseUrl,
            string email,
            string password,
            Action<BackendApiResult<AuthTokenHttpResponse>> onCompleted)
        {
            var url = BuildApiUrl(apiBaseUrl, "/api/auth/login");
            var payload = new LoginHttpRequest(email, password);

            yield return PostJson(url, payload, onCompleted);
        }

        public IEnumerator Refresh(
            string apiBaseUrl,
            string refreshToken,
            Action<BackendApiResult<AuthTokenHttpResponse>> onCompleted)
        {
            var url = BuildApiUrl(apiBaseUrl, "/api/auth/refresh");
            var payload = new RefreshHttpRequest(refreshToken);

            yield return PostJson(url, payload, onCompleted);
        }

        public IEnumerator Logout(
            string apiBaseUrl,
            string refreshToken,
            Action<BackendApiResult<LogoutHttpResponse>> onCompleted)
        {
            var url = BuildApiUrl(apiBaseUrl, "/api/auth/logout");
            var payload = new LogoutHttpRequest(refreshToken);

            yield return PostJson(url, payload, onCompleted);
        }

        private static IEnumerator PostJson<TRequest, TResponse>(
            string url,
            TRequest payload,
            Action<BackendApiResult<TResponse>> onCompleted)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                onCompleted?.Invoke(BackendApiResult<TResponse>.Fail(0, "API URL is empty."));
                yield break;
            }

            var json = JsonUtility.ToJson(payload);
            var body = Encoding.UTF8.GetBytes(json);
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(body),
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = REQUEST_TIMEOUT_SECONDS
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            UnityWebRequestAsyncOperation operation;

            try
            {
                operation = request.SendWebRequest();
            }
            catch (Exception exception)
            {
                request.Dispose();
                onCompleted?.Invoke(BackendApiResult<TResponse>.Fail(0, exception.Message));
                yield break;
            }

            yield return operation;

            var responseText = request.downloadHandler.text;
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<TResponse>(responseText);
                onCompleted?.Invoke(BackendApiResult<TResponse>.Ok(request.responseCode, response));
                request.Dispose();
                yield break;
            }

            onCompleted?.Invoke(BackendApiResult<TResponse>.Fail(
                request.responseCode,
                BuildErrorMessage(request, responseText)));
            request.Dispose();
        }

        private static string BuildErrorMessage(UnityWebRequest request, string responseText)
        {
            if (!string.IsNullOrWhiteSpace(responseText))
            {
                var errorResponse = JsonUtility.FromJson<ApiErrorResponse>(responseText);
                if (errorResponse != null && !string.IsNullOrWhiteSpace(errorResponse.message))
                {
                    return errorResponse.message;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.error))
            {
                return request.error;
            }

            return $"Request failed with response code {request.responseCode}.";
        }

        private static string BuildApiUrl(string apiBaseUrl, string path)
        {
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                return string.Empty;
            }

            return $"{apiBaseUrl.TrimEnd('/')}{path}";
        }
    }
}
