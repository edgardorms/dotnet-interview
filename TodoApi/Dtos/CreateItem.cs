namespace TodoApi.Dtos;

public class CreateItem
{
    public required string Description { get; set; }
    public bool Completed { get; set; } = false;

}
