using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<PizzaDb>(
    options => options.UseInMemoryDatabase("pizzas")
);


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "PizzaStore API",
        Description = "Making the Pizzas you love",
        Version = "v1" });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI(c =>
 {
 c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
 });
}


app.MapGet("/", () => "Hello World!");

// GET all pizzaa
app.MapGet("/pizzas", async (PizzaDb db) => await
db.Pizzas.ToListAsync()
);

// POST create new pizza
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

// GET single pizza
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) =>
    await db.Pizzas.FindAsync(id));

// PUT update
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatePizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    pizza.Name = updatePizza.Name;
    pizza.Description = updatePizza.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE
app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();
