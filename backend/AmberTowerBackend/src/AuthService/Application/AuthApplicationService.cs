using AuthService.Domain;

namespace AuthService.Application;

public sealed class AuthApplicationService
{
    private const int MinimumPasswordLength = 6;

    private readonly IAuthUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthApplicationService(
        IAuthUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHashService passwordHashService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHashService = passwordHashService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<RegisterUserResult> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(password) || password.Length < MinimumPasswordLength)
        {
            return new RegisterUserResult
            {
                ErrorCode = AuthErrorCodes.ValidationError,
                ErrorMessage = $"Email and password are required. Password must be at least {MinimumPasswordLength} characters long."
            };
        }

        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            return new RegisterUserResult
            {
                ErrorCode = AuthErrorCodes.DuplicateEmail,
                ErrorMessage = "A user with this email already exists."
            };
        }

        var user = new AuthUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHashService.HashPassword(user, password);

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterUserResult
        {
            IsSuccess = true,
            UserId = user.Id,
            Email = user.Email
        };
    }

    public async Task<LoginUserResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(password))
        {
            return new LoginUserResult
            {
                ErrorCode = AuthErrorCodes.ValidationError,
                ErrorMessage = "Email and password are required."
            };
        }

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !_passwordHashService.VerifyPassword(user, password))
        {
            return new LoginUserResult
            {
                ErrorCode = AuthErrorCodes.InvalidCredentials,
                ErrorMessage = "Invalid email or password."
            };
        }

        var accessToken = _tokenService.Issue(user);
        var refreshToken = _refreshTokenService.Issue();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshToken.TokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = refreshToken.ExpiresAtUtc
        }, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new LoginUserResult
        {
            IsSuccess = true,
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken.Token,
            AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc
        };
    }

    public async Task<RefreshSessionResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new RefreshSessionResult
            {
                ErrorCode = AuthErrorCodes.ValidationError,
                ErrorMessage = "Refresh token is required."
            };
        }

        var storedRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(
            _refreshTokenService.Hash(refreshToken),
            cancellationToken);

        if (storedRefreshToken is null || !IsRefreshTokenActive(storedRefreshToken))
        {
            return InvalidRefreshTokenResult();
        }

        var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId, cancellationToken);
        if (user is null)
        {
            return InvalidRefreshTokenResult();
        }

        var newAccessToken = _tokenService.Issue(user);
        var newRefreshToken = _refreshTokenService.Issue();
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newRefreshToken.TokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = newRefreshToken.ExpiresAtUtc
        };

        storedRefreshToken.RevokedAtUtc = DateTime.UtcNow;
        storedRefreshToken.ReplacedByTokenId = newRefreshTokenEntity.Id;

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new RefreshSessionResult
        {
            IsSuccess = true,
            UserId = user.Id,
            Email = user.Email,
            AccessToken = newAccessToken.Token,
            AccessTokenExpiresAtUtc = newAccessToken.ExpiresAtUtc,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAtUtc = newRefreshToken.ExpiresAtUtc
        };
    }

    public async Task<LogoutSessionResult> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new LogoutSessionResult
            {
                ErrorCode = AuthErrorCodes.ValidationError,
                ErrorMessage = "Refresh token is required."
            };
        }

        var storedRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(
            _refreshTokenService.Hash(refreshToken),
            cancellationToken);

        if (storedRefreshToken is null || !IsRefreshTokenActive(storedRefreshToken))
        {
            return new LogoutSessionResult
            {
                ErrorCode = AuthErrorCodes.InvalidRefreshToken,
                ErrorMessage = "Refresh token is invalid or expired."
            };
        }

        storedRefreshToken.RevokedAtUtc = DateTime.UtcNow;
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new LogoutSessionResult
        {
            IsSuccess = true
        };
    }

    private static bool IsRefreshTokenActive(RefreshToken refreshToken)
    {
        return refreshToken.RevokedAtUtc is null
            && refreshToken.ExpiresAtUtc > DateTime.UtcNow;
    }

    private static RefreshSessionResult InvalidRefreshTokenResult()
    {
        return new RefreshSessionResult
        {
            ErrorCode = AuthErrorCodes.InvalidRefreshToken,
            ErrorMessage = "Refresh token is invalid or expired."
        };
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
