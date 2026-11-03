using FluentValidation;

namespace RecruitmentSystemApplication.Services.CandidateProjects.Create
{
    public class CandidateProjectCreateValidator : AbstractValidator<CandidateProjectCreateDTO>
    {
        public CandidateProjectCreateValidator()
        {
            RuleFor(project => project.Name)
                .NotEmpty().WithMessage("Project name cannot be empty");

            RuleFor(project => project.EndDate)
                .Must((project, endDate) => !project.StartDate.HasValue || !endDate.HasValue || endDate >= project.StartDate)
                .WithMessage("End date cannot be before start date");
        }
    }
}
