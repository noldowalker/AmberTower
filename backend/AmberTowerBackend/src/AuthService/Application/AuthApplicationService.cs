using AuthService.Domain;

namespace AuthService.Application;

public sealed class AuthApplicationService
{
    private const int MinimumPasswordLength = 6;

    private readonly IAuthUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ITokenService _tokenService;

    public AuthApplicationService(
        IAuthUserRepository userRepository,
        IPasswordHashService passwordHashService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _tokenService = tokenService;
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

        var token = _tokenService.Issue(user);

        return new LoginUserResult
        {
            IsSuccess = true,
            UserId = user.Id,
            Email = user.Email,
            AccessToken = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc
        };
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
