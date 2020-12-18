using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MovieHub.Hubs
{
    [Authorize]
    public class DiscussHub : Hub
    {
        
        private string GetUserName()
        {
            return Context.User?.Identity.Name;
        }
        
        public async Task Send(string message, string group)
        {
            Console.WriteLine(group);
            var userName = GetUserName();
            Console.WriteLine(userName);
            await Clients.Group(group).SendAsync("Send", message, userName);
        }
        public async Task JoinGroup(string group)
        {
            Console.WriteLine("join group");
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await Clients.Group(group).SendAsync("Notify", Context.User.Identity.Name + " joined");
        }

        public async Task LeaveGroup(string group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            await Clients.Group(group).SendAsync("Notify", Context.User.Identity.Name + " left");
        }
    }
}