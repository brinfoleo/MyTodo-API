using MeuTodo.Data;
using MeuTodo.Models;
using MeuTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MeuTodo.Controllers
{
    [ApiController]
    [Route(template: "v1")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route(template: "todos")]
        public async Task<IActionResult> GetAsync(
            [FromServices] AppDbContext context)
        {
            var todos = await context //AppDbContext
                .Todos //DbSet<Todo>
                .AsNoTracking() //IQueryable<Todo>
                .ToListAsync();//Task <list
            return Ok(todos);

        }
        [HttpGet]
        [Route(template: "todos/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context //AppDbContext
                .Todos //DbSet<Todo>
                .AsNoTracking() //IQueryable<Todo>
                .FirstOrDefaultAsync(x => x.Id == id);
            return todo == null ? NotFound() : Ok(todo);

        }
        [HttpPost(template: "todos")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = new Todo
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };
            try
            {
                await context.Todos.AddAsync(todo);//Memoria
                await context.SaveChangesAsync();//Database
                return Created(uri: $"v1/todos/{todo.Id}", todo);

            }
            catch (Exception e)

            {
                Console.WriteLine(e);
                return BadRequest();
            }

        }


        [HttpPut(template: "todos/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = await context.
                Todos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (todo == null)
                return NotFound();


            try
            {
                todo.Title = model.Title;

                context.Todos.Update(todo);//Nao existe Update async

                await context.SaveChangesAsync();//save in Database

                return Ok(todo);

            }
            catch (Exception e)

            {
                Console.WriteLine(e);
                return BadRequest();
            }

        }
        [HttpDelete(template: "todos/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();//save in Database

                return Ok(todo);

            }
            catch (Exception e)

            {
                Console.WriteLine(e);
                return BadRequest();
            }

        }
    }
}
