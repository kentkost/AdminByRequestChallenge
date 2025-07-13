using AdminByRequestChallenge.Core.Configurations;
using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.Core.Repositories;
using AdminByRequestChallenge.Core.Services;
using AdminByRequestChallenge.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AdminByRequestChallenge.Core;

public static class ServiceCollectionExtensionMethods
{
    public static IServiceCollection UseCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ISessionRepository, SessionDatabaseRepository>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        var jwtOptions = configuration.GetRequiredSection("Jwt").Get<JwtOptions>();

        if (jwtOptions is null)
            throw new Exception("Missing configuration for JWT");

        services.AddSingleton<JwtOptions>(jwtOptions);
        return services;
    }
}

