using UnityEngine;

namespace AmberTower.Client.Infrastructure.Auth
{
    public sealed class PlayerPrefsAuthSessionStore : IAuthSessionStore
    {
        // Minimal local-development storage. Replace with platform secure storage before production use.
        private const string UserIdKey = "AmberTower.Auth.UserId";
        private const string EmailKey = "AmberTower.Auth.Email";
        private const string AccessTokenKey = "AmberTower.Auth.AccessToken";
        private const string AccessTokenExpiresAtKey = "AmberTower.Auth.AccessTokenExpiresAt";
        private const string RefreshTokenKey = "AmberTower.Auth.RefreshToken";
        private const string RefreshTokenExpiresAtKey = "AmberTower.Auth.RefreshTokenExpiresAt";

        public AuthSession Load()
        {
            return new AuthSession
            {
                UserId = PlayerPrefs.GetString(UserIdKey, string.Empty),
                Email = PlayerPrefs.GetString(EmailKey, string.Empty),
                AccessToken = PlayerPrefs.GetString(AccessTokenKey, string.Empty),
                AccessTokenExpiresAtUnixSeconds = ReadLong(AccessTokenExpiresAtKey),
                RefreshToken = PlayerPrefs.GetString(RefreshTokenKey, string.Empty),
                RefreshTokenExpiresAtUnixSeconds = ReadLong(RefreshTokenExpiresAtKey)
            };
        }

        public void Save(AuthSession session)
        {
            PlayerPrefs.SetString(UserIdKey, session.UserId);
            PlayerPrefs.SetString(EmailKey, session.Email);
            PlayerPrefs.SetString(AccessTokenKey, session.AccessToken);
            PlayerPrefs.SetString(AccessTokenExpiresAtKey, session.AccessTokenExpiresAtUnixSeconds.ToString());
            PlayerPrefs.SetString(RefreshTokenKey, session.RefreshToken);
            PlayerPrefs.SetString(RefreshTokenExpiresAtKey, session.RefreshTokenExpiresAtUnixSeconds.ToString());
            PlayerPrefs.Save();
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(UserIdKey);
            PlayerPrefs.DeleteKey(EmailKey);
            PlayerPrefs.DeleteKey(AccessTokenKey);
            PlayerPrefs.DeleteKey(AccessTokenExpiresAtKey);
            PlayerPrefs.DeleteKey(RefreshTokenKey);
            PlayerPrefs.DeleteKey(RefreshTokenExpiresAtKey);
            PlayerPrefs.Save();
        }

        private static long ReadLong(string key)
        {
            var rawValue = PlayerPrefs.GetString(key, "0");
            return long.TryParse(rawValue, out var value) ? value : 0;
        }
    }
}
