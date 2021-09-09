using App.Data.Entities;
using App.Integration.Enums;
using App.Integration.Interfaces;
using App.Utilities.Common;
using App.ViewModel.AppUsers;
using App.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.Integration.Services
{
    public class ChatApiClient : BaseApiClient, IChatApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly string BaseUrl = "api/Chats";

        public async Task<ApiResult<Chat>> CreatePrivateChat(string userId)
        {
            var url = $"{BaseUrl}/Private";
            var headers = new Dictionary<string, string>()
            {
                {SystemConstants.AppSettings.Token, "Bearer"}
            };
            var response = await SendRequestBase(url, HttpMethodType.POST, userId, HttpContentType.StringContent, headers);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<Chat>>(body);
        }

        public async Task<ApiResult<List<Chat>>> GetAllPrivateChat()
        {
            var url = $"{BaseUrl}/Private";
            var headers = new Dictionary<string, string>()
            {
                {SystemConstants.AppSettings.Token, "Bearer"}
            };
            var response = await SendRequestBase(url, headers);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<List<Chat>>>(body);
        }

        public async Task<ApiResult<Chat>> GetAllPrivateChatById(Guid chatId)
        {
            var url = $"{BaseUrl}/Private/{chatId}"; 
            var headers = new Dictionary<string, string>()
            {
                {SystemConstants.AppSettings.Token, "Bearer"}
            };
            var response = await SendRequestBase(url, headers);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<Chat>>(body);
        }
    }
}
