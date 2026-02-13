using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NZWalks.API.Repositories
{
    public class SQLTokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;

        public SQLTokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateTWTToken(IdentityUser identityUser, List<string> roles)
        {
            // Create claims based on the user information and roles
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, identityUser.Email),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Generate JWT token using the claims and return it
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
