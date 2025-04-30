namespace TodoApi.Dtos;

public class UpdateItem
{
    public required string Description { get; set; }
    public required bool Completed { get; set; }
}
