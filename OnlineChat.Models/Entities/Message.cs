using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OnlineChat.Models.Entities
{
    public class Message : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Messag { get; set; }
        public Chat Chat { get; set; }
        public string ChatId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
