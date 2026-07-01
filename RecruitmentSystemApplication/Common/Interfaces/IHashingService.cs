
namespace RecruitmentSystemApplication.Common.Interfaces
{
    public interface IHashingService
    {
        string HashPassword(string plainPassword);
        bool VerifyPassword(string plainPassword, string passwordHash);
    }
}
