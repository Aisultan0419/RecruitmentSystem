using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Contracts;
using RecruitmentSystemApplication.Services.Auth.Login;
using RecruitmentSystemApplication.Services.Auth.Register;
using RecruitmentSystemInfrastructure;
using RecruitmentSystemInfrastructure.Repositories;
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
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IGoogleValidationService, GoogleValidationService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddScoped<ILoginService, LoginService>();
            return services;
        }
    }
}
