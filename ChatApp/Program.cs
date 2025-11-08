using ChatApp.Models;
using ChatApp.Data;
using ChatApp.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});
builder.Services.AddSignalR();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseInMemoryDatabase("ChatAppDb"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING")
        ?? throw new Exception("Database connection string not found.");

    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    db.ChatMessages.AddRange(
        new ChatMessage { Username = "System", Message = "Hello!" },
        new ChatMessage { Username = "System", Message = "Welcome to the chat room!" }
    );
    db.SaveChanges();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
