using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SignalRServer;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var secretKey = Guid.NewGuid().ToString();
var jwtIssuer = "test";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Authorization
    options.AddSecurityDefinition("Bearer",
    new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization"
    });


    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});
builder.Services.AddSignalR();

builder.Services.AddSingleton<IJwtHandler>(new JwtHandler(secretKey, jwtIssuer, 30));
builder.Services.AddSingleton<CurrentClientService>();
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
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
                else if (context.Request.Headers.Authorization.Count > 0)
                {
                    context.Token = context.Request.Headers.Authorization.First();
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
}).RequireAuthorization();

app.MapGet("jwt", (string user, IJwtHandler jwtHandler) =>
{
    return jwtHandler.GenerateToken(user);
});

app.Run();

