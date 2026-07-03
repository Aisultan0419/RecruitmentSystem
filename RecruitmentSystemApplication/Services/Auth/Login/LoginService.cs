using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
namespace RecruitmentSystemApplication.Services.Auth.Login
{
    public class LoginService(IHashingService hashingService
        ,IUserRepository userRepository
        ,IJwtService jwtService) : ILoginService
    {
        public async Task<Result<string>> LoginUserAsync(LoginQueryDTO loginDTO)
        {
            var user = await userRepository.FindUser(loginDTO.Email);
            if (user is null)
            {
                return Result.Fail(
                        new Error("Unauthorized").WithMetadata("StatusCode", 401)
                    );
            }
            if (user.IsBlocked is true)
            {
                return Result.Fail(
                        new Error("Your account has been blocked").WithMetadata("StatusCode", 403)
                    );
            }
            if (user.PasswordHash is null)
            {
                return Result.Fail(""); //i should handle it later
            }
            var isPasswordValid = hashingService.VerifyPassword(loginDTO.Password, user.PasswordHash!);
            if (isPasswordValid is false)
            {
                return Result.Fail(
                        new Error("Unauthorized").WithMetadata("StatusCode", 401)
                    );
            }
            string token = jwtService.GenerateToken(user);
            return Result.Ok<string>(token);
        }
    }
}
