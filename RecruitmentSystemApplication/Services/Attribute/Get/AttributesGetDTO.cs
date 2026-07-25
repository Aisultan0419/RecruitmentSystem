
using RecruitmentSystemApplication.Services.Attribute.Common;

namespace RecruitmentSystemApplication.Services.Attribute.Get
{
    public record AttributesGetDTO(int TotalItems, List<AttributeResponseDTO> AttributeResponseDTO);
}
