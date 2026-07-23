
namespace RecruitmentSystemDomain.Models
{
    public class Position
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int MaxProjectsCount { get; set; }

        public bool IsPublic { get; set; }
        public int Version { get; set; } = 0;                 
        public DateTime CreatedAt { get; set; }             
        public DateTime? UpdatedAt { get; set; }

        public ICollection<CV> CVs { get; set; } = [];
        public ICollection<Tag> Tags { get; set; } = [];
        public ICollection<PositionAttribute> PositionAttributes { get; set; } = [];
        public ICollection<PositionAccessRule> AccessRules { get; set; } = [];
        public ICollection<DiscussionPost> DiscussionPosts { get; set; } = [];
    }
}
