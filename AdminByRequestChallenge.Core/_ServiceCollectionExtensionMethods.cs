using AdminByRequestChallenge.Core.Interfaces;
using AdminByRequestChallenge.Core.Repositories;
using AdminByRequestChallenge.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AdminByRequestChallenge.Core;

public static class ServiceCollectionExtensionMethods
{
    public static IServiceCollection UseCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<IUsersRepository, UsersRepository>();
        return services;
    }
}

