using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ToDoApi;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/items",async(ToDoDbContext d)=>
await d.Items.ToListAsync());
app.MapPost("/items",async(Item item,ToDoDbContext d)=>
{d.Items.Add(item);
await d.SaveChangesAsync();
return Results.Created($"/items/{item.Id}",item);
});
app.MapPut("/items/{id}",async(int id,Boolean isComplete,ToDoDbContext d)=>
{var newItem=await d.Items.FindAsync(id);
if(newItem is null)
return Results.NotFound();
newItem.isComplete=isComplete;
await d.SaveChangesAsync();
return Results.NoContent();
});
app.MapDelete("/{id}",async(int id,ToDoDbContext d)=>
{
    var newItem=await d.Items.FindAsync(id);
    if(newItem is null)
    return Results.NotFound();
    d.Items.Remove(newItem);
    await d.SaveChangesAsync();
    return Results.NoContent();
})
app.UseCors("CorsPolicy");
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options=>{
    options.AddPolicy("CorsPolicy",
    builder=>builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
});
builder.Services.AddSwaggerGen();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});


app.Run();
