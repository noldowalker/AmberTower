using AmberTower.Profile.Contracts;
using Grpc.Core;
using ProfileService.Application;

namespace ProfileService.Grpc;

public sealed class ProfileGrpcService : ProfileGrpc.ProfileGrpcBase
{
    private readonly ProfileApplicationService _profileApplicationService;

    public ProfileGrpcService(ProfileApplicationService profileApplicationService)
    {
        _profileApplicationService = profileApplicationService;
    }

    public override async Task<GetMyProfileResponse> GetMyProfile(GetMyProfileRequest request, ServerCallContext context)
    {
        var result = await _profileApplicationService.GetMyProfileAsync(
            request.AuthUserId,
            request.Email,
            context.CancellationToken);

        return new GetMyProfileResponse
        {
            Success = result.IsSuccess,
            PlayerId = result.PlayerId.ToString(),
            AuthUserId = result.AuthUserId.ToString(),
            Email = result.Email,
            Nickname = result.Nickname,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };
    }

    public override async Task<UpdateMyProfileResponse> UpdateMyProfile(UpdateMyProfileRequest request, ServerCallContext context)
    {
        var result = await _profileApplicationService.UpdateMyProfileAsync(
            request.AuthUserId,
            request.Email,
            request.Nickname,
            context.CancellationToken);

        return new UpdateMyProfileResponse
        {
            Success = result.IsSuccess,
            PlayerId = result.PlayerId.ToString(),
            AuthUserId = result.AuthUserId.ToString(),
            Email = result.Email,
            Nickname = result.Nickname,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage
        };
    }
}
