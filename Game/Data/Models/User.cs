﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MUS.Game.Commands;
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

    public async Task<CommandResult> DeleteBeing(string beingName)
    {
        var beingExists = CreatedBeings.Any(
            being => being.Name == beingName
        );

        if (beingExists)
        {
            var being = CreatedBeings.First(
                being => being.Name == beingName
            );
            CreatedBeings.Remove(being);

            await _context.SaveChangesAsync();

            return new CommandResult(
                CommandResult.StatusCode.Success
            ).AddMessage(Message.Deleted("being", being.Name));
        }
        else
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage(Message.DoesNotExist("being", beingName));
        }
    }

    public async Task<CommandResult> DeleteFeature(string featureName)
    {
        if(!IsBuilder)
        {
            return CommandResult.UserIsNotBuilder();
        }

        var featureExists = _context.Features.Any(
            feature => feature.Name == featureName
        );

        if(featureExists)
        {
            var feature = _context.Features.First(
                feature => feature.Name == featureName
            );
            _context.Features.Remove(feature);

            await _context.SaveChangesAsync();

            return new CommandResult(
                CommandResult.StatusCode.Success
            ).AddMessage(Message.Deleted("feature", feature.Name));
        }
        else
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage(Message.DoesNotExist("feature", featureName));
        }
    }

    public async Task<CommandResult> DeleteItem(string itemName)
    {
        if(!IsBuilder)
        {
            return CommandResult.UserIsNotBuilder();
        }

        var itemExists = _context.Items.Any(
            item => item.Name == itemName
        );

        if(itemExists)
        {
            var item = _context.Items.First(
                item => item.Name == itemName
            );

            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            return new CommandResult(
                CommandResult.StatusCode.Success
            ).AddMessage(Message.Deleted("item", item.Name));
        }
        else
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage(Message.DoesNotExist("item", itemName));
        }
    }

    public async Task<CommandResult> DeleteRoom(string roomName)
    {
        if(!IsBuilder)
        {
            return CommandResult.UserIsNotBuilder();
        }

        var roomExists = _context.Rooms.Any(
            room => room.Name == roomName
        );

        if(roomExists)
        {
            var room = _context.Rooms.First(
                room => room.Name == roomName
            );

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();

            return new CommandResult(
                CommandResult.StatusCode.Success
            ).AddMessage(Message.Deleted("room", room.Name));
        }
        else
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage(Message.DoesNotExist("room", roomName));
        }
    }

    public async Task<CommandResult> DeleteRoomPool(string poolName)
    {
        if(!IsBuilder)
        {
            return CommandResult.UserIsNotBuilder();
        }

        var poolExists = _context.RoomPools.Any(
            pool => pool.Name == poolName
        );

        if(poolExists)
        {
            var pool = _context.RoomPools.First(
                pool => pool.Name == poolName
            );

            var roomsWithCuriosity = await _context.Rooms.Where(
                room => room.Curiosity == pool
            ).ToListAsync();

            foreach(var room in roomsWithCuriosity)
            {
                room.Curiosity = null;
            }

            _context.RoomPools.Remove(pool);

            await _context.SaveChangesAsync();

            return new CommandResult(
                CommandResult.StatusCode.Success
            ).AddMessage(Message.Deleted("room pool", pool.Name));

        }
        else
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage(Message.DoesNotExist("room pool", poolName));
        }
    }

    public async Task<CommandResult> Explore()
    {
        if(SelectedBeing is null)
        {
            return new CommandResult(
                CommandResult.StatusCode.Fail
            ).AddMessage($"{Username} has not selected a being.");
        }
        else
        {
            return await SelectedBeing.Explore();
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
