using fd.infrastructure.core.Utilities;
using fd.infrastructure.entity.SysModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Auth
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtOptions _options;

        public TokenGenerator(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(UserContext user)
        {
            string exp = $"{new DateTimeOffset(DateTime.Now.AddMinutes(_options.ExpireMinutes)).ToUnixTimeSeconds()}";
            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier,user.user_id.ToString()),
                new Claim(ClaimTypes.Name,user.user_name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // 设置唯一标识符
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),                
                //默认设置jwt过期时间120分钟
                new Claim (JwtRegisteredClaimNames.Exp,exp),
                new Claim(JwtRegisteredClaimNames.Iss,_options.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud,_options.Audience),
               };

            //秘钥16位
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                   issuer: _options.Issuer,
                   audience: _options.Audience,
                   claims: claims,
                   expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes),
                   signingCredentials: creds
               );
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
