using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.New;

public class NewRoomCommand : ICommandPattern
{
    public bool AdminOnly => true;

    public string HelpText =>
    "Creates a new room and connects it to the current room.";

    public Regex Pattern => new("^new room (.+)$");

    private string RoomNameInInput => _input.GetGroup(this.Pattern, 1);

    private readonly GameContext _context;
    private readonly IInputCommand _input;

    public NewRoomCommand(
        GameContext context,
        IInputCommand input
    )
    {
        _context = context;
        _input = input;
    }

    public async Task<CommandResult> Run(User user)
    {
        if(user.SelectedBeing is null)
        {
            return user.NoSelectedBeingResult();
        }
        else
        {
            return await NewRoom(user.SelectedBeing);
        }
    }

    private async Task<CommandResult> NewRoom(Being being)
    {
        var validationResult = TextSanitation.ValidateName(RoomNameInInput);
        if(validationResult.GetStatus() == StatusCode.Fail)
        {
            return validationResult;
        }
        
        var cleanName = TextSanitation.GetCleanName(RoomNameInInput);
        if(await _context.RoomNameIsReserved(cleanName))
        {
            return NameIsReserved("room", cleanName);
        }

        EntityEntry<Room> entry = await _context.Rooms.AddAsync(
            new Room()
            {
                GlobalAccess = false,
                InBeing = null,
                Inventory = new Inventory(),
                Name = cleanName
            }
        );
        var newRoom = entry.Entity;

        newRoom.ConnectBidirectionally(being.InRoom);

        being.InRoom = newRoom;

        await _context.SaveChangesAsync();

        return new CommandResult(StatusCode.Success)
        .AddMessage(Message.Created("room", newRoom.Name))
        .AddMessage($"{being.Name} has moved in {newRoom.Name} automatically.");
    }
}
