using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemApplication.Common.Interfaces;
using FluentResults;
using RecruitmentSystemDomain.Models;
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

        public async Task<Result<PositionAttribute>> AddAttribute(Guid attributeId, Guid positionId)
        {
            var position = await dbContext.Positions.FindAsync(positionId);
            if (position == null)
            {
                return Result.Fail("Position not found");
            }

            var attribute = await dbContext.AttributeDefinitions.FindAsync(attributeId);
            if (attribute == null)
            {
                return Result.Fail("Attribute not found");
            }

            var positionAttribute = new PositionAttribute
            {
                Id = Guid.NewGuid(),
                PositionId = positionId,
                AttributeDefinitionId = attributeId
            };
            return Result.Ok(positionAttribute);
        }

        public async Task<Position?> GetPosition(Guid id)
        {
            var position = await dbContext.Positions.FindAsync(id);
            return position;
        }
        public async Task<PositionAccessRule?> GetAccessRule(Guid id)
        {
            var accessRule = await dbContext.PositionAccessRules.FindAsync(id);
            return accessRule;
        }

        public async Task<Result> DeleteAttribute(Guid positionId, Guid attributeId)
        {
            var deleted = await dbContext.PositionAttributes
                .Where(a => a.AttributeDefinitionId == attributeId && a.PositionId == positionId)
                .ExecuteDeleteAsync();

            if (deleted == 0)
                return Result.Fail("Attribute was not found in this position");

            return Result.Ok();
        }

        public async Task<Result<List<PositionAccessRule>>> GetAccessRules(Guid positionId)
        {
            var result = await dbContext.PositionAccessRules
                .Where(a => a.PositionId == positionId)
                .ToListAsync();

            return Result.Ok(result);
        }
    }
}