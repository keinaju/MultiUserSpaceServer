using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.New;

public class NewRoomPoolCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserIsBuilder
    ];

    protected override string Description =>
    "Creates a new room pool to generate cloned rooms.";

    private readonly IGameResponse _response;
    private readonly IRoomPoolRepository _roomPoolRepository;

    public NewRoomPoolCommand(
        IGameResponse response,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^new room pool$")
    {
        _response = response;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task Invoke()
    {
        var newRoomPool = new RoomPool() { 
            Description = string.Empty,
            ItemToExplore = null,
            Name = string.Empty
        };

        var roomPoolInDb = await _roomPoolRepository
        .CreateRoomPool(newRoomPool);
        roomPoolInDb.Name = $"rp{roomPoolInDb.PrimaryKey}";

        await _roomPoolRepository.UpdateRoomPool(roomPoolInDb);

        _response.AddText(
            MessageStandard.Created(
                "room pool", roomPoolInDb.Name
            )
        );
    }
}
