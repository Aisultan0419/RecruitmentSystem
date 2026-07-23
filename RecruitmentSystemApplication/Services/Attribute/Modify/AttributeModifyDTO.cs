namespace RecruitmentSystemApplication.Services.Attribute.Modify
{
    public record AttributeModifyDTO(
         string Id
        ,string? Name
        ,string? DataType
        ,int Version
        ,string? AttributeCategory
        ,ICollection<string>? AttributeOptions);
}
