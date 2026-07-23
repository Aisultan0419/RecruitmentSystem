
using FluentValidation;
using FluentValidation.AspNetCore;
using RecruitmentSystemApplication.Common;

namespace RecruitmentSystemApplication.Services.Attribute.Modify
{
    public class AttributeModifyValidator : AbstractValidator<AttributeModifyDTO>
    {
        public AttributeModifyValidator()
        {
            RuleFor(a => a.Name).NotEmpty().When(a => a.Name is not null).WithErrorCode("400")
                .WithMessage("Name must be set");

            RuleFor(a => a.DataType).DataTypeRule().When(a => a.DataType is not null);

            RuleFor(a => a.AttributeOptions)
            .RequiredOptionRule(a => a.DataType)
            .When(a => a.AttributeOptions is not null);

            RuleFor(a => a.AttributeOptions)
                .ForbiddenOptionRule(a => a.DataType)
                .When(a => a.AttributeOptions is not null);

            RuleFor(a => a.AttributeOptions)
                    .NotEmpty()
                    .When(a => string.Equals(a.DataType, "OneOfMany", StringComparison.OrdinalIgnoreCase))
                    .WithErrorCode("400")
                    .WithMessage("You must provide attribute options when changing data type to One of Many");

            RuleFor(a => a.Id)
                    .NotEmpty()
                    .WithErrorCode("400")
                    .WithMessage("You must provide attribute id");
        }
    }
}
