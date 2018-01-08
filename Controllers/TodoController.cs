using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Firelink.Todos.Persistence;
using Firelink.Todos.Entity;
using Firelink.Todos.Specification;
using NSpecifications;

namespace Firelink.Todos.Controllers
{
    [Route("api/[controller]")]
    public class TodosController : Controller
    {
        private static readonly ISpecificationBuilder<Todo, TodoQueryParams> _specBuilder = new TodoSpecificationBuilder();
        private readonly TodoDbContext _context;
        public TodosController(TodoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TodoQueryParams specParams)
        {
            return Ok(new
            {
                Todos = await _context.Todos.Where(
                    _specBuilder.Build(specParams).Expression
                ).ToListAsync()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var todo = await _context.Todos.Where(TodoSpecifications.ById(id).Expression).FirstOrDefaultAsync();

            if (todo == null)
            {
                return WhenTodoNull(id);
            }

            return Ok(new
            {
                todo
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TodoBag value)
        {
            var todo = Todo.CreateFrom(value);

            _context.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = todo.Id }, todo);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]TodoBag value)
        {
            var todo = await _context.Todos.Where(TodoSpecifications.ById(id).Expression).FirstOrDefaultAsync();

            if (todo == null)
            {
                return WhenTodoNull(id);
            }

            todo.UpdateFrom(value);
            await _context.SaveChangesAsync();
            return AcceptedAtAction("Get", new { id = todo.Id }, todo);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var todo = await _context.Todos.Where(TodoSpecifications.ById(id).Expression).FirstOrDefaultAsync();

            if (todo == null)
            {
                return WhenTodoNull(id);
            }

            todo.MarkAs(TodoStatus.Abandoned);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IActionResult WhenTodoNull(Guid id)
        {
            return NotFound(new
            {
                Message = $"No todo found for {id}"
            });
        }
    }
}
