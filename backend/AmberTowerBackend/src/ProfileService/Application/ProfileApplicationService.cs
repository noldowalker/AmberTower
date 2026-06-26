using ProfileService.Domain;

namespace ProfileService.Application;

public sealed class ProfileApplicationService
{
    private const int MaxNicknameLength = 32;

    private readonly IPlayerProfileRepository _profileRepository;

    public ProfileApplicationService(IPlayerProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ProfileResult> GetMyProfileAsync(string authUserId, string email, CancellationToken cancellationToken)
    {
        if (!TryParseAuthUser(authUserId, email, out var parsedAuthUserId, out var normalizedEmail, out var errorResult))
        {
            return errorResult;
        }

        var profile = await GetOrCreateProfileAsync(parsedAuthUserId, normalizedEmail, cancellationToken);
        return ToResult(profile);
    }

    public async Task<ProfileResult> UpdateMyProfileAsync(
        string authUserId,
        string email,
        string nickname,
        CancellationToken cancellationToken)
    {
        if (!TryParseAuthUser(authUserId, email, out var parsedAuthUserId, out var normalizedEmail, out var errorResult))
        {
            return errorResult;
        }

        var normalizedNickname = NormalizeNickname(nickname);
        if (string.IsNullOrWhiteSpace(normalizedNickname) || normalizedNickname.Length > MaxNicknameLength)
        {
            return new ProfileResult
            {
                ErrorCode = ProfileErrorCodes.ValidationError,
                ErrorMessage = $"Nickname is required and must be at most {MaxNicknameLength} characters long."
            };
        }

        var profile = await GetOrCreateProfileAsync(parsedAuthUserId, normalizedEmail, cancellationToken);
        profile.Nickname = normalizedNickname;
        profile.UpdatedAtUtc = DateTime.UtcNow;

        await _profileRepository.SaveChangesAsync(cancellationToken);

        return ToResult(profile);
    }

    private async Task<PlayerProfile> GetOrCreateProfileAsync(
        Guid authUserId,
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        var existingProfile = await _profileRepository.GetByAuthUserIdAsync(authUserId, cancellationToken);
        if (existingProfile is not null)
        {
            return existingProfile;
        }

        var now = DateTime.UtcNow;
        var profile = new PlayerProfile
        {
            PlayerId = Guid.NewGuid(),
            AuthUserId = authUserId,
            Email = normalizedEmail,
            Nickname = BuildDefaultNickname(normalizedEmail),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _profileRepository.AddAsync(profile, cancellationToken);
        await _profileRepository.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private static ProfileResult ToResult(PlayerProfile profile)
    {
        return new ProfileResult
        {
            IsSuccess = true,
            PlayerId = profile.PlayerId,
            AuthUserId = profile.AuthUserId,
            Email = profile.Email,
            Nickname = profile.Nickname
        };
    }

    private static bool TryParseAuthUser(
        string authUserId,
        string email,
        out Guid parsedAuthUserId,
        out string normalizedEmail,
        out ProfileResult errorResult)
    {
        normalizedEmail = NormalizeEmail(email);
        if (!Guid.TryParse(authUserId, out parsedAuthUserId) || string.IsNullOrWhiteSpace(normalizedEmail))
        {
            errorResult = new ProfileResult
            {
                ErrorCode = ProfileErrorCodes.ValidationError,
                ErrorMessage = "Authenticated user id and email are required."
            };
            return false;
        }

        errorResult = new ProfileResult();
        return true;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static string NormalizeNickname(string nickname)
    {
        return nickname.Trim();
    }

    public static string BuildDefaultNickname(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        var separatorIndex = normalizedEmail.IndexOf('@');
        if (separatorIndex <= 0)
        {
            return normalizedEmail;
        }

        return normalizedEmail[..separatorIndex];
    }
}
