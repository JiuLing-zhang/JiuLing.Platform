using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JiuLing.Platform.Common.Services;
public class JwtTokenService(IConfiguration configuration)
{
    private readonly byte[] _jwtKey = Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ?? throw new Exception("JWT 验证未正确配置"));
    private const int ExpiresDay = 7;

    public string GenerateToken(JwtUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim( ClaimTypes.NameIdentifier  , user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("AvatarUrl", user.AvatarUrl),
                new Claim("CreateTime", user.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")),
            ]),
            Expires = DateTime.Now.AddDays(ExpiresDay),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string? token)
    {
        if (token == null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_jwtKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string RenewToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // 延长 Token 有效期
        var newTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(jwtToken.Claims),
            Expires = DateTime.Now.AddDays(ExpiresDay),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var newToken = tokenHandler.CreateToken(newTokenDescriptor);
        return tokenHandler.WriteToken(newToken);
    }
}