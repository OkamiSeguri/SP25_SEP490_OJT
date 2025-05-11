using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using BusinessObject;
using System.Security.Cryptography;

namespace Services
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("UserName", user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMonths(int.Parse(jwtSettings["ExpiryInMonths"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        //public string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //    }
        //    return Convert.ToBase64String(randomNumber);
        //}

        //public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        //{
        //    var jwtSettings = _configuration.GetSection("JwtSettings");
        //    var secretKey = jwtSettings["SecretKey"];
        //    var key = Encoding.UTF8.GetBytes(secretKey);

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateLifetime = false, 
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = jwtSettings["Issuer"],
        //        ValidAudience = jwtSettings["Audience"],
        //        IssuerSigningKey = new SymmetricSecurityKey(key)
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    try
        //    {
        //        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        //        var jwtSecurityToken = securityToken as JwtSecurityToken;
        //        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            throw new SecurityTokenException("Invalid token");
        //        }
        //        return principal;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}
