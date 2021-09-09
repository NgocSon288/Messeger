using App.Integration.Enums;
using App.Integration.Interfaces;
using App.Integration.Models;
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
    public class AppUserApiClient : BaseApiClient, IAppUserApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppUserApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        private readonly string BaseUrl = "api/AppUsers";

        public async Task<ApiResult<List<AppUserViewModel>>> GetAppUserPrivate()
        { 
            var url = $"{BaseUrl}/AppUser";
            var headers = new Dictionary<string, string>()
            {
                {SystemConstants.AppSettings.Token, "Bearer"}
            };
            var response = await SendRequestBase(url, headers);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<List<AppUserViewModel>>>(body);
        }

        /// <summary>
        /// IsAuthenticate
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Access Token</returns>
        public async Task<ApiResult<string>> Authenticate(UserLoginRequest request)
        {
            var url = $"{BaseUrl}/Authenticate";
            var response = await SendRequestBase(url, HttpMethodType.POST, request, HttpContentType.StringContent, null);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<string>>(body);
        }

        /// <summary>
        /// Register Account
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Access Token</returns>
        public async Task<ApiResult<string>> Register(UserRegisterRequest request)
        {
            var url = $"{BaseUrl}/Register";
            var response = await SendRequestBase(url, HttpMethodType.POST, request, HttpContentType.MultipartFormDataContent, null, new ItemImage(ItemImageType.Signle, nameof(UserRegisterRequest.Avatar)));

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<string>>(body);
        }
    }
}
