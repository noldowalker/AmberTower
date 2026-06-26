using ApiGateway.CurrentUser.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway.CurrentUser;

public static class CurrentUserEndpoints
{
    public static RouteGroupBuilder MapCurrentUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api")
            .WithTags("CurrentUser")
            .RequireAuthorization();

        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .Produces<CurrentUserHttpResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        return group;
    }

    private static IResult GetCurrentUser(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var email = user.FindFirstValue(JwtRegisteredClaimNames.Email);

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(email))
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new CurrentUserHttpResponse(userId, email));
    }
}
