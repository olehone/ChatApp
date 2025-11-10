using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Options;
using ChatApp.Services;
using Microsoft.Azure.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TextAnalyticsOptions>(
    builder.Configuration.GetSection(TextAnalyticsOptions.SectionName));

builder.Services.AddControllers();
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddSignalR().AddAzureSignalR(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("AzureSignalR")
        ?? throw new Exception("SignalR connection string not found.");
});


var connectionString = builder.Configuration.GetConnectionString("AzureSql")
    ?? throw new Exception("Database connection string not found.");

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.Configure<TextAnalyticsOptions>(options =>
{
    builder.Configuration.GetSection("TextAnalytics").Bind(options);

    var envKey = builder.Configuration["AzureAI:TextAnalyticsKey"];
    var envEndpoint = builder.Configuration["AzureAI:TextAnalyticsEndpoint"];

    if (!string.IsNullOrEmpty(envKey)) options.Key = envKey;
    if (!string.IsNullOrEmpty(envEndpoint)) options.Endpoint = envEndpoint;
    options.Enabled = !string.IsNullOrEmpty(options.Key) && !string.IsNullOrEmpty(options.Endpoint);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
