using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RecruitmentSystemInfrastructure
{
    public static class AppDbContextConnection
    {
        public static IServiceCollection AddDbContextConnection(
            this IServiceCollection services)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? throw new NullReferenceException("DB connection string is not set");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            return services;
        }
    }
}
