using Azure.Identity;
using ChatApp.Models;
using ChatApp.Data;
using ChatApp.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment;

if (!environment.IsDevelopment())
{
    var keyVaultName = builder.Configuration["KeyVaultName"];
    if (string.IsNullOrWhiteSpace(keyVaultName))
        throw new Exception("KeyVaultName not configured in App Settings or Environment Variables.");

    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

if (environment.IsDevelopment())
{
    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseInMemoryDatabase("ChatAppDb"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new Exception("No connection string found in Key Vault or App Configuration.");

    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseSqlServer(connectionString));
}

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    db.ChatMessages.AddRange(
        new ChatMessage { Username = "System", Message = "Hello!" },
        new ChatMessage { Username = "System", Message = "Welcome to chat room!" }
    );
    db.SaveChanges();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.UseStaticFiles();
app.Run();
