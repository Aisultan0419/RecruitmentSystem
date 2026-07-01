using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RecruitmentSystemInfrastructure
{
    public static class AppDbContextConnection
    {
        public static IServiceCollection AddDbContextConnection(
            this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"] ?? throw new NullReferenceException("DB connection string is not set");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            return services;
        }
    }
}
