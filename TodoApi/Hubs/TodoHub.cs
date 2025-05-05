using Microsoft.AspNetCore.SignalR;

namespace TodoApi.Hubs
{
    public class TodoHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task NotifyTodoCompletionUpdate(
            long listId,
            string[] justCompletedIds,
            int completedCount,
            int totalCount
        )
        {
            await Clients
                .Group(listId.ToString())
                .SendAsync(
                    "ReceiveTodoCompletionUpdate",
                    listId,
                    justCompletedIds,
                    completedCount,
                    totalCount
                );
        }

        public async Task JoinListGroup(string listId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, listId);
        }

        public async Task LeaveListGroup(string listId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, listId);
        }
    }
}
