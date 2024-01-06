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

builder.Services.AddHostedService<TimeService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            NameClaimType = ClaimTypes.NameIdentifier,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (path.StartsWithSegments("/sampleHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHub<SampleHub>("/sampleHub");
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("", async (string message, IHubContext<SampleHub> hub) =>
{
    await hub.Clients.All.SendAsync("ReceiveMessage", message);
});

app.MapGet("jwt", (string user, IJwtHandler jwtHandler) =>
{
    return jwtHandler.GenerateToken(user);
});

app.Run();

