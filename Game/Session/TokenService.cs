using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MUS.Game.Session;

public class TokenService : ITokenService
{
    private const string CLAIM_NAME = "username";
    private const string SECRET_ENVIRONMENT_VARIABLE_NAME = "MUS_256-BIT_KEY";

    private string _secret;

    public TokenService()
    {
        var secret = Environment.GetEnvironmentVariable(SECRET_ENVIRONMENT_VARIABLE_NAME);
        if(secret is not null)
        {
            _secret = secret;
        }
        else
        {
            throw new Exception($"Environment variable {SECRET_ENVIRONMENT_VARIABLE_NAME} is not defined.");
        }
    }

    /// <summary>
    /// Creates a new token.
    /// </summary>
    /// <param name="username">Username to write in the token.</param>
    /// <returns>A new token with a username in the payload.</returns>
    public string CreateToken(string username)
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
            claims: new List<Claim> { new Claim(CLAIM_NAME, username) },
            notBefore: null,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates the token.
    /// </summary>
    /// <param name="token">Token string to validate.</param>
    /// <returns>Username if token is valid, otherwise null.</returns>
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
            return result.Claims[CLAIM_NAME].ToString();
        }
        else
        {
            return null;
        }
    }
}
