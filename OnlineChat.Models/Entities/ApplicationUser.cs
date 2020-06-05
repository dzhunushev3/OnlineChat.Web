using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace OnlineChat.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string AvatarPath { get; set; }
    }
}
