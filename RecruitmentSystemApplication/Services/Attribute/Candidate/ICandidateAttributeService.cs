using FluentResults;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Create;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Get;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Modify;
using System.Net;

namespace RecruitmentSystemApplication.Services.Attribute.Candidate
{
    public interface ICandidateAttributeService
    {
        Task<Result> CreateCandidateAttribute(string userId, CandidateAttributeCreateDTO candidateAttributeCreateDTO);
        Task<Result> ModifyCandidateAttribute(string userId, CandidateAttributeModifyDTO candidateAttributeModifyDTO);
        Task<Result<List<CandidateAttributesDTO>>> GetCandidateAttributes(string userId);
        Task DeleteCandidateAttributes(List<Guid> candidateAttributeIds);
        Task<Result> CreateCandidateAttributeImage(Stream fileStream, string contentType, string userId, bool isAvatar
            , string attributeId);
    }
}
