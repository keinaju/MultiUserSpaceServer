using MUS.Game.Data;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands;

public class GoToCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [
        Prerequisite.UserIsLoggedIn,
        Prerequisite.UserHasPickedBeing
    ];

    private readonly IBeingRepository _beingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerState _state;

    private string RoomNameInUserInput => GetParameter(1);

    public GoToCommand(
        IBeingRepository beingRepository,
        IRoomRepository roomRepository,
        IPlayerState state
    )
    : base(regex: @"^go to (.+)$")
    {
        _beingRepository = beingRepository;
        _roomRepository = roomRepository;
        _state = state;
    }

    public override async Task<string> Invoke()
    {
        var destinationRoom = await _roomRepository.FindRoom(RoomNameInUserInput);
        if (destinationRoom is null)
        {
            return $"'{RoomNameInUserInput}' was not found.";
        }

        var pickedBeing = await _state.Being();
        if (pickedBeing.Room == destinationRoom)
        {
            return $"{pickedBeing.Name} is in {destinationRoom.Name}.";
        }

        pickedBeing.Room = destinationRoom;
        await _beingRepository.UpdateBeing(pickedBeing);
        return $"{pickedBeing.Name} moved to {destinationRoom.Name}.";
    }
}
