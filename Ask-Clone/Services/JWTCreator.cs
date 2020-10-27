using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ask_Clone.Services
{
    public class JWTCreator
    {
        private readonly JWTSettings _settings;

        public JWTCreator(JWTSettings settings)
        {
            _settings = settings;
        }

        public string GenerateToken(string username)
        {   
            var key = Environment.GetEnvironmentVariable("Token");
            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                            {
                            new Claim("UserName", username)
                            }),
                Expires = DateTime.Now.AddHours(_settings.Expire),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                            SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDecriptor);
            
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
