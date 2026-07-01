using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemDomain.Enums;
using RecruitmentSystemDomain.Models;
using RecruitmentSystemApplication.Common.ResultWrapper;
using RecruitmentSystemApplication.Contracts;

namespace RecruitmentSystemApplication.Services.Auth.Register
{
    public class RegistrationService(
        IBaseRepository _baseRepository,
        IHashingService _hashService
    ) : IRegistrationService
    {
        public async Task<Result<string>> RegisterAsync(UserRegisterDTO userRegisterDTO)
        {
            User user = new User()
            {
                Id = Guid.NewGuid(),
                Email = userRegisterDTO.Email,
                PasswordHash = _hashService.HashPassword(userRegisterDTO.plainPassword),
                CreatedAt = DateTime.UtcNow,
                UserRole = UserRole.Candidate,
                UserProfile = new UserProfile()
                {
                    Id = Guid.NewGuid(),
                    FirstName = userRegisterDTO.FirstName,
                    LastName = userRegisterDTO.LastName ?? string.Empty,
                    PhotoUrl = string.Empty, //user puts it in app, not in registration window
                    Location = userRegisterDTO.Location ?? string.Empty,
                }
            };

            await _baseRepository.AddItem<User>(user);
            await _baseRepository.SaveChanges();
            return Result<string>.Success(user.Id.ToString());
        }


    }
}
