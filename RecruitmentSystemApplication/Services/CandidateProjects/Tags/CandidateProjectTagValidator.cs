using FluentValidation;

namespace RecruitmentSystemApplication.Services.CandidateProjects.Tags
{
    public class CandidateProjectTagValidator : AbstractValidator<CandidateProjectTagDTO>
    {
        public CandidateProjectTagValidator()
        {
            RuleFor(tag => tag.CandidateProjectId)
                .NotEmpty().WithMessage("Candidate project ID cannot be empty");

            RuleFor(tag => tag.TagId)
                .NotEmpty().WithMessage("Tag ID cannot be empty");
        }
    }
}
