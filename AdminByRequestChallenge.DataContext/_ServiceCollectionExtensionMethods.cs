using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AdminByRequestChallenge.DataContext;

public static class ServiceCollectionExtensionMethods
{
    public static IServiceCollection UseDbContexts(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<AuthContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("CoreDatabase"));
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        return services;
    }
}

