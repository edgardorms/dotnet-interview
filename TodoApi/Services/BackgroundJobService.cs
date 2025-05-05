namespace TodoApi.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IServiceProvider _serviceProvider;

        public BackgroundJobService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task EnqueueCompleteAllItemsJob(long todoListId)
        {
            _ = Task.Run(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var completionService = scope.ServiceProvider.GetRequiredService<TodoCompletionService>();
                    await completionService.CompleteAllItemsAsync(todoListId);
                }
            });

            return Task.CompletedTask;
        }

        public Task EnqueueCompleteAllItemsJobSignalR(long todoListId)
        {
            _ = Task.Run(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var completionService = scope.ServiceProvider.GetRequiredService<TodoCompletionService>();
                    await completionService.CompleteAllItemsAsyncSignalR(todoListId);
                }
            });

            return Task.CompletedTask;
        }
    }
}
