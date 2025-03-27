using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver"),
        new UrlSegmentApiVersionReader());
}).AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddServiceDefaults();

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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("abcd", policy => policy.RequireClaim("scope", "abc"));
});

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


var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

app.MapGet("/api/v{version:apiVersion}/weatherforecast", [Authorize("abcd")]() => TypedResults.Ok("Ho Chi Minh"))
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1)
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();