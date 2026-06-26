using AmberTower.Auth.Contracts;
using AuthService.Application;
using Grpc.Core;

namespace AuthService.Grpc;

public sealed class AuthGrpcService : AuthGrpc.AuthGrpcBase
{
    private readonly AuthApplicationService _authApplicationService;

    public AuthGrpcService(AuthApplicationService authApplicationService)
    {
        _authApplicationService = authApplicationService;
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await _authApplicationService.RegisterAsync(request.Email, request.Password, context.CancellationToken);

        return new RegisterResponse
        {
            Success = result.IsSuccess,
            UserId = result.UserId.ToString(),
            Email = result.Email,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await _authApplicationService.LoginAsync(request.Email, request.Password, context.CancellationToken);

        return new LoginResponse
        {
            Success = result.IsSuccess,
            UserId = result.UserId.ToString(),
            Email = result.Email,
            AccessToken = result.AccessToken,
            AccessTokenExpiresAtUnixSeconds = result.IsSuccess
                ? new DateTimeOffset(result.AccessTokenExpiresAtUtc).ToUnixTimeSeconds()
                : 0,
            RefreshToken = result.RefreshToken,
            RefreshTokenExpiresAtUnixSeconds = result.IsSuccess
                ? new DateTimeOffset(result.RefreshTokenExpiresAtUtc).ToUnixTimeSeconds()
                : 0,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };
    }

    public override async Task<RefreshResponse> Refresh(RefreshRequest request, ServerCallContext context)
    {
        var result = await _authApplicationService.RefreshAsync(request.RefreshToken, context.CancellationToken);

        return new RefreshResponse
        {
            Success = result.IsSuccess,
            UserId = result.UserId.ToString(),
            Email = result.Email,
            AccessToken = result.AccessToken,
            AccessTokenExpiresAtUnixSeconds = result.IsSuccess
                ? new DateTimeOffset(result.AccessTokenExpiresAtUtc).ToUnixTimeSeconds()
                : 0,
            RefreshToken = result.RefreshToken,
            RefreshTokenExpiresAtUnixSeconds = result.IsSuccess
                ? new DateTimeOffset(result.RefreshTokenExpiresAtUtc).ToUnixTimeSeconds()
                : 0,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };
    }

    public override async Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
    {
        var result = await _authApplicationService.LogoutAsync(request.RefreshToken, context.CancellationToken);

        return new LogoutResponse
        {
            Success = result.IsSuccess,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };
    }
}
