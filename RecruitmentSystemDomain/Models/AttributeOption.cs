namespace RecruitmentSystemDomain.Models
{
    public class AttributeOption
    {
        public Guid Id { get; set; }
        public Guid AttributeDefinitionId { get; set; }
        public required string Value { get; set; }
    }
}
