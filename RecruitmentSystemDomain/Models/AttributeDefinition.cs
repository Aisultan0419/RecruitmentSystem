
using System.ComponentModel.DataAnnotations;

namespace RecruitmentSystemDomain.Models
{
    public class AttributeDefinition
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public DataType DataType { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<AttributeOption> AttributeOptions { get; set; } = [];
    }
}
