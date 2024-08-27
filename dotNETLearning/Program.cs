using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using dotNETLearning;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

List<Person> persons = new List<Person>()
{
    new() { Id = Guid.NewGuid().ToString(), Name = "Tom", Age = 24},
    new() { Id = Guid.NewGuid().ToString(), Name = "Tomik", Age = 24},
    new() { Id = Guid.NewGuid().ToString(), Name = "Tomas", Age = 53}
};

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Use(async (context, next) =>
{
    app.Logger.LogInformation($"Path: {context.Request.Path} and Time: {DateTime.Now.ToLongTimeString()}");
    await next.Invoke(context);
});

app.MapGet("/users", () => persons);
app.MapGet("/users/{id}", (string id) =>
{
    Person? person = persons.FirstOrDefault(x => x.Id == id);
    if (person == null) return Results.NotFound(new{message = "User's not found"});
    return Results.Json(person);
});

app.MapDelete("/users/{id}", (string id) =>
{
    Person? person = persons.FirstOrDefault(x => x.Id == id);
    if (person == null) return Results.NotFound(new { message = "User's not found" });
    persons.Remove(person);
    return Results.Json(person);
});

app.MapPost("/users", (Person person) =>
{
    person.Id = Guid.NewGuid().ToString();
    persons.Add(person);  
    return person;
});

app.MapPut("/users", (Person personData) =>
{
    var person = persons.FirstOrDefault(x => x.Id == personData.Id);
    if (person == null) return Results.NotFound(new { message = "User's not found" });
    person.Name = personData.Name;
    person.Age = personData.Age;
    return Results.Json(person);
});

app.Run();

