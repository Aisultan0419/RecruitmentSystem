using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentSystemApplication.Services.Attribute.Create;
using RecruitmentSystemApplication.Services.Attribute.Modify;

namespace RecruitmentSystemApplication.Extensions
{
    public static class ValidatorExtension
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AttributeModifyValidator>();
            services.AddValidatorsFromAssemblyContaining<AttributeCreateValidator>();
            return services;
        }
    }
}
