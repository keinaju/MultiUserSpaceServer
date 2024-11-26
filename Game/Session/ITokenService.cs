namespace MUS.Game.Session;

public interface ITokenService
{
    string CreateToken(string userId);
    Task<string?> ValidateToken(string token);
}
