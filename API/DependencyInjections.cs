using Microsoft.EntityFrameworkCore;
using Repositories.Context;

namespace API
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnectionString");
                options.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}
