using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemApplication.Common.Interfaces;

namespace RecruitmentSystemInfrastructure.Repositories
{
    public class PositionRepository(AppDbContext dbContext) : IPositionRepository
    {
        public async Task<GetPositionsDTO> GetAll(GetPositionFilter getPositionFilter)
        {
            var query = dbContext.Positions.AsQueryable();

            if (getPositionFilter.title is not null)
            {
                query = query.Where(a => EF.Functions.ILike(a.Title, getPositionFilter.title));
            }

            if (getPositionFilter.description != null)
            {
                query = query.Where(a => EF.Functions.ILike(a.Description ?? "", getPositionFilter.description));
            }

            if (getPositionFilter.isDraft is not null)
            {
                query = query.Where(a => a.IsDraft == getPositionFilter.isDraft);
            }

            if (getPositionFilter.newestFirst is false)
            {
                query = query.OrderByDescending(a => a.UpdatedAt);
            }

            var totalItems = await query.CountAsync();

            var positions = await query
                .AsNoTracking()
                .Include(a => a.Tags)
                .Select(a => new PositionDTO(a.Title, a.Description, a.MaxProjectsCount, a.IsDraft, a.IsPublic, a.Version, a.CreatedAt, a.UpdatedAt, a.Tags))
                .ToListAsync();

            return new GetPositionsDTO(totalItems, positions);
        }
    }
}
