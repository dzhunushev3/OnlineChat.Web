using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OnlineChat.Models.Entities
{
    public class Chat : BaseEntity
    {
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsPublic { get; set; }
        [NotMapped]
        public List<ApplicationUser> UserList { get; set; }
        public bool IsGroup { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
        public bool IsJoin { get; set; }
        public Chat()
        {
            UserList = new List<ApplicationUser>();
        }
    }
}
