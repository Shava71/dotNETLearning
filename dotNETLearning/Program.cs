using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using dotNETLearning;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));

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

app.Use(async (context, next) =>
{
    app.Logger.LogInformation($"Path: {context.Request.Path} and Time: {DateTime.Now.Date}");
    await next.Invoke(context);
});

app.Use(async (context, next) =>
{
    context.Items.Add("message", "Hello world");
    await next.Invoke(context);

});

app.Run(async (context) =>
{
    if(context.Items.ContainsKey("messages"))
    {
        await context.Response.WriteAsync("Hello is real"); 
        app.Logger.LogInformation($"Message: {context.Items["message"]}");
    }
    else
    {
        await context.Response.WriteAsync("no hello :(");
    }
});

app.Run();

