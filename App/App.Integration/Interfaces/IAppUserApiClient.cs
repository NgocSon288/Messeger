using App.ViewModel.AppUsers;
using App.ViewModel.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Integration.Interfaces
{
    public interface IAppUserApiClient
    {
        Task<ApiResult<string>> Authenticate(UserLoginRequest request);
        Task<ApiResult<List<AppUserViewModel>>> GetAppUserPrivate();
        Task<ApiResult<string>> Register(UserRegisterRequest request);
    }
}