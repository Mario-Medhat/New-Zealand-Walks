using Microsoft.AspNetCore.Identity;

namespace NZWalks.API.Repositories
{
    public interface ITokenRepository
    {
        string CreateTWTToken(IdentityUser identityUser, List<string> roles);
    }
}
