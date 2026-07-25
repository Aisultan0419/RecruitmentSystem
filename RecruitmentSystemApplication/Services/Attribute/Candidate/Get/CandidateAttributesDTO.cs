
namespace RecruitmentSystemApplication.Services.Attribute.Candidate.Get
{
    public record CandidateAttributesDTO(Guid Id,
        string Value,
        Guid AttributeId,
        string AttributeName,
        Guid UserProfileId,
        long Version,
        DateTime UpdatedAt);
}
