using RecruitmentSystemApplication.Services.Attribute.Common;
using RecruitmentSystemApplication.Services.Attribute.Get;
using RecruitmentSystemApplication.Services.Attribute.Modify;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IAttributeRepository
    {
        Task<List<string>> GetCategories();
        Task<AttributeCategory?> GetCategory(string catergoryName);
        Task<List<AttributeResponseDTO>> GetAttributes(AttributeCategory? attributeCategory, string? prefix, int page, int pageSize);
        Task<AttributeDefinition?> GetAttribute(string Id);
        Task ModifyAttribute(AttributeModifyDTO attributeModifyDTO
            , AttributeDefinition attributeDefinition
            , AttributeCategory? category);
    }
}
