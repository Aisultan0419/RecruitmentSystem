using RecruitmentSystemApplication.Common.ResultWrapper;
using RecruitmentSystemApplication.Services.Auth.Register;

namespace RecruitmentSystemApplication.Contracts
{
    public interface IRegistrationService
    {
        Task<Result<string>> RegisterAsync(UserRegisterDTO userRegisterDTO);
    }
}
