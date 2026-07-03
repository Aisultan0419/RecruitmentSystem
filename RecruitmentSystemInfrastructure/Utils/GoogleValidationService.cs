using Google.Apis.Auth;
using RecruitmentSystemApplication.Services.Auth.Register;

namespace RecruitmentSystemInfrastructure.Utils
{
    public class GoogleValidationService : IGoogleValidationService
    {
        public async Task<GoogleUserPayload?> ValidateGoogleToken(string googleClientToken)
        {
            //i should change it later client id is hardcoded
            const string GOOGLE_CLIENT_ID = "684390088485-7m1i4sbd91l80momqf8ekjcf78s0rvm2.apps.googleusercontent.com";
            var payload = await VerifyGoogleToken(googleClientToken, GOOGLE_CLIENT_ID);

            GoogleUserPayload userPayload = new(
                payload!.GivenName ?? string.Empty,
                payload.FamilyName ?? string.Empty,
                payload.Picture ?? string.Empty,
                payload.Email
            );
            return userPayload;
        }
        private static async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string token, string expectedClientId)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { expectedClientId }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
    }
}
