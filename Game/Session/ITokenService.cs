namespace MUS.Game.Session;

public interface ITokenService
{
    string CreateToken(string username);
    
    Task<string?> ValidateToken(string token);
}
