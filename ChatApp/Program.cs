using ChatApp.Models;
using ChatApp.Data;
using ChatApp.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddSignalR().AddAzureSignalR(options =>
{
    options.ConnectionString = builder.Configuration["Azure__SignalR__ConnectionString"]
        ?? throw new Exception("SignalR connection string not found.");
});


var connectionString =
    builder.Configuration["SQLAZURECONNSTR_AZURE_SQL_CONNECTIONSTRING"]
    ?? throw new Exception("Database connection string not found.");

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chathub");
});

app.Run();
