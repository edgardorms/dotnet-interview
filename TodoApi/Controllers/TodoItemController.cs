using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/todolists/{id:long}/todos")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly IBackgroundJobService _backgroundJobService;

        public TodoItemController(TodoContext context, IBackgroundJobService backgroundJobService)
        {
            _context = context;
            _backgroundJobService = backgroundJobService;
        }

        // GET: api/todolists/{id}/todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems(long id)
        {
            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            var items = await _context.TodoItem
                .Where(item => item.TodoListId == id)
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/todolists/{id}/todos/{itemId}
        [HttpGet("{itemId}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id, long itemId)
        {
            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            var todoItem = await _context.TodoItem
                .Where(i => i.TodoListId == id && i.Id == itemId)
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound("TodoItem not found");
            }

            return Ok(todoItem);
        }

        // POST: api/todolists/{id}/todos
        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodoItem(long id, [FromBody] CreateItem payload)
        {
            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            var todoItem = new TodoItem
            {
                Description = payload.Description,
                Completed = false,
                TodoListId = id
            };

            _context.TodoItem.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = id, itemId = todoItem.Id }, todoItem);
        }

        // PUT: api/todolists/{id}/todos/{itemId}
        [HttpPut("{itemId}")]
        public async Task<ActionResult> UpdateTodoItem(long id, long itemId, [FromBody] UpdateItem payload)
        {
            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            var todoItem = await _context.TodoItem
                .Where(i => i.TodoListId == id && i.Id == itemId)
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound("TodoItem not found");
            }

            todoItem.Description = payload.Description;
            todoItem.Completed = payload.Completed;

            await _context.SaveChangesAsync();

            return Ok(todoItem);
        }

        // DELETE: api/todolists/{id}/todos/{itemId}
        [HttpDelete("{itemId}")]
        public async Task<ActionResult> DeleteTodoItem(long id, long itemId)
        {
            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            var todoItem = await _context.TodoItem
                .Where(i => i.TodoListId == id && i.Id == itemId)
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound("TodoItem not found");
            }

            _context.TodoItem.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/todolists/{id}/todos/complete-all
        [HttpPost("complete-all")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteAllTodoItems(long id)
        {
            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            await _backgroundJobService.EnqueueCompleteAllItemsJob(id);

            return Accepted();
        }

        // POST: api/todolists/{id}/todos/complete-all-signalr
        [HttpPost("complete-all-signalr")] // Nueva ruta
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteAllTodoItemsSignalR(long id)
        {

            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            await _backgroundJobService.EnqueueCompleteAllItemsJobSignalR(id);

            return Accepted();
        }

        // POST: api/todolists/{id}/todos/mockupData
        [HttpPost("mockupData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateMockupData(long id)
        {
            if (!await TodoListExists(id))
            {
                return NotFound("TodoList not found");
            }

            var mockItems = new List<TodoItem>();
            for (int i = 1; i <= 100; i++)
            {
                mockItems.Add(new TodoItem
                {
                    Description = $"Mock Item {i}",
                    Completed = false,
                    TodoListId = id
                });
            }

            await _context.TodoItem.AddRangeAsync(mockItems);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Successfully added 100 mock items to list {id}" });
        }

        private async Task<bool> TodoListExists(long id)
        {
            return await _context.TodoList.AnyAsync(e => e.Id == id);
        }
    }
}