using System.Collections;
using AmberTower.Client.Infrastructure;
using UnityEngine;
using UnityEngine.UIElements;

namespace AmberTower.Client.Ui.HealthCheck
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class HealthCheckPanel : MonoBehaviour
    {
        private const string StatusIdleClass = "status-indicator--idle";
        private const string StatusLoadingClass = "status-indicator--loading";
        private const string StatusSuccessClass = "status-indicator--success";
        private const string StatusErrorClass = "status-indicator--error";

        [SerializeField]
        private string defaultHealthUrl = "http://localhost:8080/health";

        [SerializeField]
        private StyleSheet panelStyleSheet;

        private readonly BackendApi backendApi = new BackendApi();

        private Button checkButton;
        private Label resultLabel;
        private TextField urlField;
        private VisualElement statusIndicator;
        private Coroutine activeRequest;

        private void OnEnable()
        {
            var document = GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            if (panelStyleSheet != null && !root.styleSheets.Contains(panelStyleSheet))
            {
                root.styleSheets.Add(panelStyleSheet);
            }

            urlField = root.Q<TextField>("health-url-field");
            checkButton = root.Q<Button>("health-check-button");
            statusIndicator = root.Q<VisualElement>("health-status-indicator");
            resultLabel = root.Q<Label>("health-result-label");

            if (urlField == null || checkButton == null || statusIndicator == null || resultLabel == null)
            {
                Debug.LogError("HealthCheckPanel UI elements were not found.");
                enabled = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(urlField.value))
            {
                urlField.value = defaultHealthUrl;
            }
            checkButton.clicked += OnCheckClicked;

            SetIdleState("Ready.");
        }

        private void OnDisable()
        {
            checkButton.clicked -= OnCheckClicked;

            if (activeRequest != null)
            {
                StopCoroutine(activeRequest);
                activeRequest = null;
            }
        }

        private void OnCheckClicked()
        {
            if (activeRequest != null)
            {
                StopCoroutine(activeRequest);
            }

            checkButton.SetEnabled(false);
            SetStatus(StatusLoadingClass, "Checking...");

            activeRequest = StartCoroutine(RunHealthCheck(urlField.value));
        }

        private IEnumerator RunHealthCheck(string url)
        {
            HealthCheckResult result = null;

            yield return backendApi.CheckHealth(url, response => result = response);

            checkButton.SetEnabled(true);
            activeRequest = null;

            if (result == null)
            {
                SetStatus(StatusErrorClass, "No response.");
                yield break;
            }

            if (result.IsSuccess)
            {
                SetStatus(StatusSuccessClass, result.Message);
                yield break;
            }

            SetStatus(StatusErrorClass, result.Message);
        }

        private void SetIdleState(string message)
        {
            SetStatus(StatusIdleClass, message);
        }

        private void SetStatus(string statusClass, string message)
        {
            statusIndicator.RemoveFromClassList(StatusIdleClass);
            statusIndicator.RemoveFromClassList(StatusLoadingClass);
            statusIndicator.RemoveFromClassList(StatusSuccessClass);
            statusIndicator.RemoveFromClassList(StatusErrorClass);
            statusIndicator.AddToClassList(statusClass);

            resultLabel.text = message;
        }
    }
}
