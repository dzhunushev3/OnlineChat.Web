using System;

namespace OnlineChat.Models.Entities
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public Exception Exception { get; set; }
    }
}
