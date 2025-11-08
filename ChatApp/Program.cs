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

var connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_AZURE_SQL_CONNECTIONSTRING")
    ?? throw new Exception("Database connection string not found.");

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.UseStaticFiles();
app.Run();
