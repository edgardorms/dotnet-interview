using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Tests;

#nullable disable
public class TodoItemControllerTests
{
    private readonly IBackgroundJobService _backgroundJobService; // Inject the service

    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
        context.TodoItem.Add(
            new TodoItem
            {
                Id = 1,
                Description = "Task 1",
                TodoListId = 1,
                Completed = false,
            }
        );
        context.TodoItem.Add(
            new TodoItem
            {
                Id = 2,
                Description = "Task 2",
                TodoListId = 1,
                Completed = true,
            }
        );
        context.TodoItem.Add(
            new TodoItem
            {
                Id = 3,
                Description = "Task 3",
                TodoListId = 2,
                Completed = false,
            }
        );
        context.TodoList.Add(new TodoList { Id = 1, Name = "Task 1" });
        context.TodoList.Add(new TodoList { Id = 2, Name = "Task 2" });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetTodoItem_WhenCalled_ReturnsTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.GetTodoItems(1);

            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(2, ((result.Result as OkObjectResult).Value as IList<TodoItem>).Count);
        }
    }

    [Fact]
    public async Task GetTodoItem_WhenCalled_ReturnsTodoItemById()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.GetTodoItem(1, 1);

            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(1, ((result.Result as OkObjectResult).Value as TodoItem).Id);
            Assert.Equal(
                "Task 1",
                ((result.Result as OkObjectResult).Value as TodoItem).Description
            );
        }
    }

    [Fact]
    public async Task GetTodoItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.GetTodoItem(1, 999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }

    [Fact]
    public async Task GetTodoItem_WhenListDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.GetTodoItem(999, 1);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }

    [Fact]
    public async Task PutTodoItem_WhenCalled_UpdatesTheTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.UpdateTodoItem(
                1,
                1,
                new Dtos.UpdateItem { Description = "Changed Task 2", Completed = true }
            );

            Assert.IsType<OkObjectResult>(result);
            Assert.True(((result as OkObjectResult).Value as TodoItem).Completed);
        }
    }

    [Fact]
    public async Task PostTodoItem_WhenCalled_CreatesTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.CreateTodoItem(
                2,
                new Dtos.CreateItem { Description = "Task 4", Completed = false }
            );
            var listTodo = await controller.GetTodoItems(2);

            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(2, ((listTodo.Result as OkObjectResult).Value as IList<TodoItem>).Count);
        }
    }

    [Fact]
    public async Task DeleteTodoItem_WhenCalled_RemovesTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.DeleteTodoItem(1, 1);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(1, context.TodoItem.Count(i => i.TodoListId == 1));
        }
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.DeleteTodoItem(1, 999);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }

    [Fact]
    public async Task DeleteTodoItem_WhenListDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemController(context, _backgroundJobService);

            var result = await controller.DeleteTodoItem(999, 1);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
