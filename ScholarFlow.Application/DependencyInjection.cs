using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ScholarFlow.Application;

/// <summary>
/// Dependency Injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register AutoMapper (if needed later)
        // services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
