using System.Text;
using Identity.Service.Interfaces;
using Identity.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddEndpointsApiExplorer();
builder.AddServiceDefaults();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/token", ([FromServices] IAuthenticationService authenticationService) =>
    {
        var token = authenticationService.GenerateJwtToken("vvdung2");
        return token;
    })
    .WithName("Identity")
    .WithOpenApi();

app.MapGet("/resource", () => { return "Private Resource"; })
    .RequireAuthorization()
    .WithName("Resource")
    .WithOpenApi();

app.Run();