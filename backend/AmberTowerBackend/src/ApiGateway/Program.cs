using AmberTower.Auth.Contracts;
using ApiGateway.Auth;
using Grpc.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<AuthGrpc.AuthGrpcClient>(options =>
{
    var authServiceUrl = builder.Configuration["Grpc:AuthServiceUrl"] ?? "http://localhost:5081";
    options.Address = new Uri(authServiceUrl);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "ApiGateway",
    message = "AmberTower backend entry point"
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "ApiGateway",
    utcTime = DateTime.UtcNow
}));

app.MapPost("/api/auth/register", RegisterAsync)
    .WithName("Register")
    .WithTags("Auth")
    .Accepts<RegisterHttpRequest>("application/json")
    .Produces<RegisterHttpResponse>(StatusCodes.Status200OK)
    .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
    .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
    .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

app.MapPost("/api/auth/login", LoginAsync)
    .WithName("Login")
    .WithTags("Auth")
    .Accepts<LoginHttpRequest>("application/json")
    .Produces<LoginHttpResponse>(StatusCodes.Status200OK)
    .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
    .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
    .Produces<ApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

app.Run();

static async Task<IResult> RegisterAsync(
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

static async Task<IResult> LoginAsync(
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
                response.ExpiresAtUnixSeconds));
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
