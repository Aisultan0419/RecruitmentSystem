
using FluentValidation;

namespace RecruitmentSystemApplication.Services.Tags.Modify
{
    public class TagModifyValidator : AbstractValidator<TagModifyDTO>
    {
        public TagModifyValidator()
        {
            RuleFor(tag => tag.Name)
               .NotEmpty().WithMessage("Tag name cannot be empty")
               .MaximumLength(50).WithMessage("Tag name cannot exceed 50 characters");
        }
    }
}
