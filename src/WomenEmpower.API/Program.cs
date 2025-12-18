using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WomenEmpower.API.Services;
using WomenEmpower.Core.Entities;
using WomenEmpower.Infrastructure.Data; // Add this at the top with other using directives
                                        // This is the key using for UseSwagger and UseSwaggerUI

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// --- SERVICE REGISTRATION ---

// 1. Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register Identity (Must come after DbContext)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// CRITICAL FIX: Explicitly add Authorization services
builder.Services.AddAuthorization();

// 3. Register Token Service
builder.Services.AddScoped<TokenService>();

// 4. Register JWT Authentication Scheme
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey))
{
    // This will tell us if the app actually sees the key or not
    throw new InvalidOperationException("JWT Secret Key is not configured in appsettings.json.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add services to the container.
builder.Services.AddControllers();

// --- SWAGGER/OPENAPI SERVICES (UPDATED) ---
builder.Services.AddEndpointsApiExplorer(); // Enables API exploration for tools like Swagger
builder.Services.AddSwaggerGen();           // Registers the Swagger generator
// ----------------------------------------

var app = builder.Build();

// --- MIDDLEWARE CONFIGURATION ---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // --- SWAGGER MIDDLEWARE (UPDATED) ---
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Women Empower API V1");
        // This makes it load on the root URL like http://localhost:7063/
        options.RoutePrefix = "swagger";
    });
    // ------------------------------------
}

app.UseHttpsRedirection();

// CRITICAL: Auth middleware order
app.UseAuthentication();
app.UseAuthorization();

// Maps controllers and other endpoints
app.MapControllers();

// (Your existing weather forecast endpoints)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}