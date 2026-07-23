using FluentValidation;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemApplication.Common;
namespace RecruitmentSystemApplication.Services.Attribute.Create
{
    public class AttributeCreateValidator : AbstractValidator<AttributeCreateDTO>
    {
        public AttributeCreateValidator()
        {
            RuleFor(a => a.Name).NotEmpty().WithErrorCode("400").WithMessage("Name must be set");

            RuleFor(a => a.DataType).NotEmpty().DataTypeRule();

            RuleFor(a => a.AttributeOptions)
                .RequiredOptionRule(a => a.DataType);

            RuleFor(a => a.AttributeOptions)
                .ForbiddenOptionRule(a => a.DataType);
        }
    }
}
