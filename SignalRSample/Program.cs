using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using SignalRServer;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var secretKey = Guid.NewGuid().ToString();
var jwtIssuer = "test";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IJwtHandler>(new JwtHandler(secretKey, jwtIssuer, 30));

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

app.MapGet("jwt", (string user, IJwtHandler jwtHandler) =>
{
    return jwtHandler.GenerateToken(user);
});

app.Run();

