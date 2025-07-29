using fd.infrastructure.entity.SysModels;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Utilities
{
    public class JwtHelper
    {
        protected readonly IConfiguration Configuration;

        private static readonly string JwtKey = "findingdata.cn";



        public static string IssueJwt(JwtUser userInfo)
        {
            var payload = new Dictionary<string, object>()
            {
                { "user_id" , userInfo.user_id },
                { "user_name" , userInfo.user_name },
                { "display_name" , userInfo.display_name },
            };
            IJwtAlgorithm algorithm = new RS256Algorithm(certificate);
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            const string key = null; // not needed if algorithm is asymmetric

            var token = encoder.Encode(payload, key);
            //IJwtAlgorithm algorithm 
        }

        /// <summary>
        /// 生成JWT
        /// </summary>
        /// <param name="serInfo"></param>
        /// <returns></returns>
        //    public static string IssueJwt(UserInfo userInfo)
        //    {
        //        string exp = $"{new DateTimeOffset(DateTime.Now.AddMinutes(ManageUser.UserContext.MenuType == 1 ? 43200 : AppSetting.ExpMinutes)).ToUnixTimeSeconds()}";
        //        var claims = new List<Claim>
        //            {

        //            new Claim(JwtRegisteredClaimNames.Jti,userInfo.User_Id.ToString()),
        //            new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
        //            new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,

        //            new Claim (JwtRegisteredClaimNames.Exp,exp),
        //            new Claim(JwtRegisteredClaimNames.Iss,AppSetting.Secret.Issuer),
        //            new Claim(JwtRegisteredClaimNames.Aud,AppSetting.Secret.Audience),
        //           };

        //        //秘钥16位
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSetting.Secret.JWT));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //        JwtSecurityToken securityToken = new JwtSecurityToken(issuer: AppSetting.Secret.Issuer, claims: claims, signingCredentials: creds);
        //        string jwt = new JwtSecurityTokenHandler().WriteToken(securityToken);
        //        return jwt;
        //    }

        //    /// <summary>
        //    /// 解析
        //    /// </summary>
        //    /// <param name="jwtStr"></param>
        //    /// <returns></returns>
        //    public static UserInfo SerializeJwt(string jwtStr)
        //    {
        //        var jwtHandler = new JwtSecurityTokenHandler();
        //        JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
        //        UserInfo userInfo = new UserInfo
        //        {
        //            User_Id = Convert.ToInt32(jwtToken.Id),
        //            Role_Id = (jwtToken.Payload[ClaimTypes.Role] ?? 0).GetInt(),
        //            UserName = jwtToken.Payload[ClaimTypes.Name]?.ToString()
        //        };
        //        return userInfo;
        //    }
        //    /// <summary>
        //    /// 获取过期时间
        //    /// </summary>
        //    /// <param name="jwtStr"></param>
        //    /// <returns></returns>
        //    public static DateTime GetExp(string jwtStr)
        //    {
        //        var jwtHandler = new JwtSecurityTokenHandler();
        //        JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

        //        DateTime expDate = (jwtToken.Payload[JwtRegisteredClaimNames.Exp] ?? 0).GetInt().GetTimeSpmpToDate();
        //        return expDate;
        //    }
        //    public static bool IsExp(string jwtStr)
        //    {
        //        return GetExp(jwtStr) < DateTime.Now;
        //    }

        //    public static int GetUserId(string jwtStr)
        //    {
        //        try
        //        {
        //            if (jwtStr.IsNullOrEmpty()) return 0;
        //            jwtStr = jwtStr.Replace("Bearer ", "");
        //            return new JwtSecurityTokenHandler().ReadJwtToken(jwtStr).Id.GetInt();
        //        }
        //        catch
        //        {
        //            return 0;
        //        }
        //    }
        }
}
