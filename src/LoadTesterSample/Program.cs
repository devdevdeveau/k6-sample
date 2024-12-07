using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/people/{lastname}/{firstname}",
    (string lastname, string firstname, [FromServices] IMemoryCache memoryCache) =>
        memoryCache.TryGetValue<Person>(Person.Key(firstname, lastname), out var person) && person is not null
            ? Results.Ok(person)
            : Results.NotFound());

app.MapPut("/people/{lastname}/{firstname}",
    (string lastname, string firstname, [FromServices] IMemoryCache memoryCache, HttpRequest request) =>
    {
        Person person = new(firstname, lastname);
        
        memoryCache.Remove(person.FullName);
        var cached = memoryCache.GetOrCreate<Person>(person.FullName, entry =>
        {
            entry.Value = person;
            return person;
        });

        var appUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/people/{lastname}/{firstname}";
        
        return Results.Created(new Uri(appUrl), cached);
    });

app.MapGet("/weatherforecast", () =>
    {
        return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ));
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record Person(string FirstName, string LastName)
{
    public string FullName => Key(FirstName, LastName);

    public DateTimeOffset DateCreated { get; init; } = DateTimeOffset.UtcNow;

    public static string Key(string firstname, string lastname) => $"{firstname} {lastname}";
}
