using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemInfrastructure.Repositories
{
    public class TagRepository(AppDbContext dbContext) : ITagRepository
    {
        public async Task<List<Tag>> GetTagsByPrefix(string prefix)
        {
            return await dbContext.Tags
                .AsNoTracking()
                .Where(tag => EF.Functions.ILike(tag.Name, $"{prefix}%"))
                .OrderBy(tag => tag.Name)
                .ToListAsync();
        }

        public async Task<Tag?> GetTag(Guid id)
        {
            return await dbContext.Tags.SingleOrDefaultAsync(tag => tag.Id == id);
        }

        public async Task<Tag?> GetTagByName(string name)
        {
            return await dbContext.Tags
                .AsNoTracking()
                .SingleOrDefaultAsync(tag => tag.Name == name);
        }
    }
}
