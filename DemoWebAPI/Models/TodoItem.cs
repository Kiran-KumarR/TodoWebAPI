using DemoWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }


public static class TodoItemEndpoints
{
	public static void MapTodoItemEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/TodoItem", async (TodoContext db) =>
        {
            return await db.TodoItems.ToListAsync();
        })
        .WithName("GetAllTodoItems")
        .Produces<List<TodoItem>>(StatusCodes.Status200OK);

        routes.MapGet("/api/TodoItem/{id}", async (long Id, TodoContext db) =>
        {
            return await db.TodoItems.FindAsync(Id)
                is TodoItem model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetTodoItemById")
        .Produces<TodoItem>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routes.MapPut("/api/TodoItem/{id}", async (long Id, TodoItem todoItem, TodoContext db) =>
        {
            var foundModel = await db.TodoItems.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }

            db.Update(todoItem);

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateTodoItem")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        routes.MapPost("/api/TodoItem/", async (TodoItem todoItem, TodoContext db) =>
        {
            db.TodoItems.Add(todoItem);
            await db.SaveChangesAsync();
            return Results.Created($"/TodoItems/{todoItem.Id}", todoItem);
            //return Created(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        })
        .WithName("CreateTodoItem")
        .Produces<TodoItem>(StatusCodes.Status201Created);


        routes.MapDelete("/api/TodoItem/{id}", async (long Id, TodoContext db) =>
        {
            if (await db.TodoItems.FindAsync(Id) is TodoItem todoItem)
            {
                db.TodoItems.Remove(todoItem);
                await db.SaveChangesAsync();
                return Results.Ok(todoItem);
            }

            return Results.NotFound();
        })
        .WithName("DeleteTodoItem")
        .Produces<TodoItem>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}}
