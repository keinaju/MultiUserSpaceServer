using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Utilities;
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
    public ICollection<Being> CreatedBeings
    { 
        get => _lazyLoader.Load(this, ref _createdBeings);
        set => _createdBeings = value;
    }

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
    public int? SelectedBeingPrimaryKey { get; set; }
    public Being? SelectedBeing 
    {
        get => _lazyLoader.Load(this, ref _selectedBeing);
        set => _selectedBeing = value;
    }

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
        return BCrypt.Net.BCrypt.HashPassword(
            password,
            workFactor: 12
        );
    }

    /// <summary>
    /// Method to verify if password in user input
    /// matches with hashed password in database.
    /// </summary>
    /// <param name="password">Password in user input (not hashed value).</param>
    /// <returns>True if password in user input is correct, otherwise false.</returns>
    public bool IsCorrectPassword(string password)
    {
        bool isValid = BCrypt.Net.BCrypt.Verify(
            password, HashedPassword
        );
        
        return isValid;
    }

    private readonly ILazyLoader _lazyLoader;
    private Being _selectedBeing;
    private ICollection<Being> _createdBeings;

    public User() {}

    private User(ILazyLoader lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    public List<string> Show()
    {
        var texts = new List<string>();

        texts.Add($"You are {Username}.");
        if(IsBuilder)
        {
            texts.Add("You have access to builder commands.");
        }
        texts.Add(GetSelectedBeingText());
        texts.Add(GetBeingsText());

        return texts;
    }

    private string GetSelectedBeingText()
    {
        if(SelectedBeing is not null)
        {
            return $"Your selected being is {SelectedBeing.Name}.";
        }
        else
        {
            return "You have not selected a being.";
        }
    }

    private string GetBeingsText()
    {
        if(CreatedBeings.Count > 0)
        {
            return $"You have following beings: {GetBeingNames()}.";
        }
        else
        {
            return "You do not have any beings.";
        }
    }

    private string GetBeingNames()
    {
        var beingNames = new List<string>();

        foreach (var being in CreatedBeings)
        {
            beingNames.Add(being.Name!);
        }

        return Message.List(beingNames);
    }
}
