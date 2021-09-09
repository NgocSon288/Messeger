using App.Integration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Components
{
    public class FriendContact : ViewComponent
    { 
        private readonly IChatApiClient _chatApiClient;

        public FriendContact(IChatApiClient chatApiClient)
        {
            _chatApiClient = chatApiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _chatApiClient.GetAllPrivateChat();
            return View(result.Data);
        }
    }
}
