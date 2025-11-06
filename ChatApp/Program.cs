using ChatApp.Models;
using ChatApp.Data;
using ChatApp.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var environment = "Development"; // "Production" 
builder.Environment.EnvironmentName = environment;

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
    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        db.ChatMessages.Add(new ChatMessage { Username = "System", Message = "Hello!" });
        db.ChatMessages.Add(new ChatMessage { Username = "System", Message = "Wellcome to chat room!" });
        db.SaveChanges();

    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");


app.UsePathBase("/swagger");
app.UseStaticFiles();

app.Run();