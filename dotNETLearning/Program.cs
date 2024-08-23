using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
//{
//    {"name", "Tom"},
//    {"age", "26"}
//});


// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

//builder.Configuration.AddJsonFile("project.json");
//builder.Configuration.AddXmlFile("config2.xml");
builder.Configuration.AddTextFile("config3.txt");

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
app.Map("/conf", (IConfiguration appConfig)=> $"{appConfig["name"]} {appConfig["age"]}");

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

public class TextConfigurationProvider (string FilePath) : ConfigurationProvider
{
    public string FilePath { get; set; } = FilePath;

    public override void Load()
    {
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        using (StreamReader TextReader = new StreamReader(FilePath))
        {
            string? line;
            while ((line = TextReader.ReadLine()) != null)
            {
                string key = line.Trim();
                string? value = TextReader.ReadLine() ?? "";
                data.Add(key, value);

            }
        }

        Data = data;
    }
}

public class TextConfigurationSource(string FileName) : IConfigurationSource
{
    public string FileName { get; } = FileName;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        string filePath = builder.GetFileProvider().GetFileInfo(FileName).PhysicalPath;
        return new TextConfigurationProvider(filePath);
    }
}

public static class TextConfigurationExtentions
{
    public static IConfigurationBuilder AddTextFile(this IConfigurationBuilder builder, string path)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new AggregateException("No file path");
        }

        var source = new TextConfigurationSource(path);
        builder.Add(source);
        return builder;
    }
}