using System.Collections;
using AmberTower.Client.Infrastructure;
using AmberTower.Client.Infrastructure.Responses;
using UnityEngine;
using UnityEngine.UIElements;

namespace AmberTower.Client.Ui.HealthCheck
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class HealthCheckPanel : MonoBehaviour
    {
        private const string STATUS_IDLE_CLASS = "status-indicator--idle";
        private const string STATUS_LOADING_CLASS = "status-indicator--loading";
        private const string STATUS_SUCCESS_CLASS = "status-indicator--success";
        private const string STATUS_ERROR_CLASS = "status-indicator--error";

        [SerializeField]
        private string defaultHealthUrl = "http://localhost:8080/health";

        [SerializeField]
        private StyleSheet panelStyleSheet;

        private readonly BackendApi _backendApi = new BackendApi();

        private Button _checkButton;
        private Label _resultLabel;
        private TextField _urlField;
        private VisualElement _statusIndicator;
        private Coroutine _activeRequest;

        private void OnEnable()
        {
            var document = GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            if (panelStyleSheet != null && !root.styleSheets.Contains(panelStyleSheet))
            {
                root.styleSheets.Add(panelStyleSheet);
            }

            _urlField = root.Q<TextField>("health-url-field");
            _checkButton = root.Q<Button>("health-check-button");
            _statusIndicator = root.Q<VisualElement>("health-status-indicator");
            _resultLabel = root.Q<Label>("health-result-label");

            if (_urlField == null || _checkButton == null || _statusIndicator == null || _resultLabel == null)
            {
                Debug.LogError("HealthCheckPanel UI elements were not found.");
                enabled = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(_urlField.value))
            {
                _urlField.value = defaultHealthUrl;
            }
            _checkButton.clicked += OnCheckClicked;

            SetIdleState("Ready.");
        }

        private void OnDisable()
        {
            _checkButton.clicked -= OnCheckClicked;

            if (_activeRequest == null) 
                return;
            
            StopCoroutine(_activeRequest);
            _activeRequest = null;
        }

        private void OnCheckClicked()
        {
            if (_activeRequest != null)
            {
                StopCoroutine(_activeRequest);
            }

            _checkButton.SetEnabled(false);
            SetStatus(STATUS_LOADING_CLASS, "Checking...");

            _activeRequest = StartCoroutine(RunHealthCheck(_urlField.value));
        }

        private IEnumerator RunHealthCheck(string url)
        {
            HealthCheckResult result = null;

            yield return _backendApi.CheckHealth(url, response => result = response);

            _checkButton.SetEnabled(true);
            _activeRequest = null;

            if (result == null)
            {
                SetStatus(STATUS_ERROR_CLASS, "No response.");
                yield break;
            }

            if (result.IsSuccess)
            {
                SetStatus(STATUS_SUCCESS_CLASS, result.Message);
                yield break;
            }

            SetStatus(STATUS_ERROR_CLASS, result.Message);
        }

        private void SetIdleState(string message)
        {
            SetStatus(STATUS_IDLE_CLASS, message);
        }

        private void SetStatus(string statusClass, string message)
        {
            _statusIndicator.RemoveFromClassList(STATUS_IDLE_CLASS);
            _statusIndicator.RemoveFromClassList(STATUS_LOADING_CLASS);
            _statusIndicator.RemoveFromClassList(STATUS_SUCCESS_CLASS);
            _statusIndicator.RemoveFromClassList(STATUS_ERROR_CLASS);
            _statusIndicator.AddToClassList(statusClass);

            _resultLabel.text = message;
        }
    }
}
