using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;
using RecruitmentSystemApplication.Services.Profile;
namespace RecruitmentSystemInfrastructure.Repositories
{
    public class UserRepository(AppDbContext _context) : IUserRepository
    {
        public async Task<User?> FindUser(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            return user;
        }
        public async Task<UserProfile> FindUserProfile(Guid id)
        {
            var user = await _context.Users.Include(a => a.UserProfile).SingleOrDefaultAsync(u => u.Id == id) ?? throw new NullReferenceException("User is not found");
            var userProfile = user.UserProfile;
            return userProfile;
        }
    }
}
