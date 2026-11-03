using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemInfrastructure.Repositories
{
    public class CandidateProjectRepository(AppDbContext dbContext) : ICandidateProjectRepository
    {
        public async Task<List<CandidateProject>> GetCandidateProjects(string? prefix)
        {
            var query = dbContext.CandidateProjects
                .AsNoTracking()
                .Include(project => project.UserProfile)
                .Include(project => project.Tags)
                .AsQueryable();

            if (prefix is not null)
                query = query.Where(project => EF.Functions.ILike(project.Name, $"{prefix}%"));

            return await query
                .OrderBy(project => project.Name)
                .Take(10)
                .ToListAsync();
        }

        public async Task<CandidateProject?> GetCandidateProject(Guid id)
        {
            return await dbContext.CandidateProjects
                .Include(project => project.UserProfile)
                .Include(project => project.Tags)
                .SingleOrDefaultAsync(project => project.Id == id);
        }
    }
}
