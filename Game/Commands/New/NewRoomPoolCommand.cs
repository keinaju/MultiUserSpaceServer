using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewRoomPoolCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn
    ];

    private readonly IRoomPoolRepository _roomPoolRepository;

    public NewRoomPoolCommand(
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^new roompool$")
    {
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task<string> Invoke()
    {
        var rp = new RoomPool() { 
            Name = string.Empty,
            Description = string.Empty
        };

        var rpInDb = await _roomPoolRepository.CreateRoomPool(rp);
        rpInDb.Name = $"rp{rpInDb.PrimaryKey}";

        await _roomPoolRepository.UpdateRoomPool(rpInDb);
        return MessageStandard.Created("room pool", rpInDb.Name);
    }
}
