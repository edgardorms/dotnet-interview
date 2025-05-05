namespace TodoApi.Services
{
    public interface IBackgroundJobService
    {
        Task EnqueueCompleteAllItemsJob(long todoListId);
        Task EnqueueCompleteAllItemsJobSignalR(long todoListId);
    }
}
