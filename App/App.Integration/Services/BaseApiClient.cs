using App.Integration.Enums;
using App.Integration.Models;
using App.Utilities.Common;
using App.Utilities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration; 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.Integration.Services
{
    public class BaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        protected async Task<HttpResponseMessage> SendRequestBase<T>(string url, HttpMethodType method, T data, HttpContentType contentType, Dictionary<string, string> headers, ItemImage itemImage = null)
        {
            try
            {
                if (contentType == HttpContentType.MultipartFormDataContent && itemImage == null)
                {
                    throw new AppInternalServerException("Cần khai báo ItemImage!");
                }

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddressServer]);

                if (headers != null && headers.Count > 0)
                {
                    foreach (var (keySession, headerName) in headers)
                    {
                        var sessions = _httpContextAccessor.HttpContext.Session.GetString(keySession);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(headerName, sessions);
                    }
                }

                var response = new HttpResponseMessage();
                var content = ConvertObjectToHttpContent(data, contentType, itemImage);

                switch (method)
                {
                    case HttpMethodType.GET:
                        response = await client.GetAsync(url);
                        break;

                    case HttpMethodType.POST:
                        response = await client.PostAsync(url, content);
                        break;

                    case HttpMethodType.PUT:
                        response = await client.PutAsync(url, content);
                        break;

                    case HttpMethodType.PATCH:
                        response = await client.PatchAsync(url, content);
                        break;

                    default:
                        response = await client.DeleteAsync(url);
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new AppInternalServerException(ex);
            }
        }

        protected async Task<HttpResponseMessage> SendRequestBase(string url, Dictionary<string, string> headers = null)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddressServer]);

            if (headers != null && headers.Count > 0)
            {
                foreach (var (keySession, headerName) in headers)
                {
                    var sessions = _httpContextAccessor.HttpContext.Session.GetString(keySession);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(headerName, sessions);
                }
            }

            return await client.GetAsync(url); ;
        }

        public async Task Delete(Guid id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/users/{id}");
            var body = await response.Content.ReadAsStringAsync();

        }


        #region Convert to HttpContent

        private HttpContent ConvertObjectToHttpContent<T>(T data, HttpContentType contentType, ItemImage itemImage)
        {
            switch (contentType)
            {
                case HttpContentType.StringContent:
                    return ConvertToStringContent(data);

                default:
                    return ConvertToMultipartFormDataContent(data, itemImage);
            }
        }

        private MultipartFormDataContent ConvertToMultipartFormDataContent<T>(T data, ItemImage itemImage)
        {
            var content = new MultipartFormDataContent();
            var type = data.GetType();
            var propertiesInfo = type.GetProperties();

            var imgFileValue = type.GetProperty(itemImage.PropertyNameLayer1).GetValue(data);    // List or noList

            if (!string.IsNullOrEmpty(itemImage.PropertyNameLayer2))
            {
                var type2 = imgFileValue.GetType();
                imgFileValue = type2.GetProperty(itemImage.PropertyNameLayer2).GetValue(imgFileValue);
            }

            if (imgFileValue != null)
            {
                switch (itemImage.ItemImageType)
                {
                    case ItemImageType.Multiple:
                        List<IFormFile> formFiles = imgFileValue as List<IFormFile>;
                        break;
                    default:
                        IFormFile formFile = imgFileValue as IFormFile;
                        byte[] dataBytes;
                        using (var br = new BinaryReader(formFile.OpenReadStream()))
                        {
                            dataBytes = br.ReadBytes((int)formFile.OpenReadStream().Length);
                        }
                        ByteArrayContent bytes = new ByteArrayContent(dataBytes);
                        content.Add(bytes, itemImage.PropertyNameLayer1, formFile.FileName);
                        break;
                }
            }
            foreach (var item in propertiesInfo)
            {
                if (item.Name == itemImage.PropertyNameLayer1)
                {
                    continue;
                }

                var value = item.GetValue(data);
                content.Add(new StringContent(value?.ToString() ?? ""), item.Name);
            }

            return content;
        }

        private StringContent ConvertToStringContent<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        #endregion Convert to HttpContent
    }
}
