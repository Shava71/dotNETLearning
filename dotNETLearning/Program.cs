using dotNETLearning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

var options = new RewriteOptions().AddRedirect("data[/]?$", "data/index")
    .AddRewrite("(?i)data/index", "data/about", skipRemainingRules: false);
app.UseRewriter(options);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Use(async (context, next) =>
{
    app.Logger.LogInformation($"Path: {context.Request.Path} and Time: {DateTime.Now.ToLongTimeString()}");
    await next.Invoke(context);
});

app.Map("/data", (HttpContext context) =>
{
    context.Response.WriteAsync("data page");
});

app.Map("/data/index", (HttpContext context) =>
{
    context.Response.WriteAsync("data-index page");
});
app.MapGet("/data/about", (HttpContext context) =>
{
    context.Response.WriteAsync("real path: " + context.Request.Path);
});





app.Run();

