using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Contracts;
using RecruitmentSystemApplication.Services.Auth.Register;
using RecruitmentSystemInfrastructure;
using RecruitmentSystemInfrastructure.Utils;
namespace RecruitmentSystemAPI.Extensions
{
    public static class ApiServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<IHashingService, HashingService>();

            return services;
        }
    }
}
