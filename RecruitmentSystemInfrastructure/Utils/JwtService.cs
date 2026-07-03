using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using RecruitmentSystemDomain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RecruitmentSystemApplication.Common.Interfaces;
using System.Text;
namespace RecruitmentSystemInfrastructure.Utils
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };
            var secretKey = _configuration["JWT_SECRET"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "RECRUITMENT_SYSTEM",
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = credentials,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
