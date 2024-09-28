using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Services;
using System;

namespace SpaceY.RestApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("postgres")));
    }


    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(Program).Assembly;
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddScoped<IAuthService, AuthService>();
        services.AddCarter();

    }
}
