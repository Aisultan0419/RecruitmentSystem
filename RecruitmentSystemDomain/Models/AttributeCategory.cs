using System.Text.Json.Serialization;

namespace RecruitmentSystemDomain.Models
{
    public class AttributeCategory
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }

        [JsonIgnore]
        public ICollection<AttributeDefinition>? AttributeDefinitions { get; set; }
    }
}
