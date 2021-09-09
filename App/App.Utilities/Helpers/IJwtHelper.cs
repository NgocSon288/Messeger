using App.ViewModel.AppUsers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Utilities.Helpers
{
    public interface IJwtHelper
    {
        Task<ClaimsPrincipal> TokenToClaimsPrincipal(string token);
        Task<string> UserToToken(UserToken user);
    }
}