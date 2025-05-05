namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public long TodoListId { get; set; }
        public required string Description { get; set; }
        public bool Completed { get; set; } = false;
    }
}
