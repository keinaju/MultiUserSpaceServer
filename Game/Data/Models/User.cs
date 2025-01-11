﻿using Microsoft.EntityFrameworkCore;
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
    /// Role to determine if user has access to admin commands.
    /// </summary>
    public required bool IsAdmin { get; set; }

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

    public async Task<CommandResult> CuriosityIs(string poolName)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.CuriosityIs(poolName);
        }
        else
        {
            return NoSelectedBeingResult();
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

    public async Task<CommandResult> DeployItem(string itemName)
    {
        if(SelectedBeing is null)
        {
            return NoSelectedBeingResult();
        }
        else
        {
            return await SelectedBeing.DeployItem(itemName);
        }
    }

    public async Task<CommandResult> DeploymentIs(Item item)
    {
        if(SelectedBeing is not null)
        {
            return await item.SetDeployment(SelectedBeing);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> Explore()
    {
        if(SelectedBeing is null)
        {
            return NoSelectedBeingResult();
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

        texts.Add($"Your username is {Username}.");
        if(this.IsAdmin)
        {
            texts.Add("You have access to admin commands.");
        }
        texts.Add(GetSelectedBeingText());
        texts.Add(GetCreatedBeingsText());

        return texts;
    }
    
    public async Task<CommandResult> Go(string roomName)
    {
        if(SelectedBeing is null)
        {
            return NoSelectedBeingResult();
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

    public async Task<CommandResult> ItemHatcherIntervalIs(
        Item item, int interval
    )
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom
            .ItemHatcherIntervalIs(item, interval);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> ItemHatcherQuantityIs(
        Item item, int minQuantity, int maxQuantity
    )
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom
            .ItemHatcherQuantityIs(item, minQuantity, maxQuantity);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> ItemIsMadeOf(
        Item product, Item component, int quantity
    )
    {
        return await product.SetComponent(component, quantity);
    }

    public async Task<CommandResult> ItemNameIs(
        Item item, string newName
    )
    {
        return await item.Rename(newName);
    }

    public async Task<CommandResult> MakeItems(Item item, int quantity)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.MakeItems(item, quantity);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> NewBeing(string beingName)
    {
        return await _context.CreateBeing(this, beingName);
    }

    public async Task<CommandResult> NewFeature(string featureName)
    {
        return await _context.CreateFeature(featureName);
    }

    public async Task<CommandResult> NewItem(string itemName)
    {
        return await _context.CreateItem(itemName);
    }

    public async Task<CommandResult> NewItemHatcher(Item item)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom.CreateItemHatcher(item);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> NewRoom(string roomName)
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
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> NewRoomPool(string poolName)
    {
        return await _context.CreateRoomPool(poolName);
    }

    public async Task<CommandResult> RoomDescriptionIs(string roomDescription)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom
            .RoomDescriptionIs(roomDescription);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> RoomIsFor(Feature feature)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom.RoomIsFor(feature);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> RoomIsGlobal(bool newValue)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom.RoomIsGlobal(newValue);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> RoomIsInBeing()
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.RoomIsInside();
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> RoomIsInRoomPool(RoomPool pool)
    {
        if(SelectedBeing is not null)
        {
            return await pool.RoomIsInRoomPool(
                SelectedBeing.InRoom
            );
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> RoomNameIs(string newName)
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom.Rename(newName);
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> RoomPoolDescriptionIs(
        RoomPool pool, string poolDescription
    )
    {
        return await pool.SetDescription(poolDescription);
    }

    public async Task<CommandResult> RoomPoolFeeIs(RoomPool pool, Item item)
    {
        return await pool.SetFeeItem(item);
    }

    public async Task<CommandResult> RoomPoolNameIs(
        RoomPool pool, string newName
    )
    {
        return await pool.Rename(newName);
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
        if(SelectedBeing is not null)
        {
            return await SelectedBeing
            .BeingIsFeature(featureName);
        }
        else
        {
            return NoSelectedBeingResult();
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
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> Sell(
        int sellQuantity, int buyQuantity,
        Item sellItem, Item buyItem
    )
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.Sell(
                sellQuantity, buyQuantity,
                sellItem, buyItem
            );
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> SetTickInterval(int intervalSeconds)
    {
        var settings = await _context.GetGameSettings();

        settings.TickIntervalSeconds = intervalSeconds;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(
            Message.Set("tick interval", $"{intervalSeconds} seconds")
        );
    }

    public CommandResult ShowBeing()
    {
        if (SelectedBeing is not null)
        {
            return SelectedBeing.Show();
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public CommandResult ShowInventory()
    {
        if(SelectedBeing is not null)
        {
            return SelectedBeing.ShowInventory();
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public async Task<CommandResult> ShowOffersInCurrentRoom()
    {
        if(SelectedBeing is not null)
        {
            return await SelectedBeing.InRoom.ShowOffersInRoom();
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public CommandResult ShowRoom()
    {
        if(SelectedBeing is not null)
        {
            return SelectedBeing.ShowRoom();
        }
        else
        {
            return NoSelectedBeingResult();
        }
    }

    public CommandResult ShowUser()
    {
        return new CommandResult(StatusCode.Success)
        .AddMessages(this.GetDetails());
    }

    public async Task<CommandResult> TakeItem(string itemName)
    {
        if(SelectedBeing is null)
        {
            return NoSelectedBeingResult();
        }
        else
        {
            return await SelectedBeing.TakeItemFromRoom(itemName);
        }
    }

    public CommandResult NoSelectedBeingResult()
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage($"User {Username} has not selected a being.");
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

    private string GetCreatedBeingsText()
    {
        if(CreatedBeings.Count > 0)
        {
            return $"You have created {Message.Quantity("beings", CreatedBeings.Count)}: {GetCreatedBeingNames()}.";
        }
        else
        {
            return "You do not have any beings.";
        }
    }

    private string GetCreatedBeingNames()
    {
        var beingNames = new List<string>();

        foreach (var being in CreatedBeings)
        {
            beingNames.Add(being.Name!);
        }

        return Message.List(beingNames);
    }
}
