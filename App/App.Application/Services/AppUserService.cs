using App.Application.Common;
using App.Application.Interfaces;
using App.Data.EF;
using App.Data.Entities;
using App.Data.Enums;
using App.Utilities.Common;
using App.Utilities.Exceptions;
using App.Utilities.Helpers;
using App.ViewModel.AppUsers;
using App.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services
{
    public class AppUserService : BaseService, IAppUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtHelper _jwtHelper;

        public AppUserService(ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            IFileStorageService fileStorageService, 
            IJwtHelper jwtHelper,
            IHttpContextAccessor httpContextAccessor) : base(fileStorageService, httpContextAccessor, userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _fileStorageService = fileStorageService;
            _httpContextAccessor = httpContextAccessor;
            _jwtHelper = jwtHelper;
        }

        public async Task<ApiResult<List<AppUserViewModel>>> GetAppUserPrivate()
        {
            var user = await base.GetCurrentUserAsync();

            if (user == null)
                return null;

            var userChats = await _context.UserChats
                .Include(x => x.Chat)
                    .ThenInclude(x => x.UserChats)
                        .ThenInclude(x => x.AppUser)
                .Where(x => x.AppUserId == user.Id)
                .ToListAsync(); // danh sách các UserChat
            var chats = userChats
                .Where(x => x.Chat.ChatType == ChatType.Private)
                .Select(x => x.Chat)
                .ToList();  // Danh sách các private chat của user đo
            var exceptUserChats = chats
                .Select(x => x.UserChats
                    .FirstOrDefault(u => u.AppUserId != user.Id))
                .ToList(); // Danh sách các user đã của nhóm chat với user hiện tại

            var exceptUsers = exceptUserChats
                .Select(x => x.AppUser)
                .ToList();
            
            var users = await _userManager
                .Users
                .Where(x => x.Id != user.Id)
                .ToListAsync(); // danh sách tất cả các User

            var resultUsers = users.Except(exceptUsers).ToList();
            var result = resultUsers.Select(x => AsAppUserViewModel(x));

            return new ApiResult<List<AppUserViewModel>>(true, "", result.ToList());
        }

        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>AccessToken</returns>
        public async Task<ApiResult<string>> Authenticate(UserLoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);

                if (user == null)
                {
                    return ApiResultFactory.NoData(false, "Tài khoản không tồn tại!");
                }

                var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

                if (!result.Succeeded)
                {
                    return ApiResultFactory.NoData(false, "Mật khẩu không đúng!");
                }

                var userToken = new UserToken()
                {
                    Id = user.Id,
                    Name = user.Name
                };

                return new ApiResult<string>(true, "", await _jwtHelper.UserToToken(userToken));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi " + ex.Message);
                throw new AppInternalServerException(ex);
            }
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="request"></param>
        /// <returns>AccessToken</returns>
        public async Task<ApiResult<string>> Register(UserRegisterRequest request)
        {
            var avatarPath = "";
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);

                if (user != null)
                {
                    return ApiResultFactory.NoData(false, "Tài khoản đã tồn tại!");
                }

                avatarPath = await _fileStorageService.SaveFileAsync(request.Avatar, SystemConstants.FileUploadPath.UserAvatars);
                user = new AppUser()
                {
                    Name = request.Name,
                    Email = request.Email,
                    AvatarPath = avatarPath,
                    UserName = request.Username
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    await base.RollbackFile(avatarPath);
                    return ApiResultFactory.NoData(false, result.Errors.ToString());
                }

                return await this.Authenticate(new UserLoginRequest()
                {
                    Username = request.Username,
                    Password = request.Password
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi " + ex.Message);
                await base.RollbackFile(avatarPath);
                throw new AppInternalServerException(ex);
            }
        }

        private AppUserViewModel AsAppUserViewModel(AppUser user)
        {
            return new AppUserViewModel()
            {
                Id = user.Id,
                Name = user.Name,
                Avatar = user.AvatarPath
            };
        }
    }
}
