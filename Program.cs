using Dapper;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Repositories;
using Repositories.IRepositories;
using Scalar.AspNetCore;
using Services;
using Services.IServices;
using System.Data;
using System.Text;
using Utilities.Filters;
using Utilities.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ====================================================================
// CONFIGURACIÓN DE BASE DE DATOS
// ====================================================================

var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection")
    ?? throw new InvalidOperationException("Connection string 'SupabaseConnection' not found.");
connectionString = BuildPostgresConnectionString(connectionString);

var keysPath = Path.Combine(builder.Environment.ContentRootPath, ".keys");
if (!Directory.Exists(keysPath)) Directory.CreateDirectory(keysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("SIAE-SAEV3");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        npgsqlOptions.CommandTimeout(120);
    });
    options.UseSnakeCaseNamingConvention();
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(connectionString));

// ====================================================================
// CONFIGURACIÓN DE CORS
// ====================================================================

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? [];
allowedOrigins = allowedOrigins
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .ToArray();

if (allowedOrigins.Length == 0 && builder.Environment.IsDevelopment())
    allowedOrigins = ["http://localhost:4200"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            policy.AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

// ====================================================================
// SERVICIOS Y REPOSITORIOS
// ====================================================================

builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IJWTService, JWTService>();

builder.Services.AddScoped<IUserRepositorie, UserRepositorie>();
builder.Services.AddScoped<IAdminCatalogRepositorie, AdminCatalogRepositorie>();
builder.Services.AddScoped<IStudentRepositorie, StudentRepositorie>();
builder.Services.AddScoped<IStudentSupportRepositorie, StudentSupportRepositorie>();
builder.Services.AddScoped<INotificationRepositorie, NotificationRepositorie>();

// ====================================================================
// CONFIGURACIÓN JWT
// ====================================================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                    token = context.Request.Cookies["jwt"];

                if (!string.IsNullOrEmpty(token))
                    context.Token = token;

                return Task.CompletedTask;
            }
        };
    });

// ====================================================================
// OTROS SERVICIOS
// ====================================================================

builder.Services.AddControllers(options =>
{
    options.Filters.Add<JSchemaResultFilter>();
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// ====================================================================
// APLICAR MIGRACIONES AUTOMÁTICAMENTE AL INICIAR
// ====================================================================

if (builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", builder.Environment.IsDevelopment()))
{
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    app.Logger.LogInformation("Database migrations applied successfully.");
}
catch (Exception ex) when (ex is NpgsqlException or TimeoutException or InvalidOperationException)
{
    app.Logger.LogError(ex, "No se pudieron aplicar migraciones al iniciar. La API seguirá levantando; revisa conexión a PostgreSQL.");
}
}

// ====================================================================
// CONFIGURACIÓN DEL PIPELINE HTTP
// ====================================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("SIAE API - Microservicio Gestión de Usuarios")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngularDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => app.Environment.IsDevelopment()
        ? Results.Redirect("/scalar/v1")
        : Results.Ok(new { service = "SIAE-SAEV3", status = "running" }))
    .ExcludeFromDescription();

app.MapGet("/health", () => Results.Ok(new
{
    service = "SIAE-SAEV3",
    status = "ok",
    utc = DateTime.UtcNow
}));

app.MapGet("/health/database", async (AppDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        return canConnect
            ? Results.Ok(new { status = "ok", database = "connected" })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception ex) when (ex is NpgsqlException or TimeoutException or InvalidOperationException)
    {
        return Results.Json(
            new { status = "unavailable", database = "disconnected", message = ex.GetBaseException().Message },
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.Run();

static string BuildPostgresConnectionString(string rawConnectionString)
{
    var builder = new NpgsqlConnectionStringBuilder(rawConnectionString)
    {
        Timeout = 30,
        CommandTimeout = 120,
        KeepAlive = 30,
        Pooling = true,
        MinPoolSize = 0,
        MaxPoolSize = 20,
        ConnectionIdleLifetime = 30,
        ConnectionPruningInterval = 10,
        IncludeErrorDetail = true
    };

    return builder.ConnectionString;
}
