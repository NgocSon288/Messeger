using App.Data.Entities;
using App.Integration.Enums;
using App.Integration.Interfaces;
using App.Utilities.Common;
using App.ViewModel.Common;
using App.ViewModel.Messages;
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
    public class MessageApiClient : BaseApiClient, IMessageApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly string BaseUrl = "api/Messages";

        public async Task<ApiResult<Message>> CreateMessage(MessageCreateRequest request)
        {
            var url = $"{BaseUrl}";
            var headers = new Dictionary<string, string>()
            {
                {SystemConstants.AppSettings.Token, "Bearer"}
            };
            var response = await SendRequestBase(url, HttpMethodType.POST, request, HttpContentType.StringContent, headers);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<Message>>(body);
        }

    }

}
