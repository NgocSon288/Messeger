using App.Application.Common;
using App.Data.EF;
using App.Data.Entities;
using App.ViewModel.Common;
using App.ViewModel.Messages;
using App.Utilities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Application.Interfaces;

namespace App.Application.Services
{
    public class MessageService : BaseService, IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageService(ApplicationDbContext context,
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

        public async Task<ApiResult<Message>> CreateMessage(MessageCreateRequest request)
        {
            try
            {
                var user = await base.GetCurrentUserAsync();
                var chat = await _context.Chats
                    .Include(x => x.Messages)
                    .FirstOrDefaultAsync(x => x.Id == request.ChatId);

                if (chat == null)
                {
                    throw new AppNotFoundException("Chat không tồn tại");
                }

                var message = new Message()
                {
                    ChatId = chat.Id,
                    CreatedDate = DateTime.Now,
                    Content = request.Content,
                    Sender = user.Id
                };

                await _context.AddAsync(message);
                await _context.SaveChangesAsync();

                return new ApiResult<Message>(true, "", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new AppInternalServerException(ex);
            }
        }
    }
}
