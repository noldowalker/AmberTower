using System;
using System.Collections;
using AmberTower.Client.Infrastructure.Responses;
using UnityEngine;
using UnityEngine.Networking;

namespace AmberTower.Client.Infrastructure
{
    public sealed class BackendApi
    {
        public IEnumerator CheckHealth(string url, Action<HealthCheckResult> onCompleted)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                onCompleted?.Invoke(HealthCheckResult.Fail("URL is empty."));
                yield break;
            }

            var request = UnityWebRequest.Get(url);
            request.timeout = 10;

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
    }
}
