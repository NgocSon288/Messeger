using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.ChatHubs
{
    public class ChatHub : Hub
    {  
        public async Task JoinPrivateGroup(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            await Clients.Caller.SendAsync("ReceiveJoinPrivateGroup", chatId);
        }
    }
}
