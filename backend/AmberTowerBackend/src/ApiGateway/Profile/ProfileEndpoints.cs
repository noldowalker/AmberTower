using AmberTower.Profile.Contracts;
using ApiGateway.Auth.Responses;
using ApiGateway.Profile.Requests;
using ApiGateway.Profile.Responses;
using Grpc.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway.Profile;

public static class ProfileEndpoints
{
    public static RouteGroupBuilder MapProfileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/profile")
            .WithTags("Profile")
            .RequireAuthorization();

        group.MapGet("/me", GetMyProfileAsync)
            .WithName("GetMyProfile")
            .Produces<ProfileHttpResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        group.MapPatch("/me", UpdateMyProfileAsync)
            .WithName("UpdateMyProfile")
            .Accepts<UpdateMyProfileHttpRequest>("application/json")
            .Produces<ProfileHttpResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        return group;
    }

    private static async Task<IResult> GetMyProfileAsync(
        ClaimsPrincipal user,
        ProfileGrpc.ProfileGrpcClient profileClient,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(user, out var authUserId, out var email))
        {
            return Results.Unauthorized();
        }

        try
        {
            var response = await profileClient.GetMyProfileAsync(new GetMyProfileRequest
            {
                AuthUserId = authUserId,
                Email = email
            }, cancellationToken: cancellationToken);

            if (response.Success)
            {
                return Results.Ok(ToResponse(response.PlayerId, response.AuthUserId, response.Email, response.Nickname));
            }

            return Results.BadRequest(new ApiErrorResponse(response.ErrorCode, response.ErrorMessage));
        }
        catch (RpcException exception)
        {
            return Results.Json(
                new ApiErrorResponse("profile_service_unavailable", $"Profile service call failed: {exception.Status.Detail}"),
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    private static async Task<IResult> UpdateMyProfileAsync(
        UpdateMyProfileHttpRequest request,
        ClaimsPrincipal user,
        ProfileGrpc.ProfileGrpcClient profileClient,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(user, out var authUserId, out var email))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Nickname))
        {
            return Results.BadRequest(new ApiErrorResponse("validation_error", "Nickname is required."));
        }

        try
        {
            var response = await profileClient.UpdateMyProfileAsync(new UpdateMyProfileRequest
            {
                AuthUserId = authUserId,
                Email = email,
                Nickname = request.Nickname
            }, cancellationToken: cancellationToken);

            if (response.Success)
            {
                return Results.Ok(ToResponse(response.PlayerId, response.AuthUserId, response.Email, response.Nickname));
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
                new ApiErrorResponse("profile_service_unavailable", $"Profile service call failed: {exception.Status.Detail}"),
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    private static bool TryGetCurrentUser(ClaimsPrincipal user, out string authUserId, out string email)
    {
        authUserId = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
        email = user.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty;

        return !string.IsNullOrWhiteSpace(authUserId) && !string.IsNullOrWhiteSpace(email);
    }

    private static ProfileHttpResponse ToResponse(string playerId, string authUserId, string email, string nickname)
    {
        return new ProfileHttpResponse(playerId, authUserId, email, nickname);
    }
}
