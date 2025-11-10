using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Options;
using ChatApp.Services;
using Microsoft.Azure.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Text Analytics Options
builder.Services.Configure<TextAnalyticsOptions>(options =>
{
    builder.Configuration.GetSection("TextAnalytics").Bind(options);

    var envKey = builder.Configuration["AzureAI:TextAnalyticsKey"];
    var envEndpoint = builder.Configuration["AzureAI:TextAnalyticsEndpoint"];

    if (!string.IsNullOrEmpty(envKey)) options.Key = envKey;
    if (!string.IsNullOrEmpty(envEndpoint)) options.Endpoint = envEndpoint;

    // Enable Text Analytics only if both Key and Endpoint are available
    options.Enabled = !string.IsNullOrEmpty(options.Key) && !string.IsNullOrEmpty(options.Endpoint);
});

builder.Services.AddControllers();

// Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// SignalR 
builder.Services.AddSignalR().AddAzureSignalR(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("AzureSignalR")
        ?? throw new Exception("SignalR connection string not found.");
});

// Database
var connectionString = builder.Configuration.GetConnectionString("AzureSql")
    ?? throw new Exception("Database connection string not found.");

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(connectionString));

// Services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// App

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
