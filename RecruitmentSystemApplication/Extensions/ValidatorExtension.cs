using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentSystemApplication.Services.Attribute.Create;
using RecruitmentSystemApplication.Services.Attribute.Modify;
using RecruitmentSystemApplication.Services.Positions.Create;

namespace RecruitmentSystemApplication.Extensions
{
    public static class ValidatorExtension
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AttributeModifyValidator>();
            services.AddValidatorsFromAssemblyContaining<AttributeCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<PositionCreateValidator>();
            return services;
        }
    }
}
