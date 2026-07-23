
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystemApplication.Services.Attribute.Create;
using RecruitmentSystemApplication.Services.Attribute.Get;
using RecruitmentSystemApplication.Services.Attribute.Modify;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.Attribute.Common
{
    public interface IAttributeService
    {
        Task<List<string>> GetCategories();
        Task<Result> CreateAttribute(AttributeCreateDTO attributeCreationDTO);
        Task<Result<List<AttributeResponseDTO>>> GetAttributes(string? prefix,
            string? category,int page ,int pageSize);
        Task DeleteAttributes(List<Guid> attributeIds);
        Task<Result> ModifyAttribute(AttributeModifyDTO attributeModifyDTO);
    }
}
