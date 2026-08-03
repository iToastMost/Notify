using Notify;
using Notify.Classes;
using Notify.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DatabaseConnection>(new DatabaseConnection());

builder.WebHost.UseUrls("http://0.0.0.0:5272");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapGet("/searchSong/{songName}", (string songName, DatabaseConnection db) =>
{
    if (string.IsNullOrWhiteSpace(songName))
    {
        return Results.BadRequest("A song name is required.");
    }
    var song = db.SearchSong(songName);
    return TypedResults.Ok(song);
});

app.MapGet("/music", (DatabaseConnection db) =>
{
    return db.GetSongs();
});

app.MapGet("/", () =>
{
    var path = "home/notifyserver/media/music/complete";
    
    var files = Directory.GetFiles(path);
    
    return Results.Ok(files);
});

//scan and update db
app.MapPost("/scan", (DatabaseConnection db) =>
{
    var path = "home/notifyserver/media/music/complete";
    
    var files = Directory.GetFiles(path);

    foreach (var file in files)
    {
        
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}