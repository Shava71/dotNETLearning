using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

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


app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<RoutingMiddleware>();

app.Run();

public class RoutingMiddleware(RequestDelegate _)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;

        if (path == "/index")
        {
            await context.Response.WriteAsync("Home page");
        }
        else if (path == "/about")
        {
            await context.Response.WriteAsync("About page");
        }
        else
        {
            context.Response.StatusCode = 404;
        }
    }
}

public class AuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Query["token"];

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 403;
        }
        else
        {
            await next.Invoke(context);
        }
    }
}

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next.Invoke(context);
        if (context.Response.StatusCode == 403)
        {
            await context.Response.WriteAsync("Access Denied");
        }
        else if (context.Response.StatusCode == 404)
        {
            await context.Response.WriteAsync("Not Found");
        }
    }
}