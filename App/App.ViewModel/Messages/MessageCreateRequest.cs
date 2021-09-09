using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.ViewModel.Messages
{
    public class MessageCreateRequest
    { 
        [Required]
        public string Content { get; set; } 

        [Required]
        public Guid ChatId { get; set; } 
    }
}
