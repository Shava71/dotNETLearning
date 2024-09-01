using dotNETLearning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var adminRole = new Role("admin");
var userRole = new Role("user");

var people = new List<Person>
{
    new Person("timoshka@mail.ru", "12345", 2004, adminRole, "London", "Microsoft"),
    new Person("tima@mail.ru", "123", 2010, userRole, "Berlin", "Mercedes"),
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IAuthorizationHandler, AgeHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyForMicrosoft", policy => policy.RequireClaim("Company", "Microsoft"));
    options.AddPolicy("OnlyForBerlin", policy => policy.RequireClaim(ClaimTypes.Locality, "Berlin"));
    options.AddPolicy("AgeLimit", policy => policy.AddRequirements(new AgeRequirement(18)));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/accessdenied";
});

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

//app.MapGet("/login/{username}/{phoneNumber}/{company}", async (string username, string phoneNumber, string company, HttpContext context) =>
//{
//    var claims = new List<Claim> {
//        new (ClaimTypes.Name, username),
//        new (ClaimTypes.MobilePhone, phoneNumber),
//        new ("company", company),
//        new ("language", "Russian"),
//        new ("language", "English"),
//        new ("language", "Spanish"),

//    };
//    var claimIdentity = new ClaimsIdentity(claims, "Cookies");
//    var claimPrincipal = new ClaimsPrincipal(claimIdentity);

//    await context.SignInAsync(claimPrincipal);
//    return Results.Redirect("/data");
//});

app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    string loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>METANIT.COM</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <form method='post'>
            <p>
                <label>Email</label><br />
                <input name='email' />
            </p>
            <p>
                <label>Password</label><br />
                <input type='password' name='password' />
            </p>
            <input type='submit' value='Login' />
        </form>
    </body>
    </html>";
    await context.Response.WriteAsync(loginForm);
});

app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    returnUrl = null;

    var form = context.Request.Form;
    if (!form.ContainsKey("email") || !form.ContainsKey("password"))
        return Results.BadRequest("Email и/или пароль не введены");

    var email = form["email"];
    var password = form["password"];

    Person? person = people.FirstOrDefault(p => p.email == email && p.password == password);

    if (person is null)
        return Results.Unauthorized();

    var claims = new List<Claim> { 
    new Claim(ClaimsIdentity.DefaultNameClaimType, person.email),
    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.role.Name),
    new Claim(ClaimTypes.DateOfBirth, person.year.ToString()),
    new Claim(ClaimTypes.Locality, person.City),
    new Claim("Company", person.Company)
    };

    var claimIdentity = new ClaimsIdentity(claims, "Cookie");
    var claimPrincipal = new ClaimsPrincipal(claimIdentity);

    await context.SignInAsync(claimPrincipal);
    return Results.Redirect(returnUrl ?? "/data");    
});

app.Map("/admin", [Authorize(Roles = "admin")] () => "Admin panel");

app.MapGet("/accessdenied",  (HttpContext context) =>
{
    context.Response.StatusCode = 403;
    return Results.Text("Access denied");

});

//app.MapGet("/addage/{age}", async (HttpContext context, string age) =>
//{
//    if (context.User.Identity is ClaimsIdentity claimsIdentity)
//    {
//        claimsIdentity.AddClaims(new[] { new Claim("age", age) });

//        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
//        context.SignInAsync(claimsPrincipal);
//    }
//    return Results.Redirect("/data");
//});

//app.MapGet("/removephone", async (HttpContext context) =>
//{
//    if (context.User.Identity is ClaimsIdentity claimsIdentity)
//    {
//        var phoneNumber = claimsIdentity.FindFirst(ClaimTypes.MobilePhone);
//        if (claimsIdentity.TryRemoveClaim(phoneNumber))
//        {
//            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
//            await context.SignInAsync(claimPrincipal);
//        }
//    }
//    return Results.Redirect("/data");
//});


//app.Map("/data", (HttpContext context) =>
//{
//    var user = context.User.Identity;
//    var userName = context.User.FindFirst(ClaimTypes.Name);
//    var phoneNumber = context.User.FindFirst(ClaimTypes.MobilePhone);
//    var company = context.User.FindFirst("company");
//    var age = context.User.FindFirst("age");
//    var languages = context.User.FindAll("language");
//    var stringLanguage = string.Empty;
//    foreach (var language in languages)
//    {
//        stringLanguage = $"{stringLanguage} {language.Value}";
//    }
//    if (user.IsAuthenticated)
//        return $"Username: {userName?.Value} Phone: {phoneNumber?.Value} Company: {company?.Value} Age: {age} \nLanguages: {stringLanguage}";
//    else return "???????????? ?? ?????????????????";
//});

app.Map("/data", [Authorize(Roles = "admin, user")](HttpContext context) =>
{
    var Username = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    var Userrole = context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
    var City = context.User.FindFirst(ClaimTypes.Locality);
    var Company = context.User.FindFirst("Company");
    var Year = context.User.FindFirst( ClaimTypes.DateOfBirth);
    if (int.TryParse(Year?.Value, out var year) )
    {
        var age = DateTime.Now.Year - year;
        
        return Results.Text(
            $"Name: {Username?.Value}   Role: {Userrole?.Value}   Age: {age}   " +
            $"City: {City?.Value}   Company: {Company?.Value} ");
    }
    else return Results.BadRequest("Bad Year value");
    // return Results.Text(
    // $"Name: {Username?.Value}   Role: {Userrole?.Value}   Year: {Year?.Value}   " +
    // $"City: {City?.Value}   Company: {Company?.Value} ");
    
});

app.Map("/Berlin", [Authorize(Policy = "OnlyForBerlin")] () => " U r living in Berlin");

app.Map("/Microsoft", [Authorize(Policy = "OnlyForMicrosoft")] () => " U r working in Microsoft");

app.Map("/Age", [Authorize(Policy = "AgeLimit")] () => "Age limit is passed");

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Данные удалены";
});

app.Run();

