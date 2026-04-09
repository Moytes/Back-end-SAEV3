using Dapper;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ====================================================================
// CONFIGURACIÓN DE BASE DE DATOS
// ====================================================================

// Obtener connection string
var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection")
    ?? throw new InvalidOperationException("Connection string 'SupabaseConnection' not found.");

// Configurar Entity Framework Core con PostgreSQL y Snake Case
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();
});

// Configurar Dapper para usar snake_case
DefaultTypeMap.MatchNamesWithUnderscores = true;

// Registrar IDbConnection para Dapper
builder.Services.AddScoped<IDbConnection>(sp => 
    new NpgsqlConnection(connectionString));

// ====================================================================
// SERVICIOS Y REPOSITORIOS
// ====================================================================

// Services
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IJWTService, JWTService>();

// Repositories
builder.Services.AddScoped<IUserRepositorie, UserRepositorie>();
builder.Services.AddScoped<IServiceRepositorie, ServiceRepositorie>();
builder.Services.AddScoped<IAdminCatalogRepositorie, AdminCatalogRepositorie>();
builder.Services.AddScoped<IStudentRepositorie, StudentRepositorie>();
builder.Services.AddScoped<IStudentSupportRepositorie, StudentSupportRepositorie>();
builder.Services.AddScoped<ICanalizationRepositorie, CanalizationRepositorie>();
builder.Services.AddScoped<IPsychoeducationalAssessmentRepositorie, PsychoeducationalAssessmentRepositorie>();
builder.Services.AddScoped<ICIERepositorie, CIERepositorie>();
builder.Services.AddScoped<ITEARepositorie, TEARepositorie>();

// ====================================================================
// Settings para el JWT
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

builder.Services.AddControllers();

// OpenAPI and Scalar Documentation
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ====================================================================
// APLICAR MIGRACIONES AUTOMÁTICAMENTE
// ====================================================================

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<AppDbContext>();
        
//        // Aplicar migraciones pendientes
//        context.Database.Migrate();
        
//        app.Logger.LogInformation("Database migrations applied successfully.");
//    }
//    catch (Exception ex)
//    {
//        app.Logger.LogError(ex, "An error occurred while migrating the database.");
//        throw;
//    }
//}

// ====================================================================
// CONFIGURACIÓN DEL PIPELINE HTTP
// ====================================================================

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Scalar API Documentation - Accessible at /scalar/v1
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("SIAE API Documentation")
            .WithTheme(Scalar.AspNetCore.ScalarTheme.Purple)
            .WithDefaultHttpClient(Scalar.AspNetCore.ScalarTarget.CSharp, Scalar.AspNetCore.ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Redireccionar la raíz a la documentación de Scalar
app.MapGet("/", () => Results.Redirect("/scalar/v1"))
    .ExcludeFromDescription();

app.Run();
