using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PracticeFinal.WebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PracticeFinal.WebAPI.Services
{
    public class AuthService
    {
        public string CreateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var credentials = new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = AuthOptions.ISSUER,
                Audience = AuthOptions.AUDIENCE,
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddMinutes(2),
                Subject = GenerateClaims(user)
                
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(User user)
        {
            var ci = new ClaimsIdentity();

            ci.AddClaim(new Claim("id", user.Id.ToString()));
            ci.AddClaim(new Claim(ClaimTypes.Name, user.Name!));
            ci.AddClaim(new Claim(ClaimTypes.Email, user.Email!));

            foreach (var role in user.Roles!)
                ci.AddClaim(new Claim(ClaimTypes.Role, role));

            return ci;
        }
    }
}
