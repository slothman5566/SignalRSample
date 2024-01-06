using Microsoft.AspNetCore.SignalR;
using SignalRServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.MapHub<SampleHub>("/sampleHub");


app.MapPost("", async (string message, IHubContext<SampleHub> hub) =>
{
    await hub.Clients.All.SendAsync("ReceiveMessage", message);
});

app.Run();

