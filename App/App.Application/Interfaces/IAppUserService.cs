using App.Data.Entities;
using App.ViewModel.AppUsers;
using App.ViewModel.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Application.Interfaces
{
    public interface IAppUserService
    {
        Task<ApiResult<string>> Authenticate(UserLoginRequest request); 
        Task<ApiResult<List<AppUserViewModel>>> GetAppUserPrivate();
        Task<ApiResult<string>> Register(UserRegisterRequest request);
    }
}