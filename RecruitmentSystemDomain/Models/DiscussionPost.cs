
namespace RecruitmentSystemDomain.Models
{
    public class DiscussionPost
    {
        public Guid Id { get; set; }
        public Guid PositionId { get; set; }        
        public required Position Position { get; set; }       

        public Guid AuthorId { get; set; }      
        public required User Author { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
