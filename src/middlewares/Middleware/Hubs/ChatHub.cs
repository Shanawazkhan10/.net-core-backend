using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Middleware.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string fromuser, string Org_Id, string User_Id, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", fromuser, Org_Id, User_Id, message);
        }
    }
}
