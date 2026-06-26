namespace AuthService.Application;

public interface IRefreshTokenService
{
    RefreshTokenIssueResult Issue();

    string Hash(string token);
}
