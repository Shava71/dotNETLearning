public class CounterMiddleware(RequestDelegate next)
{
    private RequestDelegate next = next;
    int i = 0;

    public async Task InvokeAsync(HttpContext context, ICounter counter, CounterService counterService)
    {
        i++;
        context.Response.ContentType = "text/html;charset=utf-8";
        await context.Response.WriteAsync($"Запрос {i}; Counter: {counter.Value}; Service: {counterService.Counter.Value}");
    }
}