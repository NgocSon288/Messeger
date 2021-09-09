using App.Data.Entities;
using App.Integration.Interfaces;
using App.ViewModel.Common;
using App.ViewModel.Messages;
using App.Web.ChatHubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Controllers
{
    [Authorize]
    [Route("Chat")]
    public class ChatController : BaseController
    {
        private readonly IAppUserApiClient _appUserApiClient;
        private readonly IChatApiClient _chatApiClient;
        private readonly IMessageApiClient _messageApiClient;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IAppUserApiClient appUserApiClient,
            IChatApiClient chatApiClient,
            IMessageApiClient messageApiClient,
            IHubContext<ChatHub> hubContext)
        {
            _appUserApiClient = appUserApiClient;
            _chatApiClient = chatApiClient;
            _messageApiClient = messageApiClient;
            _hubContext = hubContext;
        }

        [HttpGet("ket-ban")]
        public async Task<IActionResult> Friend()
        {
            var result = await _appUserApiClient.GetAppUserPrivate();
            var data = result.Data;

            return View(data);
        }

        public async Task<IActionResult> CreatePrivateChat(string userId)
        {
            var result = await _chatApiClient.CreatePrivateChat(userId);
            return RedirectToAction("Friend", "Chat");
        }

        [HttpGet("ban-be")]
        public IActionResult Private()
        {
            return View();
        }

        [HttpGet("GetChatById/{chatId}")]
        public async Task<ApiResult<Chat>> GetChatById([FromRoute] Guid chatId)
        {
            return await _chatApiClient.GetAllPrivateChatById(chatId);
        }

        [HttpPost("CreateMessage")]
        public async Task<ApiResult<Message>> CreateMessage([FromBody]MessageCreateRequest input)
        {  
            if (!ModelState.IsValid)
            {
                return new ApiResult<Message>(false, ModelState.ToString());
            }

            var result = await _messageApiClient.CreateMessage(input);

            if (result.IsSuccessfully)
            {
                await _hubContext.Clients.Group(input.ChatId.ToString()).SendAsync("ReceiveMessage", result.Data);
            }

            return result;
        }
    }
}
