using Kemet.Core.Entities.Identity;
using Kemet.Core.Services.InterFaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Kemet.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;

        public TokenServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> _userManager)
        {
            var AuthClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.GivenName, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
        };

            var UserRoles = await _userManager.GetRolesAsync(user);

            foreach (var Role in UserRoles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            var key = _configuration["JWT:Key"];
            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var experieDays = double.Parse(_configuration["JWT:DurationDays"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: AuthClaims,
                expires: DateTime.Now.AddDays(experieDays),
                signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
