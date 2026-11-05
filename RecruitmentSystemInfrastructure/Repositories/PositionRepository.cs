using FluentResults;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Attribute.Common;
using RecruitmentSystemApplication.Services.Positions.Get;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemInfrastructure.Repositories
{
    public class PositionRepository(AppDbContext dbContext) : IPositionRepository
    {
        public async Task<GetPositionsDTO> GetAll(GetPositionFilter getPositionFilter)
        {
            var query = dbContext.Positions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(getPositionFilter.Title))
            {
                query = query.Where(a => EF.Functions.ILike(a.Title, $"{getPositionFilter.Title.Trim()}%"));
            }

            if (!string.IsNullOrWhiteSpace(getPositionFilter.Description))
            {
                query = query.Where(a => EF.Functions.ILike(a.Description!, $"{getPositionFilter.Description.Trim()}%"));
            }

            if (getPositionFilter.IsDraft is not null)
            {
                query = query.Where(a => a.IsDraft == getPositionFilter.IsDraft);
            }

            if (getPositionFilter.NewestFirst is true)
            {
                query = query.OrderByDescending(a => a.CreatedAt);
            }
            else
            {
                query = query.OrderBy(a => a.CreatedAt);
            }

            var totalItems = await query.CountAsync();

            var positions = await query
                .AsNoTracking()
                .Include(a => a.Tags)
                .Select(a => new PositionDTO(a.Id, a.Title, a.Description, a.MaxProjectsCount, a.IsDraft, a.IsPublic, a.Version, a.CreatedAt, a.UpdatedAt, a.Tags))
                .Skip((getPositionFilter.Page - 1) * getPositionFilter.PageSize)
                .Take(getPositionFilter.PageSize)
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
        public async Task<Position?> GetPositionWithTags(Guid id)
        {
            return await dbContext.Positions
                .Include(position => position.Tags)
                .SingleOrDefaultAsync(position => position.Id == id);
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

            await dbContext.PositionAccessRules
                .Where(r => r.PositionId == positionId && r.AttributeDefinitionId == attributeId)
                .ExecuteDeleteAsync();

            return Result.Ok();
        }

        public async Task<Result<List<PositionAccessRule>>> GetAccessRules(Guid positionId)
        {
            var result = await dbContext.PositionAccessRules
                .Where(a => a.PositionId == positionId)
                .ToListAsync();

            return Result.Ok(result);
        }

        public async Task<List<AttributeResponseDTO>> GetPositionAttributes(Guid positionId)
        {
            var positionAttributes = await dbContext.PositionAttributes.Where(a => a.PositionId == positionId).ToListAsync();

            var attributes = await dbContext.AttributeDefinitions
                .Where(a => positionAttributes.Select(pa => pa.AttributeDefinitionId).Contains(a.Id))
                .AsNoTracking()
                .Select(a => new AttributeResponseDTO(
                    a.Id,
                    a.Name,
                    a.DataType.ToString(),
                    a.AttributeCategory.Name,
                    a.Version,
                    a.AttributeOptions.Select(o => o.Value).ToList()
                ))
                .ToListAsync();

            return attributes;
        }
    }
}