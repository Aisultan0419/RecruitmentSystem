using RecruitmentSystemDomain.Enums;
namespace RecruitmentSystemDomain.Models
{
    public class User
    {
        public Guid Id { get; init; }
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsBlocked { get; set; } = false;
        public DateTime CreatedAt { get; init; }

        public required UserRole UserRole { get; set; }
        public required UserProfile UserProfile { get; set; }
        public ICollection<CV> CVs { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];
    }
}
