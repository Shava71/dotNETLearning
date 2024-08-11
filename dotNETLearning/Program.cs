using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

var valueStorage = new ValueStorage();

builder.Services.AddSingleton<IGenerator>(valueStorage);
builder.Services.AddSingleton<IReader>(valueStorage);
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

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

app.UseMiddleware<GeneratorMiddleware>();
app.UseMiddleware<ReaderMiddleware>();
//app.Run(async context =>
//{
  
//});
app.Run();

interface IGenerator
{
    int GenerateValue();
}

interface IReader
{
    int ReadValue();
}

class ValueStorage : IGenerator, IReader
{
    int Value;

    public int GenerateValue()
    {
        Value = new Random().Next();
        return Value;
    }

    public int ReadValue() => Value;
}

class GeneratorMiddleware(RequestDelegate next, IGenerator generator)
{
    private RequestDelegate next = next;
    private IGenerator generator = generator;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/generator")
        {
            await context.Response.WriteAsync($"Generated value = {generator?.GenerateValue()}");
        }
        else
        {
            next.Invoke(context);
        }
    }
}

class ReaderMiddleware(RequestDelegate next, IReader reader)
{
    private RequestDelegate next = next;
    private IReader reader = reader;

    public async Task InvokeAsync(HttpContext context)
    {
        
        await context.Response.WriteAsync($"Current value = {reader?.ReadValue()}");
       
    }
}