
using FluentValidation;

namespace RecruitmentSystemApplication.Services.Tags.Create
{
    public class TagCreateValidator : AbstractValidator<TagCreateDTO>
    {
        public TagCreateValidator()
        {
            RuleFor(tag => tag.Name)
                .NotEmpty().WithMessage("Tag name cannot be empty")
                .MaximumLength(50).WithMessage("Tag name cannot exceed 50 characters");
        }
    }
}
