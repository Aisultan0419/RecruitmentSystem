namespace RecruitmentSystemApplication.Services.Attribute.Common
{
    public record AttributeResponseDTO(
        Guid Id,
        string Name,
        string DataType,
        string CategoryName, 
        int Version,
        ICollection<string> AttributeOptions 
);
}
