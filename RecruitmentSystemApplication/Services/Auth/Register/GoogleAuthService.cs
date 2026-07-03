using RecruitmentSystemApplication.Contracts;
using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
namespace RecruitmentSystemApplication.Services.Auth.Register
{
    public class GoogleAuthService(
         IGoogleValidationService googleValidationService
        ,IRegistrationService registrationService
        ,IUserRepository userRepository
        ,IJwtService jwtService) : IGoogleAuthService
    {
        public async Task<string> GoogleAuthAsync(string googleClientToken)
        {
            var userPayload = await googleValidationService.ValidateGoogleToken(googleClientToken); //It can be null, I handle it in global ex handler

            var user = await userRepository.FindUser(userPayload!.email);
            if (user is null)
            {
                //if first name and second name is empty I will do on boarding step to get client's display name later
                var userRegisterDto = new UserRegisterDTO(Email: userPayload!.email
                    , plainPassword: null
                    , FirstName: userPayload.firstName
                    , LastName: userPayload.lastName
                    , Location: null);
                var registeredUser = await registrationService.RegisterAsync(userRegisterDto);
                return jwtService.GenerateToken(registeredUser);
            }
            return jwtService.GenerateToken(user);
        }
    }
}
