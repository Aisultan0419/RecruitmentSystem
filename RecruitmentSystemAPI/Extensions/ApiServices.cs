using RecruitmentSystemApplication.Common;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Contracts;
using RecruitmentSystemApplication.Services.Attribute.Candidate;
using RecruitmentSystemApplication.Services.Attribute.Common;
using RecruitmentSystemApplication.Services.Auth.Login;
using RecruitmentSystemApplication.Services.Auth.Register;
using RecruitmentSystemApplication.Services.Positions;
using RecruitmentSystemApplication.Services.Positions.Create;
using RecruitmentSystemApplication.Services.Profile;
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
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAttributeRepository, AttributeRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IAttributeService, AttributeService>();
            services.AddScoped<ICandidateAttributeService, CandidateAttributeService>();
            services.AddScoped<ICommonMethods, CommonMethods>();
            services.AddScoped<IPositionService, PositionService>();
            return services;
        }
    }
}
