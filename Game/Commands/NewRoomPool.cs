using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

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
        var rp = new RoomPool() { Name = string.Empty };
        var rpInDb = await _roomPoolRepository.CreateRoomPool(rp);
        rpInDb.Name = $"RP-{rpInDb.PrimaryKey}";
        await _roomPoolRepository.UpdateRoomPool(rpInDb);
        return $"Room pool {rpInDb.Name} was created.";
    }
}
