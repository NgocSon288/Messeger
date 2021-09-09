using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
