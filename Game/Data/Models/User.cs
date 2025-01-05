using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MUS.Game.Commands.CommandResult;

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

    private readonly GameContext _context;
    private readonly ILazyLoader _lazyLoader;
    private Being? _selectedBeing;
    private ICollection<Being> _createdBeings;

    public User() {}

    private User(GameContext context, ILazyLoader lazyLoader)
    {
        _context = context;
        _lazyLoader = lazyLoader;
    }

    public async Task<CommandResult> BreakItem(string itemName)
    {
        if(SelectedBeing is null)
        {
            return UserHasNotSelectedBeing();
        }
        else
        {
            return await SelectedBeing.TryBreakItem(itemName);
        }
    }

    public async Task<CommandResult> CraftItem(string itemName)
    {
        if(SelectedBeing is null)
        {
            return UserHasNotSelectedBeing();
        }
        else
        {
            return await SelectedBeing.TryCraftItem(itemName);
        }
    }

    public async Task<CommandResult> DeleteBeing(string beingName)
    {
        var being = CreatedBeings.SingleOrDefault(
            being => being.Name == beingName
        );

        if (being is not null)
        {
            CreatedBeings.Remove(being);

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(Message.Deleted("being", being.Name));
        }
        else
        {
            return BeingDoesNotExist(beingName);
        }
    }

    public async Task<CommandResult> DeleteFeature(string featureName)
    {
        if(!IsBuilder)
        {
            return UserIsNotBuilder();
        }
        else
        {
            return await _context.DeleteFeature(featureName);
        }
    }

    public async Task<CommandResult> DeleteItem(string itemName)
    {
        if(!IsBuilder)
        {
            return UserIsNotBuilder();
        }
        else
        {
            return await _context.DeleteItem(itemName);
        }
    }

    public async Task<CommandResult> DeleteRoom(string roomName)
    {
        if(!IsBuilder)
        {
            return UserIsNotBuilder();
        }
        else
        {
            return await _context.DeleteRoom(roomName);
        }
    }

    public async Task<CommandResult> DeleteRoomPool(string poolName)
    {
        if(!IsBuilder)
        {
            return UserIsNotBuilder();
        }
        else
        {
            return await _context.DeleteRoomPool(poolName);
        }
    }

    public async Task<CommandResult> DeployItem(string itemName)
    {
        if(SelectedBeing is null)
        {
            return UserHasNotSelectedBeing();
        }
        else
        {
            return await SelectedBeing.DeployItem(itemName);
        }
    }

    public async Task<CommandResult> Explore()
    {
        if(SelectedBeing is null)
        {
            return UserHasNotSelectedBeing();
        }
        else
        {
            return await SelectedBeing.Explore();
        }
    }

    public async Task<CommandResult> Go(string roomName)
    {
        if(SelectedBeing is null)
        {
            return UserHasNotSelectedBeing();
        }
        else
        {
            return await SelectedBeing.Go(roomName);
        }
    }

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

    public async Task<CommandResult> SelectBeing(string beingName)
    {
        var being = CreatedBeings.SingleOrDefault(
            being => being.Name == beingName
        );

        if(being is not null)
        {
            SelectedBeing = being;

            await _context.SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(
                Message.Set($"{Username}'s selected being", SelectedBeing.Name)
            );
        }
        else
        {
            return BeingDoesNotExist(beingName);
        }
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

    public async Task<CommandResult> TakeItem(string itemName)
    {
        if(SelectedBeing is null)
        {
            return UserHasNotSelectedBeing();
        }
        else
        {
            return await SelectedBeing.TakeItemFromRoom(itemName);
        }
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
    
    private CommandResult UserHasNotSelectedBeing()
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage($"User {Username} has not selected a being.");
    }

    private CommandResult UserIsNotBuilder()
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage($"User {Username} does not have a builder role.");
    }
}
