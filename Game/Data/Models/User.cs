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

    public async Task<CommandResult> CuriosityIs(string poolName)
    {
        if(IsBuilder) 
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.CuriosityIs(poolName);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
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

    public async Task<CommandResult> DeploymentIs(Item item)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await item.SetDeployment(SelectedBeing);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
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

    public async Task<CommandResult> FeatureNameIs(
        string oldFeatureName, string newFeatureName
    )
    {
        if(!IsBuilder)
        {
            return UserIsNotBuilder();
        }

        var feature = await _context.FindFeature(oldFeatureName);
        if(feature is not null)
        {
            return await feature.Rename(newFeatureName);
        }
        else
        {
            return FeatureDoesNotExist(oldFeatureName);
        }
    }

    public List<string> GetDetails()
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

    public async Task<CommandResult> ItemDescriptionIs(
        string itemName, string itemDescription
    )
    {
        if(IsBuilder)
        {
            var item = await _context.FindItem(itemName);
            if(item is not null)
            {
                return await item.DescriptionIs(itemDescription);
            }
            else
            {
                return ItemDoesNotExist(itemName);
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> ItemHatcherIntervalIs(
        Item item, int interval
    )
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.InRoom
                .ItemHatcherIntervalIs(item, interval);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> ItemHatcherQuantityIs(
        Item item, int minQuantity, int maxQuantity
    )
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.InRoom
                .ItemHatcherQuantityIs(item, minQuantity, maxQuantity);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> ItemIsMadeOf(
        Item product, Item component, int quantity
    )
    {
        if(IsBuilder)
        {
            return await product.SetComponent(component, quantity);
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> ItemNameIs(
        Item item, string newName
    )
    {
        if(IsBuilder)
        {
            return await item.Rename(newName);
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> MakeItems(Item item, int quantity)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.MakeItems(item, quantity);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> NewBeing(string beingName)
    {
        return await _context.CreateBeing(this, beingName);
    }

    public async Task<CommandResult> NewFeature(string featureName)
    {
        if(IsBuilder)
        {
            return await _context.CreateFeature(featureName);
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> NewItem(string itemName)
    {
        if(IsBuilder)
        {
            return await _context.CreateItem(itemName);
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> NewItemHatcher(Item item)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.InRoom.CreateItemHatcher(item);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> NewRoom(string roomName)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await _context.CreateRoom(
                    inputName: roomName,
                    being: SelectedBeing
                );
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> NewRoomPool(string poolName)
    {
        if(IsBuilder)
        {
            return await _context.CreateRoomPool(poolName);
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomDescriptionIs(string roomDescription)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.InRoom
                .RoomDescriptionIs(roomDescription);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomIsFor(Feature feature)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.InRoom.RoomIsFor(feature);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomIsGlobal(bool newValue)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.InRoom.RoomIsGlobal(newValue);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomIsInBeing()
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.RoomIsInside();
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomIsInRoomPool(RoomPool pool)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await pool.RoomIsInRoomPool(
                    SelectedBeing.InRoom
                );
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomNameIs(string newName)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing.InRoom.Rename(newName);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomPoolDescriptionIs(
        RoomPool pool, string poolDescription
    )
    {
        if(IsBuilder)
        {
            return await pool.SetDescription(poolDescription);
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomPoolFeeIs(RoomPool pool, Item item)
    {
        if(IsBuilder)
        {
            return await pool.SetFeeItem(item);
        }
        else
        {
            return UserIsNotBuilder();
        }
    }

    public async Task<CommandResult> RoomPoolNameIs(
        RoomPool pool, string newName
    )
    {
        if(IsBuilder)
        {
            return await pool.Rename(newName);
        }
        else
        {
            return UserIsNotBuilder();
        }
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

    public async Task<CommandResult> SelectedBeingIsFeature(string featureName)
    {
        if(IsBuilder)
        {
            if(SelectedBeing is not null)
            {
                return await SelectedBeing
                .BeingIsFeature(featureName);
            }
            else
            {
                return UserHasNotSelectedBeing();
            }
        }
        else
        {
            return UserIsNotBuilder();
        }
    }
    
    public async Task<CommandResult> SelectedBeingNameIs(string beingName)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.BeingNameIs(beingName);
        }
        else
        {
            return UserHasNotSelectedBeing();
        }
    }

    public async Task<CommandResult> ShowBeing()
    {
        if(SelectedBeing is not null)
        {
            return SelectedBeing.Show();
        }
        else
        {
            return UserHasNotSelectedBeing();
        }
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
