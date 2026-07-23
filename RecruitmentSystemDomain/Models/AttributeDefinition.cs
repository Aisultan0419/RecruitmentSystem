using RecruitmentSystemDomain.Enums;

namespace RecruitmentSystemDomain.Models
{
    public class AttributeDefinition
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DataType DataType { get; set; }
        public int Version { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public required AttributeCategory AttributeCategory { get; set; }
        public Guid AttributeCategoryId { get; set; }
        public ICollection<AttributeOption> AttributeOptions { get; set; } = [];
    }
}
