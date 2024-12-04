using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUS.Game.Data.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public int PrimaryKey { get; set; }

    /// <summary>
    /// Beings belonging to this user.
    /// One of these beings can be selected to play the game.
    /// </summary>
    [InverseProperty(nameof(Being.CreatedByUser))]
    public ICollection<Being> CreatedBeings { get; }
        = new HashSet<Being>();

    /// <summary>
    /// Password after hash function to be stored in database.
    /// </summary>
    public required string HashedPassword { get; set; }

    /// <summary>
    /// Role to determine if user can build the game world.
    /// </summary>
    public required bool IsBuilder { get; set; }

    /// <summary>
    /// Selected being to play the game.
    /// User can have only one selected being.
    /// </summary>
    public int? PickedBeingPrimaryKey { get; set; }
    public Being? PickedBeing { get; set; }

    /// <summary>
    /// User name to store in database.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Static method for a hash function.
    /// </summary>
    /// <param name="password">Password in user input.</param>
    /// <returns>Hashed password to store in database.</returns>
    public static string HashPassword(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt
            .HashPassword(password, workFactor: 12);
        return hashedPassword;
    }

    /// <summary>
    /// Static method to verify if password in user input
    /// matches with hashed password in database.
    /// </summary>
    /// <param name="inputPassword">Password in user input (not hashed value).</param>
    /// <param name="hashedPassword">Password in database (hashed value).</param>
    /// <returns>True if password in user input is correct, otherwise false.</returns>
    public static bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        bool isValid = BCrypt.Net.BCrypt.Verify(
            inputPassword, hashedPassword
        );
        return isValid;
    }
}
