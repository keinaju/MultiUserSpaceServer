using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MUS.Game.Session;

public class TokenService : ITokenService
{
    string _secret = Environment.GetEnvironmentVariable("MUS_256-BIT_KEY");

    public TokenService() { }

    public string CreateToken(string userId)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_secret)
        );
        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: "MUS",
            claims: new List<Claim>
            {
                new Claim(ClaimTypes.Authentication, userId),
            },
            notBefore: null,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //Return user id if token is valid, otherwise null
    public async Task<string?> ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MUS",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_secret)
            )
        };
        var result = await handler.ValidateTokenAsync(token, parameters);
        if (result.IsValid)
        {
            return result.Claims[ClaimTypes.Authentication].ToString();
        }
        else
        {
            return null;
        }
    }
}
