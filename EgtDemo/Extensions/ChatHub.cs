using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EgtDemo.Extensions
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string msg)
        {
            await Clients.All.SendAsync("ReceiveMsg", user, msg);
        }

        /// <summary>
        /// 加入组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMsg", $"{Context.ConnectionId}加入组", $"{groupName}");
        }

        /// <summary>
        /// 离开组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task RemoveFromGroup(string groupName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMsg", $"{Context.ConnectionId}离开组", $"{groupName}");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 组内发送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendMsgToGroup(string groupName,string msg)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMsg", $"{Context.ConnectionId}", $"{msg}");
        }
    }
}
