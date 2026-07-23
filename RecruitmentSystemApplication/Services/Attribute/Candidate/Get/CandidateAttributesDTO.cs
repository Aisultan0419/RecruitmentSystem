
namespace RecruitmentSystemApplication.Services.Attribute.Candidate.Get
{
    public record CandidateAttributesDTO(Guid Id,
        string Value,
        Guid AttributeId,
        Guid UserProfileId,
        long Version,
        DateTime UpdatedAt);
}
