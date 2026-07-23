using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.Attribute.Candidate.Create;
using RecruitmentSystemApplication.Services.Attribute.Create;
using RecruitmentSystemApplication.Services.Attribute.Get;
using RecruitmentSystemApplication.Services.Attribute.Modify;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemApplication.Services.Attribute.Common
{
    public class AttributeService(IAttributeRepository attributeRepository,
        IBaseRepository baseRepository) : IAttributeService
    {
        public async Task<List<string>> GetCategories()
        {
            var categories = await attributeRepository.GetCategories();
            return categories;
        }


        public async Task<Result> CreateAttribute(AttributeCreateDTO attributeCreationDTO)
        {
            var validationrResult = await ValidateAttributeCategory(attributeCreationDTO.AttributeCategory);
            if (validationrResult.IsFailed) return validationrResult.ToResult();
            var normalizedType = attributeCreationDTO.DataType?.Replace(" ", "");
            Enum.TryParse(normalizedType, ignoreCase: true, out DataType result);
            var attributeDefinition = new AttributeDefinition()
            {
                Id = Guid.NewGuid(),
                Name = attributeCreationDTO.Name,
                DataType = result,
                CreatedAt = DateTime.UtcNow,
                AttributeCategory = validationrResult.Value,
                AttributeOptions = attributeCreationDTO.AttributeOptions?
                .Select(optionText => new AttributeOption
                {
                    Id = Guid.NewGuid(),
                    Value = optionText
                }).ToList() ?? []
            };
            await baseRepository.AddItem(attributeDefinition);
            await baseRepository.SaveChanges();
            return Result.Ok();
        } //I should also consider name unique check
        private async Task<Result<AttributeCategory>> ValidateAttributeCategory(string AttributeCategory)
        {
            var category = await attributeRepository.GetCategory(AttributeCategory);
            
            if (category is null)
            {
                return Result.Fail(new Error("Category not found").WithMetadata("StatusCode", 400));
            }
            return Result.Ok(category);
        }

        public async Task<Result<List<AttributeResponseDTO>>> GetAttributes(string? prefix,
            string? category, int page, int pageSize)
        {
            AttributeCategory? attributeCategory = null;
            if (category is not null)
            {
                attributeCategory = await attributeRepository.GetCategory(category);
                if(attributeCategory is null) return Result.Fail(new Error("Category not found").WithMetadata("StatusCode", 400));
            }

            var attributes = await attributeRepository.GetAttributes(attributeCategory, prefix
                , page, pageSize);
            return attributes;
        }

        public async Task DeleteAttributes(List<Guid> attributeIds)
        {
            await baseRepository.DeleteItems<AttributeDefinition>(attributeIds);
        }
        private async Task<Result<AttributeDefinition>> ValidateAttributeExistence(string id)
        {
            var attribute = await attributeRepository.GetAttribute(id);
            if (attribute is null) return Result.Fail(new Error("Attribute not found").WithMetadata("StatusCode", 400));
            return attribute;
        }
        public async Task<Result> ModifyAttribute(AttributeModifyDTO attributeModifyDTO)
        {
            var attribute = await ValidateAttributeExistence(attributeModifyDTO.Id);
            if (attribute.IsFailed) return attribute.ToResult();
            Result<AttributeCategory>? validationResult = null;
            if (attributeModifyDTO.AttributeCategory is not null)
            {
                if (attribute.Value.Version != attributeModifyDTO.Version)
                {
                    return Result.Fail(new Error("The entity you are trying to update has been modified by another user")
                        .WithMetadata("StatusCode", 409));
                }
                validationResult = await ValidateAttributeCategory(attributeModifyDTO.AttributeCategory);
                if (validationResult.IsFailed) return validationResult.ToResult();
            }
            await attributeRepository.ModifyAttribute(attributeModifyDTO, attribute.Value, validationResult?.Value);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }
    }
}
