using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUS.Game.Data.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public int PrimaryKey { get; set; }

    public string Username { get; set; }

    public string HashedPassword { get; set; }

    [InverseProperty(nameof(Being.CreatedByUser))]
    public ICollection<Being> CreatedBeings { get; } = new HashSet<Being>();

    public int? PickedBeingPrimaryKey { get; set; }
    public Being? PickedBeing { get; set; }

    public static string HashPassword(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        return hashedPassword;
    }

    public static bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        bool isValid = BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        return isValid;
    }
}
