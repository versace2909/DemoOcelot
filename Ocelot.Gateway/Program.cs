using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddJsonFile("ocelot.json")
    .AddEnvironmentVariables();
builder.AddServiceDefaults();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("JwtBearer", options =>
    {
        options.Authority = "https://localhost:7108";
        options.Audience = "weatherforecast";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudiences = new[] {"weatherforecast"},
            ValidIssuers = new[] {"https://localhost:7108"},
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey("ThisIsMyVerySecretKey@12345##$%^"u8.ToArray())
        };
        options.MapInboundClaims = false;
    });

builder.Services.AddOcelot();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseOcelot().Wait();
app.UseHttpsRedirection();
app.Run();