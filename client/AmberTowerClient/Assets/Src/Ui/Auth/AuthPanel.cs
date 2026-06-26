using System;
using System.Collections;
using AmberTower.Client.Infrastructure;
using AmberTower.Client.Infrastructure.Auth;
using AmberTower.Client.Infrastructure.Responses;
using UnityEngine;
using UnityEngine.UIElements;

namespace AmberTower.Client.Ui.Auth
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class AuthPanel : MonoBehaviour
    {
        [SerializeField]
        private string defaultApiBaseUrl = "http://localhost:8080";

        [SerializeField]
        private StyleSheet panelStyleSheet;

        private readonly BackendApi _backendApi = new BackendApi();
        private readonly IAuthSessionStore _sessionStore = new PlayerPrefsAuthSessionStore();

        private Button _changeAccountButton;
        private Button _loginButton;
        private Button _registerButton;
        private Label _resultLabel;
        private Label _sessionEmailLabel;
        private Label _stateLabel;
        private TextField _apiBaseUrlField;
        private TextField _emailField;
        private TextField _passwordField;
        private VisualElement _formContainer;
        private VisualElement _sessionContainer;
        private Coroutine _activeRequest;
        private AuthSession _currentSession;

        private void OnEnable()
        {
            var document = GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            if (panelStyleSheet != null && !root.styleSheets.Contains(panelStyleSheet))
            {
                root.styleSheets.Add(panelStyleSheet);
            }

            _apiBaseUrlField = root.Q<TextField>("auth-api-base-url-field");
            _emailField = root.Q<TextField>("auth-email-field");
            _passwordField = root.Q<TextField>("auth-password-field");
            _loginButton = root.Q<Button>("auth-login-button");
            _registerButton = root.Q<Button>("auth-register-button");
            _changeAccountButton = root.Q<Button>("auth-change-account-button");
            _resultLabel = root.Q<Label>("auth-result-label");
            _sessionEmailLabel = root.Q<Label>("auth-session-email-label");
            _stateLabel = root.Q<Label>("auth-state-label");
            _formContainer = root.Q<VisualElement>("auth-form-container");
            _sessionContainer = root.Q<VisualElement>("auth-session-container");

            if (!HasRequiredElements())
            {
                Debug.LogError("AuthPanel UI elements were not found.");
                enabled = false;
                return;
            }

            _passwordField.isPasswordField = true;
            if (string.IsNullOrWhiteSpace(_apiBaseUrlField.value))
            {
                _apiBaseUrlField.value = defaultApiBaseUrl;
            }

            _loginButton.clicked += OnLoginClicked;
            _registerButton.clicked += OnRegisterClicked;
            _changeAccountButton.clicked += OnChangeAccountClicked;

            RestoreSession();
        }

        private void OnDisable()
        {
            _loginButton.clicked -= OnLoginClicked;
            _registerButton.clicked -= OnRegisterClicked;
            _changeAccountButton.clicked -= OnChangeAccountClicked;

            if (_activeRequest == null)
            {
                return;
            }

            StopCoroutine(_activeRequest);
            _activeRequest = null;
        }

        private void RestoreSession()
        {
            _currentSession = _sessionStore.Load();
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (_currentSession.IsAccessTokenValid(now))
            {
                ShowAuthenticated(_currentSession, "Session restored.");
                return;
            }

            if (_currentSession.IsRefreshTokenValid(now))
            {
                SetLoading("Refreshing session...");
                _activeRequest = StartCoroutine(RunRefresh(_currentSession.RefreshToken));
                return;
            }

            _sessionStore.Clear();
            ShowAuthForm("Enter credentials.");
        }

        private void OnLoginClicked()
        {
            if (!CanStartRequest())
            {
                return;
            }

            SetLoading("Logging in...");
            _activeRequest = StartCoroutine(RunLogin());
        }

        private void OnRegisterClicked()
        {
            if (!CanStartRequest())
            {
                return;
            }

            SetLoading("Registering...");
            _activeRequest = StartCoroutine(RunRegister());
        }

        private void OnChangeAccountClicked()
        {
            if (!CanStartRequest())
            {
                return;
            }

            if (_currentSession == null || !_currentSession.HasRefreshToken)
            {
                ClearSessionAndShowForm("Session cleared.");
                return;
            }

            SetLoading("Logging out...");
            _activeRequest = StartCoroutine(RunLogout(_currentSession.RefreshToken));
        }

        private IEnumerator RunRegister()
        {
            BackendApiResult<RegisterHttpResponse> result = null;

            yield return _backendApi.Register(
                _apiBaseUrlField.value,
                _emailField.value,
                _passwordField.value,
                response => result = response);

            _activeRequest = null;

            if (result == null)
            {
                ShowAuthForm("No response.");
                yield break;
            }

            if (!result.IsSuccess)
            {
                ShowAuthForm(result.Message);
                yield break;
            }

            SetLoading("Registration complete. Logging in...");
            _activeRequest = StartCoroutine(RunLogin());
        }

        private IEnumerator RunLogin()
        {
            BackendApiResult<AuthTokenHttpResponse> result = null;

            yield return _backendApi.Login(
                _apiBaseUrlField.value,
                _emailField.value,
                _passwordField.value,
                response => result = response);

            _activeRequest = null;
            HandleTokenResponse(result, "Login successful.");
        }

        private IEnumerator RunRefresh(string refreshToken)
        {
            BackendApiResult<AuthTokenHttpResponse> result = null;

            yield return _backendApi.Refresh(
                _apiBaseUrlField.value,
                refreshToken,
                response => result = response);

            _activeRequest = null;
            if (HandleTokenResponse(result, "Session refreshed."))
            {
                yield break;
            }

            _sessionStore.Clear();
        }

        private IEnumerator RunLogout(string refreshToken)
        {
            BackendApiResult<LogoutHttpResponse> result = null;

            yield return _backendApi.Logout(
                _apiBaseUrlField.value,
                refreshToken,
                response => result = response);

            _activeRequest = null;
            ClearSessionAndShowForm(result != null && !result.IsSuccess ? result.Message : "Session cleared.");
        }

        private bool HandleTokenResponse(BackendApiResult<AuthTokenHttpResponse> result, string successMessage)
        {
            if (result == null)
            {
                ShowAuthForm("No response.");
                return false;
            }

            if (!result.IsSuccess || result.Response == null)
            {
                ShowAuthForm(result.Message);
                return false;
            }

            _currentSession = ToSession(result.Response);
            _sessionStore.Save(_currentSession);
            ShowAuthenticated(_currentSession, successMessage);
            return true;
        }

        private void ClearSessionAndShowForm(string message)
        {
            _sessionStore.Clear();
            _currentSession = null;
            ShowAuthForm(message);
        }

        private void ShowAuthForm(string message)
        {
            SetControlsEnabled(true);
            _formContainer.style.display = DisplayStyle.Flex;
            _sessionContainer.style.display = DisplayStyle.None;
            _stateLabel.text = "Signed out";
            _resultLabel.text = message;
        }

        private void ShowAuthenticated(AuthSession session, string message)
        {
            SetControlsEnabled(true);
            _formContainer.style.display = DisplayStyle.None;
            _sessionContainer.style.display = DisplayStyle.Flex;
            _stateLabel.text = "Authenticated";
            _sessionEmailLabel.text = session.Email;
            _resultLabel.text = message;
        }

        private void SetLoading(string message)
        {
            SetControlsEnabled(false);
            _stateLabel.text = "Working";
            _resultLabel.text = message;
        }

        private void SetControlsEnabled(bool isEnabled)
        {
            _loginButton.SetEnabled(isEnabled);
            _registerButton.SetEnabled(isEnabled);
            _changeAccountButton.SetEnabled(isEnabled);
            _apiBaseUrlField.SetEnabled(isEnabled);
            _emailField.SetEnabled(isEnabled);
            _passwordField.SetEnabled(isEnabled);
        }

        private bool CanStartRequest()
        {
            if (_activeRequest == null)
            {
                return true;
            }

            _resultLabel.text = "Request already in progress.";
            return false;
        }

        private bool HasRequiredElements()
        {
            return _apiBaseUrlField != null
                && _emailField != null
                && _passwordField != null
                && _loginButton != null
                && _registerButton != null
                && _changeAccountButton != null
                && _resultLabel != null
                && _sessionEmailLabel != null
                && _stateLabel != null
                && _formContainer != null
                && _sessionContainer != null;
        }

        private static AuthSession ToSession(AuthTokenHttpResponse response)
        {
            return new AuthSession
            {
                UserId = response.userId,
                Email = response.email,
                AccessToken = response.accessToken,
                AccessTokenExpiresAtUnixSeconds = response.accessTokenExpiresAtUnixSeconds,
                RefreshToken = response.refreshToken,
                RefreshTokenExpiresAtUnixSeconds = response.refreshTokenExpiresAtUnixSeconds
            };
        }
    }
}
