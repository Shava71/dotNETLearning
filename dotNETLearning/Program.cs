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

//app.Run(async (context) =>
//{
//    context.Response.ContentType = "text/html; charset=utf-8";

//    // если обращение идет по адресу "/postuser", получаем данные формы
//    if (context.Request.Path == "/postuser")
//    {
//        var form = context.Request.Form;
//        string name = form["name"];
//        string age = form["age"];
//        string[] languages = form["languages"];
//        string langlist = "";

//        foreach (var lang in languages)
//        {
//            langlist += $" {lang}";
//        }

//        await context.Response.WriteAsync($"<div><p>Name: {name}</p><p>Age: {age}</p><p>Language: {langlist}</p></div>");
//    }
//    else
//    {
//        await context.Response.SendFileAsync("Pages/index.cshtml");
//    }
//}); //Отправка форм




//app.Run(async (context) =>
//{
//    if (context.Request.Path == "/old")
//    {
//        //await context.Response.WriteAsync("Old page");
//        context.Response.Redirect("https://anilib.me/ru/anime/9654--koutetsujou-no-kabaneri-anime/watch?episode=64074&player=Animelib&team=32412&translation_type=2");
//    }
//    else if (context.Request.Path == "/new")
//    {
//        await context.Response.WriteAsync("New page");
//    }
//    else
//    {
//        await context.Response.WriteAsync("Main Page");
//    }
//});     //Redirect



//app.Run(async(context) =>
//{
//    var response = context.Response;
//    var request = context.Request;

//    if (request.Path == "/api/user")
//    {
//        var message = "Incorrect data";
//        if (request.HasJsonContentType())
//        {
//            var jsonoption = new JsonSerializerOptions();

//            jsonoption.Converters.Add(new PersonConverter());

//            var person = await request.ReadFromJsonAsync<Person>(jsonoption);
//            if (person != null) message = $"Name: {person.Name} Age: {person.Age}";
//        }

//        await response.WriteAsJsonAsync(new { text = message });
//    }
//    else
//    {
//        response.ContentType = "text/html; charset=utf-8";
//        await response.SendFileAsync("Pages/index.cshtml");
//    }
//});






//public record Person(string Name, int Age);
//public class PersonConverter : JsonConverter<Person>
//{
//    public override Person Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        var personName = "Undefined";
//        var personAge = 0;
//        while (reader.Read())
//        {
//            if (reader.TokenType == JsonTokenType.PropertyName)
//            {
//                var propertyName = reader.GetString();
//                reader.Read();
//                switch (propertyName?.ToLower())
//                {
//                    // если свойство age и оно содержит число
//                    case "age" when reader.TokenType == JsonTokenType.Number:
//                        personAge = reader.GetInt32();  // считываем число из json
//                        break;
//                    // если свойство age и оно содержит строку
//                    case "age" when reader.TokenType == JsonTokenType.String:
//                        string? stringValue = reader.GetString();
//                        // пытаемся конвертировать строку в число
//                        if (int.TryParse(stringValue, out int value))
//                        {
//                            personAge = value;
//                        }
//                        break;
//                    case "name":    // если свойство Name/name
//                        string? name = reader.GetString();
//                        if (name != null)
//                            personName = name;
//                        break;
//                }
//            }
//        }
//        return new Person(personName, personAge);
//    }
//    // сериализуем объект Person в json
//    public override void Write(Utf8JsonWriter writer, Person person, JsonSerializerOptions options)
//    {
//        writer.WriteStartObject();
//        writer.WriteString("name", person.Name);
//        writer.WriteNumber("age", person.Age);

//        writer.WriteEndObject();
//    }
//}

List<Person> persons = new List<Person>()
{
    new() { Id = Guid.NewGuid().ToString(), Name = "Tom", Age = 42 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Bob", Age = 24 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 35 }
};


app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;

    string expressionForGuid = @"^/api/users/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/users" && request.Method == "GET")
    {
        await GetAllPeople(response);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
    {
        // получаем id из адреса url
        string? id = path.Value?.Split("/")[3];
        await GetPerson(id, response);
    }
});


app.Run();


public class Person
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Age { get; set; }
}