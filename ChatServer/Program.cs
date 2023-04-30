using ChatServer.Auth;
using ChatServer.Data;
using ChatServer.Hubs;
using ChatServer.Models;
using ChatServer.Services;
using ChatServer.Services.Abstraction;
using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Id")
    .AddScheme<IdAuthSchemeOptions, IdAuthSchemeHandler>(
        "Id",
        opts => { }
    );

builder.Services.AddSingleton<OnlineUsersHolder>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ServerDbContext>(opt => {

    var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "db" ,"chat.db");
    opt.UseSqlite($"Filename={dbPath}");
});

builder.Services.AddMemoryCache();


builder.Services.Configure<FtpSettings>(builder.Configuration.GetSection("FtpSettings"));

builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IConnectedAppsRepository, ConnectedAppsRepository>();

//builder.Services.AddHostedService<UpdateChacker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();

//app.MapHub<ApplicationHub>("/appHub");
app.MapHub<ChatHub>("/chatHub"); 

# region Endpopints
app.MapGet("/get-direct-messages/{user1:long}/{user2:long}/{number:int}", 
    async ([FromServices] IMessageService messageService, [FromRoute] long user1, [FromRoute] long user2, [FromRoute] int number) =>
{
    var messages = await messageService.GetDirectMessagesAsync(user1, user2, number);

    return messages;
})
.WithName("GetDirectMessages");

app.MapPost("/create-user", async ([FromServices] IUserService userService, [FromBody] RegisterUserRequest user) =>
{
    return await userService.RegisterUserAsync(user);
})
.WithName("CreateUser");

app.MapPost("/login", async ([FromServices] IUserService userService, [FromBody] LoginRequest login) =>
{
    var user = await userService.GetUserByUsernameAndPasswordAsync(login.UserName, login.Password);
    if (user == null)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(user);
})
.WithName("Login");

app.MapPost("/saveuser", async ([FromServices] IUserService userService, [FromBody] SaveUserRequest user) =>
{
    return await userService.SaveUserAsync(user);
})
.WithName("SaveUser");

app.MapGet("/getuser/{userId:long}", async ([FromServices] IUserService userService, [FromRoute] long userId) =>
{
    return await userService.GetUserByIdAsync(userId);
})
.WithName("GetUser");

app.MapGet("/getusers", async ([FromServices] IUserService userService) =>
{
    return await userService.GetUsersAsync();
})
.WithName("GetUsers");

app.MapGet("/users-with-last-message/{userForId:long}", async ([FromRoute] long userForId, [FromServices] IUserService userService, HttpContext context) =>
{
    
    var usersWithLastMessage =  await userService.GetUserswithLastMessageAsync(userForId);
    return new GetUsersResponse
    {
        UsersWithLastMessage = usersWithLastMessage
    };
})
.WithName("GetUsersWithLastMessage");
# endregion

app.Run();
