
using FluentResults;

namespace RecruitmentSystemApplication.Services.Auth.Login
{
    public interface ILoginService
    {
        Task<Result<string>> LoginUserAsync(LoginQueryDTO loginDTO);
    }
}
