using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsByPrefix(string? prefix);
        Task<Tag?> GetTag(Guid id);
        Task<Tag?> GetTagByName(string name);
    }
}
