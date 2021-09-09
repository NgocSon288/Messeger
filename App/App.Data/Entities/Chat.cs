using App.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public ChatType ChatType { get; set; }
        public List<Message> Messages { get; set; }
        public List<UserChat> UserChats { get; set; }
    }
}
