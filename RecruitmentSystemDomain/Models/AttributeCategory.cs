namespace RecruitmentSystemDomain.Models
{
    public class AttributeCategory
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }
        public ICollection<AttributeDefinition>? AttributeDefinitions { get; set; }
    }
}
