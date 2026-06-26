using AmberTower.Auth.Contracts;
using ApiGateway.Auth.Requests;
using ApiGateway.Auth.Responses;
using Grpc.Core;

namespace ApiGateway.Auth;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .Accepts<RegisterHttpRequest>("application/json")
            .Produces<RegisterHttpResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .Accepts<LoginHttpRequest>("application/json")
            .Produces<LoginHttpResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        group.MapPost("/refresh", RefreshAsync)
            .WithName("Refresh")
            .Accepts<RefreshHttpRequest>("application/json")
            .Produces<RefreshHttpResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .Accepts<LogoutHttpRequest>("application/json")
            .Produces<LogoutHttpResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        return group;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterHttpRequest request,
        AuthGrpc.AuthGrpcClient authClient,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new ApiErrorResponse("validation_error", "Email and password are required."));
        }

        try
        {
            var response = await authClient.RegisterAsync(new RegisterRequest
            {
                Email = request.Email,
                Password = request.Password
            }, cancellationToken: cancellationToken);

            if (response.Success)
            {
                return Results.Ok(new RegisterHttpResponse(response.UserId, response.Email));
            }

            return response.ErrorCode switch
            {
                "duplicate_email" => Results.Conflict(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage)),
                "validation_error" => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage)),
                _ => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage))
            };
        }
        catch (RpcException exception)
        {
            return Results.Json(
                new ApiErrorResponse("auth_service_unavailable", $"Auth service call failed: {exception.Status.Detail}"),
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    private static async Task<IResult> LoginAsync(
        LoginHttpRequest request,
        AuthGrpc.AuthGrpcClient authClient,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new ApiErrorResponse("validation_error", "Email and password are required."));
        }

        try
        {
            var response = await authClient.LoginAsync(new LoginRequest
            {
                Email = request.Email,
                Password = request.Password
            }, cancellationToken: cancellationToken);

            if (response.Success)
            {
                return Results.Ok(new LoginHttpResponse(
                    response.UserId,
                    response.Email,
                    response.AccessToken,
                    response.AccessTokenExpiresAtUnixSeconds,
                    response.RefreshToken,
                    response.RefreshTokenExpiresAtUnixSeconds));
            }

            return response.ErrorCode switch
            {
                "invalid_credentials" => Results.Json(
                    new ApiErrorResponse(response.ErrorCode, response.ErrorMessage),
                    statusCode: StatusCodes.Status401Unauthorized),
                "validation_error" => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage)),
                _ => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage))
            };
        }
        catch (RpcException exception)
        {
            return Results.Json(
                new ApiErrorResponse("auth_service_unavailable", $"Auth service call failed: {exception.Status.Detail}"),
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    private static async Task<IResult> RefreshAsync(
        RefreshHttpRequest request,
        AuthGrpc.AuthGrpcClient authClient,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Results.BadRequest(new ApiErrorResponse("validation_error", "Refresh token is required."));
        }

        try
        {
            var response = await authClient.RefreshAsync(new RefreshRequest
            {
                RefreshToken = request.RefreshToken
            }, cancellationToken: cancellationToken);

            if (response.Success)
            {
                return Results.Ok(new RefreshHttpResponse(
                    response.UserId,
                    response.Email,
                    response.AccessToken,
                    response.AccessTokenExpiresAtUnixSeconds,
                    response.RefreshToken,
                    response.RefreshTokenExpiresAtUnixSeconds));
            }

            return response.ErrorCode switch
            {
                "invalid_refresh_token" => Results.Json(
                    new ApiErrorResponse(response.ErrorCode, response.ErrorMessage),
                    statusCode: StatusCodes.Status401Unauthorized),
                "validation_error" => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage)),
                _ => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage))
            };
        }
        catch (RpcException exception)
        {
            return Results.Json(
                new ApiErrorResponse("auth_service_unavailable", $"Auth service call failed: {exception.Status.Detail}"),
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    private static async Task<IResult> LogoutAsync(
        LogoutHttpRequest request,
        AuthGrpc.AuthGrpcClient authClient,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Results.BadRequest(new ApiErrorResponse("validation_error", "Refresh token is required."));
        }

        try
        {
            var response = await authClient.LogoutAsync(new LogoutRequest
            {
                RefreshToken = request.RefreshToken
            }, cancellationToken: cancellationToken);

            if (response.Success)
            {
                return Results.Ok(new LogoutHttpResponse(true));
            }

            return response.ErrorCode switch
            {
                "validation_error" => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage)),
                _ => Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage))
            };
        }
        catch (RpcException exception)
        {
            return Results.Json(
                new ApiErrorResponse("auth_service_unavailable", $"Auth service call failed: {exception.Status.Detail}"),
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
