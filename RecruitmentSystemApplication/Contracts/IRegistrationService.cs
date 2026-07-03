using RecruitmentSystemApplication.Services.Auth.Register;
using FluentResults;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemApplication.Contracts
{
    public interface IRegistrationService
    {
        Task<User> RegisterAsync(UserRegisterDTO userRegisterDTO);
    }
}
