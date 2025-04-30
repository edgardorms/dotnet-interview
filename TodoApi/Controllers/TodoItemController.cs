using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todolists/{id:long}/todos")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemController(TodoContext context)
        {
            _context = context;
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

        private async Task<bool> TodoListExists(long id)
        {
            return await _context.TodoList.AnyAsync(e => e.Id == id);
        }
    }
}