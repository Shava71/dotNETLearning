using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


var app = builder.Build();



//ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
//ILogger logger = loggerFactory.CreateLogger<Program>();




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

//app.Map("/conf", (ILogger<Program> logger) =>
//{
//    logger.LogInformation($"Path time: {DateTime.Now.ToLongTimeString()}");
    
//});
//app.Run(async (context) =>
//{
//    logger.LogInformation($"Path: {context.Request.Path}");
//    await context.Response.WriteAsync("Hello :3");
//});
app.Map("/conf", (ILoggerFactory LoggerFactory) =>
{
    ILogger logger = LoggerFactory.CreateLogger("MapLogger");
    logger.LogInformation($"Path's time: {DateTime.Now.Kind}");
    return "Hello";
});

app.Run();

