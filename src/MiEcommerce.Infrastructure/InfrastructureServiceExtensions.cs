using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiEcommerce.Application.Contracts.Infrastructure;
using MiEcommerce.Domain.Interfaces;
using MiEcommerce.Infrastructure.Middleware;
using MiEcommerce.Infrastructure.Persistence;
using MiEcommerce.Infrastructure.Repositories;
using MiEcommerce.Infrastructure.Services;

namespace MiEcommerce.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("MiEcommerce.Infrastructure")
            ), ServiceLifetime.Scoped);

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IEmailService, EmailService>();

        // PaymentService (microservicio externo, comunicación por HttpClient)
        services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["PaymentService:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Exception handler (IExceptionHandler — se activa con app.UseExceptionHandler() en Program.cs)
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
