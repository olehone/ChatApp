using ChatApp.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var environment = "Development"; // "Production" 
builder.Environment.EnvironmentName = environment;

builder.Services.AddControllers();

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
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    db.ChatMessages.Add(new ChatApp.API.Models.ChatMessage { Username = "System", Message = "Hello!" });
    db.ChatMessages.Add(new ChatApp.API.Models.ChatMessage { Username = "System", Message = "Wellcome to chat room!" });
    db.SaveChanges();

}
app.UsePathBase("/swagger");

app.Run();
