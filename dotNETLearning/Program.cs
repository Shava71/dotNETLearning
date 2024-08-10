using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITimer, Timer>();
builder.Services.AddScoped<TimeService>();
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



//app.Run(async context =>
//{
  
//});
app.UseMiddleware<TimerMiddleware>();
app.Run();

public interface ITimer
{
    string Time { get; }
}

public class Timer : ITimer
{
    public string Time { get; } = DateTime.Now.ToLongTimeString(); 
}

public class TimeService
{
    private ITimer timer;

    public TimeService(ITimer timer)
    {
        this.timer = timer;
    }

    public string GetTime() => timer.Time;
}

public class TimerMiddleware(RequestDelegate next, TimeService timeService)
{
    private RequestDelegate next = next;
    private TimeService timeService = timeService;

    //public async Task InvokeAsync(HttpContext context)
    //{
    //    if (context.Request.Path == "/time")
    //    {
    //        context.Response.ContentType = "text/html; charset=utf-8";
    //        await context.Response.WriteAsync($"Currently time: {timeService?.Time}");
    //    }
    //    else
    //    {
    //        await next.Invoke(context);
    //    }
    //}
    public async Task Invoke(HttpContext context)
    {
       
        await context.Response.WriteAsync($"Currently time: {timeService?.GetTime()}");
        
    }
}
