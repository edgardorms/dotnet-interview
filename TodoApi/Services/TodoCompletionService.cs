using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TodoApi.Hubs;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoCompletionService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<TodoHub> _hubContext;

        public TodoCompletionService(
            IServiceProvider serviceProvider,
            IHubContext<TodoHub> hubContext
        )
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        public async Task CompleteAllItemsAsync(long todoListId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TodoContext>();

                var itemsToComplete = await context
                    .TodoItem.Where(item => item.TodoListId == todoListId && !item.Completed)
                    .ToListAsync();

                int totalCount = itemsToComplete.Count;
                int completedCount = 0;

                await _hubContext
                    .Clients.Group(todoListId.ToString())
                    .SendAsync(
                        "ReceiveTodoCompletionUpdate",
                        todoListId,
                        new string[0],
                        completedCount,
                        totalCount
                    );

                foreach (var item in itemsToComplete)
                {
                    item.Completed = true;
                    await Task.Delay(100); //simulate delay
                    await context.SaveChangesAsync();
                    completedCount++;

                    await _hubContext
                        .Clients.Group(todoListId.ToString())
                        .SendAsync(
                            "ReceiveTodoCompletionUpdate",
                            todoListId,
                            new string[] { item.Id.ToString() },
                            completedCount,
                            totalCount
                        );
                }
            }
        }

        public async Task CompleteAllItemsAsyncSignalR(long todoListId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TodoContext>();

                var itemsToComplete = await context
                    .TodoItem.Where(item => item.TodoListId == todoListId && !item.Completed)
                    .ToListAsync();

                int totalCount = itemsToComplete.Count;
                int completedCount = 0;

                await _hubContext
                    .Clients.Group(todoListId.ToString())
                    .SendAsync(
                        "ReceiveTodoCompletionUpdate",
                        todoListId,
                        new string[0],
                        completedCount,
                        totalCount
                    );

                foreach (var item in itemsToComplete)
                {
                    item.Completed = true;
                    await Task.Delay(100);
                    await context.SaveChangesAsync();
                    completedCount++;

                    await _hubContext
                        .Clients.Group(todoListId.ToString())
                        .SendAsync(
                            "ReceiveTodoCompletionUpdate",
                            todoListId,
                            new string[] { item.Id.ToString() },
                            completedCount,
                            totalCount
                        );
                }
            }
        }
    }
}
