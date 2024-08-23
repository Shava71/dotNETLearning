using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
//{
//    {"name", "Tom"},
//    {"age", "26"}
//});


// Add services to the container.
builder.Services.AddRazorPages();

builder.Configuration.AddJsonFile("config.json");
builder.Services.Configure<Person>(builder.Configuration);

var app = builder.Build();

app.UseMiddleware< PersonMiddleware>();

//builder.Configuration.AddJsonFile("project.json");

//var Tom = new Person();
//builder.Configuration.Bind(Tom);
//builder.Configuration.AddXmlFile("config2.xml");
//builder.Configuration.AddTextFile("config3.txt");

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


//app.Run(async (context) =>
//{
//    string name = app.Configuration["name"];
//    string age = app.Configuration["age"];

//    await context.Response.WriteAsync($"{name}  ---  {age}");

//});
//app.Map("/Conf", (IConfiguration appConfig)=>GetSectionContent(appConfig.GetSection("projectConfig")));
//app.Map("/conf", (IConfiguration appConfig)=> $"{appConfig["name"]} {appConfig["age"]}");
//app.Run(async (context)=>context.Response.WriteAsync($"{Tom.Name + Tom.Age}"));


//app.Run(async (context) =>
//{
//    context.Response.ContentType = "text/html; charset=utf-8";
//    string name = $"<p>Name: {Tomas.Name}</p>";
//    string age = $"<p>Age: {Tomas.Age}</p>";
//    string company = $"<p>Name: {Tomas.company?.Title}</p>";
//    string langs = "<p>Languages:</p><ul>";
//    foreach (var lang in Tomas.languages)
//    {
//        langs += $"<li><p>{lang}</p></li>";
//    }

//    langs += "</ul>";
//    await context.Response.WriteAsync($"{name}{age}{company}{langs}");
//});

app.Map("/conf", (IOptions<Person> options) =>
{
    Person person = options.Value;
    return person;
});

app.Run();

//string GetSectionContent(IConfigurationSection configSection)
//{
//    System.Text.StringBuilder contentBuilder = new();
//    foreach (var section in configSection.GetChildren())
//    {
//        contentBuilder.Append($"\"{section.Key}\":");
//        if (section.Value == null)
//        {
//            string subSectionContent = GetSectionContent(section);
//            contentBuilder.Append($"{{\n{subSectionContent}}},\n");
//        }
//        else
//        {
//            contentBuilder.Append($"\"{section.Value}\",\n");
//        }
//    }
//    return contentBuilder.ToString();
//}




//public class TextConfigurationProvider (string FilePath) : ConfigurationProvider
//{
//    public string FilePath { get; set; } = FilePath;

//    public override void Load()
//    {
//        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
//        using (StreamReader TextReader = new StreamReader(FilePath))
//        {
//            string? line;
//            while ((line = TextReader.ReadLine()) != null)
//            {
//                string key = line.Trim();
//                string? value = TextReader.ReadLine() ?? "";
//                data.Add(key, value);

//            }
//        }

//        Data = data;
//    }
//}

//public class TextConfigurationSource(string FileName) : IConfigurationSource
//{
//    public string FileName { get; } = FileName;

//    public IConfigurationProvider Build(IConfigurationBuilder builder)
//    {
//        string filePath = builder.GetFileProvider().GetFileInfo(FileName).PhysicalPath;
//        return new TextConfigurationProvider(filePath);
//    }
//}

//public static class TextConfigurationExtentions
//{
//    public static IConfigurationBuilder AddTextFile(this IConfigurationBuilder builder, string path)
//    {
//        if (builder == null)
//        {
//            throw new ArgumentNullException(nameof(builder));;
//        }

//        if (string.IsNullOrWhiteSpace(path))
//        {
//            throw new AggregateException("No file path");
//        }

//        var source = new TextConfigurationSource(path);
//        builder.Add(source);
//        return builder;
//    }
//}

public class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; } = 0;
    public List<string> Languages { get; set; } = new();
    public Company? Company { get; set; } = null;
}

public class Company
{
    public string Title { get; set; } = "";
    public string Country { get; set; } = string.Empty;
}

public class PersonMiddleware(RequestDelegate _next, IOptions<Person> options)
{
    private readonly RequestDelegate _next = _next;
    public Person Person { get; } = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        System.Text.StringBuilder stringBuilder = new();
        stringBuilder.Append($"<p>Name: {Person.Name}</p>");
        stringBuilder.Append($"<p>Age: {Person.Age}</p>");
        stringBuilder.Append($"<p>Company: {Person.Company?.Title}</p>");
        stringBuilder.Append("<h3>Languages</h3><ul>");
        foreach (string lang in Person.Languages)
            stringBuilder.Append($"<li>{lang}</li>");
        stringBuilder.Append("</ul>");

        await context.Response.WriteAsync(stringBuilder.ToString());
    }

}