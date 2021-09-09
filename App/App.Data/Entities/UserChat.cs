using App.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data.Entities
{
    public class UserChat
    {
        public int Id { get; set; }
        public UserChatType UserChatType { get; set; }
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
