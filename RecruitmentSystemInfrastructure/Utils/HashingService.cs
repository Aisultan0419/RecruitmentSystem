using RecruitmentSystemApplication.Common.Interfaces;
namespace RecruitmentSystemInfrastructure.Utils
{
    public class HashingService : IHashingService
    {
        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(plainPassword);
        }
        public bool VerifyPassword(string plainPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(plainPassword, passwordHash);
        }
    }
}
