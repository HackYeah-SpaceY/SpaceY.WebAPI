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
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 0;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        //services.AddCors(options =>
        //{
        //    options.AddPolicy("AllowFrontend",
        //        builder =>
        //        {
        //            builder.WithOrigins("http://localhost:3001")
        //                  .AllowAnyMethod()
        //                  .AllowAnyHeader()
        //                  .AllowCredentials();
        //        });
        //});

        //services.AddCors(options =>
        //{
        //    options.AddPolicy("AllowFrontend2",
        //        builder =>
        //        {
        //            builder.WithOrigins("https://chef-spacey.vercel.app")
        //                  .AllowAnyMethod()
        //                  .AllowAnyHeader()
        //                  .AllowCredentials();
        //        });
        //});

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
        });

        var assembly = typeof(Program).Assembly;
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IPythonService, PythonService>();
        services.AddCarter();

    }
}
