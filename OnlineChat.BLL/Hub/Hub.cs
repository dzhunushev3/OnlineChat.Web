using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OnlineChat.BLL.Hub
{
    public class Hub : Microsoft.AspNetCore.SignalR.Hub
    {
        public static readonly List<ClaimsPrincipal> OnlineUsers = new List<ClaimsPrincipal>();
        [Authorize]
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            OnlineUsers.Add(user);
            await base.OnConnectedAsync();
            await Clients.All.SendAsync("UpdateOnlineUsers");
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            OnlineUsers.Remove(Context.User);
            await Clients.All.SendAsync("OnlineUsers", OnlineUsers);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
