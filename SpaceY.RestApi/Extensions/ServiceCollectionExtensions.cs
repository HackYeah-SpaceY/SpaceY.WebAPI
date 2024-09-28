using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Entities;
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
        services.AddIdentity<User, IdentityRole>(options =>
        {
            //Wymagania hasla
        }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3001")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
        });

        var assembly = typeof(Program).Assembly;
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddCarter();

    }
}
