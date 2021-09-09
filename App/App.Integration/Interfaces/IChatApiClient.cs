using App.Data.Entities;
using App.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Integration.Interfaces
{
    public interface IChatApiClient
    {
        Task<ApiResult<Chat>> CreatePrivateChat(string userId);
        Task<ApiResult<List<Chat>>> GetAllPrivateChat();
        Task<ApiResult<Chat>> GetAllPrivateChatById(Guid chatId);
    }
}