using Microsoft.EntityFrameworkCore;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Models;
using RecruitmentSystemApplication.Services.Profile;
using FluentResults;
namespace RecruitmentSystemInfrastructure.Repositories
{
    public class UserRepository(AppDbContext _context) : IUserRepository
    {
        public async Task<User?> FindUser(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            return user;
        }
        public async Task<UserProfile?> FindUserProfile(Guid id)
        {
            var user = await _context.Users.Include(a => a.UserProfile).ThenInclude(a => a.CandidateAttributeValues).SingleOrDefaultAsync(u => u.Id == id);
            if (user is null) return null;
            var userProfile = user.UserProfile;
            return userProfile;
        }

        public async Task<Result> UpdateProfile(string userId, UserProfileDTO userProfileDTO)
        {
            Guid.TryParse(userId, out Guid result);
            var userProfile = await FindUserProfile(result);
            if (userProfile is null) return Result.Fail("User was not found");
            userProfile.FirstName = string.IsNullOrWhiteSpace(userProfileDTO.FirstName)
                ? userProfile.FirstName
                : userProfileDTO.FirstName;

            userProfile.LastName = string.IsNullOrWhiteSpace(userProfileDTO.LastName)
                ? userProfile.LastName
                : userProfileDTO.LastName;

            userProfile.Location = string.IsNullOrWhiteSpace(userProfileDTO.Location)
                ? userProfile.Location
                : userProfileDTO.Location;

            userProfile.Version = userProfileDTO.Version + 1;
            return Result.Ok();
        }
    }
}
