using AuthService.Domain;

namespace AuthService.Application;

public interface ITokenService
{
    AccessTokenResult Issue(AuthUser user);
}
