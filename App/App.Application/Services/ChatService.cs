using App.Application.Common;
using App.Application.Interfaces;
using App.Data.EF;
using App.Data.Entities;
using App.Data.Enums;
using App.Utilities.Exceptions;
using App.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services
{
    public class ChatService : BaseService, IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatService(ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            IFileStorageService fileStorageService,
            IHttpContextAccessor httpContextAccessor) : base(fileStorageService, httpContextAccessor, userManager)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _fileStorageService = fileStorageService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Tạo phòng chat với bạn bè
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ApiResult<Chat>> CreatePrivateChat(string userId)
        {
            try
            {
                var user = await base.GetCurrentUserAsync();

                var chat = new Chat()
                {
                    ChatType = ChatType.Private,
                    Messages = new List<Message>(),
                    GroupName = "",
                    UserChats = new List<UserChat>()
                    {
                        new UserChat()
                        {
                            AppUserId = user.Id,
                            UserChatType = UserChatType.Friend
                        },
                        new UserChat()
                        {
                            AppUserId = userId,
                            UserChatType = UserChatType.Friend
                        }
                    }
                };

                await _context.AddAsync(chat);
                var result = await _context.SaveChangesAsync();

                return new ApiResult<Chat>(true, "", chat);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new AppInternalServerException(ex);
            }
        }

        /// <summary>
        /// Danh sách các Private Chat của User hiện tại
        /// </summary>
        /// <returns>List<Chat></returns>
        public async Task<ApiResult<List<Chat>>> GetAllPrivateChat()
        {
            try
            {
                var user = await base.GetCurrentUserAsync();

                var chats = await _context.UserChats
                    .Include(x => x.Chat)
                        .ThenInclude(x => x.UserChats)
                            .ThenInclude(x => x.AppUser)
                    .Where(x => x.AppUserId == user.Id)
                    .Select(x => x.Chat)
                    .ToListAsync();

                return new ApiResult<List<Chat>>(true, "", chats);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new AppInternalServerException(ex);
            }
        }

        public async Task<ApiResult<Chat>> GetChatById(Guid chatId)
        {
            try
            {
                var user = await base.GetCurrentUserAsync(); 
                var chats = await _context.Chats
                    .Include(x => x.Messages)
                    .Include(x => x.UserChats)
                        .ThenInclude(x => x.AppUser)
                    .Where(x => x.Id == chatId)
                    .ToListAsync();
                 
                if(chats.Count <=0)
                {
                    return new ApiResult<Chat>(true, "");
                }
                
                if (!chats[0].UserChats.Any(x => x.AppUserId == user.Id))
                {
                    throw new BadRequestException("Mã chat không hợp lệ!");
                }

                return new ApiResult<Chat>(true, "", chats[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new AppInternalServerException(ex);
            }
        }
    }
}
