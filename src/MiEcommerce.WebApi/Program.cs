using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using MiEcommerce.Application.Common.Behaviors;
using MiEcommerce.Application.Features.Auth;
using MiEcommerce.Infrastructure;
using MediatR;
using MiEcommerce.WebApi.Filters;
using MiEcommerce.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════════════════════
// FASE 1: Registro de servicios
// ═══════════════════════════════════════════════════════════════════════════════

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MiEcommerce API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Por favor ingrese el token JWT",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.OperationFilter<AuthorizeOperationFilter>();
});

// JWT Authentication
var config = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = config["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// MediatR + Behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<LoginCommand>();
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

// FluentValidation — registra todos los validators del ensamblado Application
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommand>();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Exception handler (IExceptionHandler — se activa con app.UseExceptionHandler() más abajo)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════════════════════
// FASE 2: Pipeline de middleware (ORDEN OBLIGATORIO)
// ═══════════════════════════════════════════════════════════════════════════════

app.UseExceptionHandler();     // ← PRIMERO: captura toda excepción de la cadena

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiEcommerce API v1");
    });
}

app.UseAuthentication();        // ← ANTES que UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
