using MUS.Game.Data.Models;

namespace MUS.Game.Data;

public interface IPlayerState
{
    Task<Room> CurrentRoom();
    Task<Being> PickedBeing();
}
