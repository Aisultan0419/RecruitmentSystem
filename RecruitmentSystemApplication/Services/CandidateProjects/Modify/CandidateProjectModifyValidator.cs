using FluentValidation;

namespace RecruitmentSystemApplication.Services.CandidateProjects.Modify
{
    public class CandidateProjectModifyValidator : AbstractValidator<CandidateProjectModifyDTO>
    {
        public CandidateProjectModifyValidator()
        {
            RuleFor(project => project.Id)
                .NotEmpty().WithMessage("Project ID cannot be empty");

            RuleFor(project => project.Name)
                .NotEmpty().WithMessage("Project name cannot be empty");

            RuleFor(project => project.EndDate)
                .Must((project, endDate) => !project.StartDate.HasValue || !endDate.HasValue || endDate >= project.StartDate)
                .WithMessage("End date cannot be before start date");
        }
    }
}
