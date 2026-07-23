using FluentValidation;
using RecruitmentSystemDomain.Enums;

namespace RecruitmentSystemApplication.Common
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, ICollection<string>?> RequiredOptionRule<T>(
            this IRuleBuilder<T, ICollection<string>?> ruleBuilder, Func<T, string?> dataTypeSelector)
        {
            return ruleBuilder
                .NotEmpty()
                .Must((ICollection<string>? collection) => collection != null && collection.Count > 0)
                .When(dto => dataTypeSelector(dto) != null && string.Equals(dataTypeSelector(dto)!.Replace(" ", ""), "OneOfMany", StringComparison.OrdinalIgnoreCase))
                .WithErrorCode("400")
                .WithMessage("Attribute options are empty");    
        }
        public static IRuleBuilderOptions<T, ICollection<string>?> ForbiddenOptionRule<T>(
            this IRuleBuilder<T, ICollection<string>?> ruleBuilder, Func<T, string?> dataTypeSelector)
        {
            return ruleBuilder
                .Empty()
                .When(dto => dataTypeSelector(dto) != null && !string.Equals(dataTypeSelector(dto)!.Replace(" ", ""), "OneOfMany", StringComparison.OrdinalIgnoreCase))
                .WithErrorCode("400")
                .WithMessage("Attribute options can be filled only with type One of Many ");
        }

        public static IRuleBuilderOptions<T, string?> DataTypeRule<T>(
            this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .Must(dataType =>
                {
                    if (dataType is null) return true;
                    var normalized = dataType.Replace(" ", "");
                    return Enum.TryParse(typeof(DataType), normalized, ignoreCase: true, out _);
                })
                .WithErrorCode("400")
                .WithMessage("Invalid data type");
                }
    }
}
