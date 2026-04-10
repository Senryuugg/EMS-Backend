using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using EmsDispatch.Backend.Configuration;
using EmsDispatch.Backend.Services;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// MongoDB Configuration
var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoSettings>();
if (mongoSettings == null)
    throw new InvalidOperationException("MongoDB settings are not configured");

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT secret key is not configured");

var jwtKeyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // SignalR JWT token from query string or header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDispatchService, DispatchService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IAmbulanceService, AmbulanceService>();
builder.Services.AddScoped<ISessionService, SessionService>();

// Add Controllers
builder.Services.AddControllers().AddNewtonsoftJson();

// Add SignalR
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:*", "https://localhost:*")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.CreateBuilder();

// Middleware
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<DispatchHub>("/hubs/dispatch");
app.MapHub<LocationHub>("/hubs/location");
app.MapHealthChecks("/health");

app.Run();
