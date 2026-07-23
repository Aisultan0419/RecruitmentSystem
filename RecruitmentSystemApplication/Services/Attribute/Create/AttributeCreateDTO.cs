namespace RecruitmentSystemApplication.Services.Attribute.Create
{
    public record AttributeCreateDTO(string Name, string DataType
        ,string AttributeCategory, ICollection<string> AttributeOptions);
}
