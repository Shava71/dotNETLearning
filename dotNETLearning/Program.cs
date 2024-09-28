using dotNETLearning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var people = new List<Person>
{
    new Person("timoshka@mail.ru", "12345"),
    new Person("tima@mail.ru", "123")
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => options.LoginPath = "/login");


builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

//app.UseDefaultFiles();
//app.UseStaticFiles();

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

app.MapGet("/login/{username}/{phoneNumber}/{company}", async (string username, string phoneNumber, string company, HttpContext context) =>
{
    var claims = new List<Claim> {
        new (ClaimTypes.Name, username),
        new (ClaimTypes.MobilePhone, phoneNumber),
        new ("company", company),
        new ("language", "Russian"),
        new ("language", "English"),
        new ("language", "Spanish"),

    };
    var claimIdentity = new ClaimsIdentity(claims, "Cookies");
    var claimPrincipal = new ClaimsPrincipal(claimIdentity);

    await context.SignInAsync(claimPrincipal);
    return Results.Redirect("/data");
});

app.MapGet("/addage/{age}", async (HttpContext context, string age) =>
{
    if (context.User.Identity is ClaimsIdentity claimsIdentity)
    {
        claimsIdentity.AddClaims(new[] { new Claim("age", age) });
        
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        context.SignInAsync(claimsPrincipal);
    }
    return Results.Redirect("/data");
});

app.MapGet("/removephone", async (HttpContext context) =>
{
    if(context.User.Identity is ClaimsIdentity claimsIdentity)
    {
        var phoneNumber = claimsIdentity.FindFirst(ClaimTypes.MobilePhone);
        if (claimsIdentity.TryRemoveClaim(phoneNumber))
        {
            var claimPrincipal = new ClaimsPrincipal (claimsIdentity);
            await context.SignInAsync(claimPrincipal);
        }
    }
    return Results.Redirect("/data");
});


app.Map("/data", (HttpContext context) =>
{
    var user = context.User.Identity;
    var userName = context.User.FindFirst(ClaimTypes.Name);
    var phoneNumber = context.User.FindFirst(ClaimTypes.MobilePhone);
    var company = context.User.FindFirst("company");
    var age = context.User.FindFirst("age");
    var languages = context.User.FindAll("language");
    var stringLanguage = "";
    foreach (var language in languages)
    {
        stringLanguage = $"{stringLanguage} {language.Value}";
    }
    if (user.IsAuthenticated)
        return $"Username: {userName?.Value} Phone: {phoneNumber?.Value} Company: {company?.Value} Age: {age} \nLanguages: {stringLanguage}";
    else return "������������ �� �����������������";
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "������ �������";
});

app.Run();
