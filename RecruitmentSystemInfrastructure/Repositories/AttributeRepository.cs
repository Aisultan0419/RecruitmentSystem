using FluentResults;
using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Attribute.Common;
using RecruitmentSystemApplication.Services.Attribute.Get;
using RecruitmentSystemApplication.Services.Attribute.Modify;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemInfrastructure.Repositories
{
    public class AttributeRepository(AppDbContext dbContext) : IAttributeRepository
    {
        public async Task<List<string>> GetCategories()
        {
            var result = await dbContext.AttributeCategories
                .AsNoTracking()
                .Select(n => n.Name)
                .Distinct()
                .ToListAsync();
            return result;
        }
        public async Task<AttributeCategory?> GetCategory(string catergoryName)
        {
            var result = await dbContext.AttributeCategories
                .SingleOrDefaultAsync(a => a.Name.Trim().ToLower() == catergoryName.Trim().ToLower());
            return result;
        }
        public async Task<List<AttributeResponseDTO>> GetAttributes(AttributeCategory? attributeCategory, string? prefix
            ,int page, int pageSize)
        {
            var query = dbContext.AttributeDefinitions.AsQueryable();

            if (attributeCategory is not null)
            {
                query = query.Where(a => a.AttributeCategoryId == attributeCategory.Id);
            }

            if (prefix is not null)
            {
                query = query.Where(a => a.Name.StartsWith(prefix));
            }

            var result = await query.AsNoTracking()
                .Include(a => a.AttributeCategory)
                .Include(a => a.AttributeOptions)
                .Select(a => new AttributeResponseDTO(
                    a.Id, a.Name, a.DataType.ToString(), a.AttributeCategory.Name, a.Version,
                    a.AttributeOptions.Select(o => o.Value).ToList()
                    ))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return result;
        }
        public async Task<AttributeDefinition?> GetAttribute(string Id)
        {
            Guid.TryParse(Id, out Guid parsedId); //i should handle later format of guid 
            var attribute = await dbContext.AttributeDefinitions
                .Include(a => a.AttributeOptions)
                .SingleOrDefaultAsync(a => a.Id == parsedId);
            return attribute;
        }

        public async Task ModifyAttribute(AttributeModifyDTO attributeModifyDTO
            ,AttributeDefinition attributeDefinition
            ,AttributeCategory? category)
        {
            var normalizedType = attributeModifyDTO.DataType?.Replace(" ", "");
            Enum.TryParse(normalizedType, ignoreCase: true, out DataType result);

            attributeDefinition.Name = attributeModifyDTO.Name ?? attributeDefinition.Name;
            attributeDefinition.DataType = attributeModifyDTO.DataType is null ? attributeDefinition.DataType : result;
            attributeDefinition.AttributeCategory = category ?? attributeDefinition.AttributeCategory;
            if (attributeModifyDTO.AttributeOptions is not null)
            {
                dbContext.AttributeOptions.RemoveRange(attributeDefinition.AttributeOptions);
                foreach (var optionText in attributeModifyDTO.AttributeOptions)
                {
                    var newOption = new AttributeOption
                    {
                        Id = Guid.NewGuid(),
                        Value = optionText,
                        AttributeDefinitionId = attributeDefinition.Id
                    };
                    dbContext.AttributeOptions.Add(newOption);
                }
            }
            attributeDefinition.Version = attributeModifyDTO.Version + 1;
        }
    }
}
