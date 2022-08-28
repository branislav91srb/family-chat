using ChatServer.Data;
using ChatServer.Data.Entities;
using ChatServer.Hubs;
using ChatServer.Requests;
using ChatServer.Services;
using ChatServer.Services.Abstraction;
using Contracts;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ServerDbContext>(opt => {

    var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "db" ,"chat.db");
    opt.UseSqlite($"Filename={dbPath}");
});


builder.Services.AddScoped<IMessageRepository, MessageRepository>();

var app = builder.Build();


app.MapHub<ChatHub>("/chatHub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/getmessages/{number:int}", async ([FromServices] IMessageRepository repository, [FromRoute] int number) =>
{
    var messages = await repository.GetMessagesAsync(number);

    var messagesResponse = new MessagesResponse();

    foreach(var message in messages)
    {
        var messageresponse = new Message
        {
            From = message.From,
            SendTime = message.SendTime,
            Text = message.Text
        };

        messagesResponse.Messages.Add(messageresponse);
    }

    return messagesResponse;
})
.WithName("GetMessages");

app.Run();
