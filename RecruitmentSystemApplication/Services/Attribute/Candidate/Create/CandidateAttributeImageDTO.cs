
using Microsoft.AspNetCore.Http;

namespace RecruitmentSystemApplication.Services.Attribute.Candidate.Create
{
    public record CandidateAttributeImageDTO(string AttributeId, IFormFile file);
}
