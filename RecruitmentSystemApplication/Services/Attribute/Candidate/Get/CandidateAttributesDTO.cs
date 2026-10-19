using RecruitmentSystemDomain.Enums;

namespace RecruitmentSystemApplication.Services.Attribute.Candidate.Get
{
    public record CandidateAttributesDTO(Guid Id,
        string Value,
        Guid AttributeId,
        string AttributeName,
        DataType DataType,
        Guid UserProfileId,
        long Version,
        DateTime UpdatedAt);
}
