using MUS.Game.Data.Models;

namespace MUS.Game.Data;

public interface IPlayerState
{
    Being GetSelectedBeing();

    Room GetCurrentRoom();
}
