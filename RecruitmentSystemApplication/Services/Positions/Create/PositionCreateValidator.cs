using FluentValidation;
namespace RecruitmentSystemApplication.Services.Positions.Create
{
    public class PositionCreateValidator : AbstractValidator<PositionCreateDTO>
    {
        public PositionCreateValidator()
        {
            RuleFor(a => a.Title).NotEmpty().WithMessage("Title must be set");

            RuleFor(a => a.MaxProjectsCount).GreaterThanOrEqualTo(0).WithMessage("It must be greater or equal to 0");
        }
    }
}
