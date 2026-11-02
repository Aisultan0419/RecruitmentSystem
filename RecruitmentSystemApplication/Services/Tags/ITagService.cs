using FluentResults;
using RecruitmentSystemApplication.Services.Tags.Create;
using RecruitmentSystemApplication.Services.Tags.Modify;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Tags
{
    public interface ITagService
    {
        Task<Result<Guid>> Create(TagCreateDTO tagCreateDTO);
        Task<Result<List<Tag>>> GetTags(string? prefix);
        Task<Result> Modify(TagModifyDTO tagModifyDTO);
        Task<Result> Delete(Guid id);
    }
}
