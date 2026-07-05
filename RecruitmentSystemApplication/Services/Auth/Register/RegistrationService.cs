using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;
using RecruitmentSystemApplication.Contracts;
using FluentResults;
namespace RecruitmentSystemApplication.Services.Auth.Register
{
    public class RegistrationService(
        IBaseRepository _baseRepository,
        IHashingService _hashService
    ) : IRegistrationService
    {
        public async Task<User> RegisterAsync(UserRegisterDTO userRegisterDTO)
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Email = userRegisterDTO.Email,
                PasswordHash = userRegisterDTO.plainPassword is not null ? _hashService.HashPassword(userRegisterDTO.plainPassword) : null,
                CreatedAt = DateTime.UtcNow,
                UserRole = UserRole.Candidate,
                UserProfile = new UserProfile()
                {
                    Id = Guid.NewGuid(),
                    FirstName = userRegisterDTO.FirstName ?? string.Empty,
                    LastName = userRegisterDTO.LastName ?? string.Empty,
                    PhotoUrl = string.Empty, //user puts it in app, not in registration window
                    Location = userRegisterDTO.Location ?? string.Empty,
                }
            };

            await _baseRepository.AddItem<User>(user);
            await _baseRepository.SaveChanges();
            return user;
        }


    }
}
