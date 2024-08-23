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

builder.Configuration.AddJsonFile("project.json");
//builder.Configuration.AddXmlFile("config2.xml");

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
app.Map("/Conf", (IConfiguration appConfig)=>GetSectionContent(appConfig.GetSection("projectConfig")));


app.Run();

string GetSectionContent(IConfigurationSection configSection)
{
    System.Text.StringBuilder contentBuilder = new();
    foreach (var section in configSection.GetChildren())
    {
        contentBuilder.Append($"\"{section.Key}\":");
        if (section.Value == null)
        {
            string subSectionContent = GetSectionContent(section);
            contentBuilder.Append($"{{\n{subSectionContent}}},\n");
        }
        else
        {
            contentBuilder.Append($"\"{section.Value}\",\n");
        }
    }
    return contentBuilder.ToString();
}

