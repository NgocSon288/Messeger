using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string AvatarPath { get; set; }
        public List<UserChat> UserChats { get; set; }
    }
}
