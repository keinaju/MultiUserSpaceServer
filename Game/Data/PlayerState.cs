using MUS.Game.Data.Models;
using MUS.Game.Session;

namespace MUS.Game.Data;

public class PlayerState : IPlayerState
{
    private readonly ISessionService _session;

    public PlayerState(
        ISessionService session
    )
    {
        _session = session;
    }
    
    public Being GetSelectedBeing()
    {
        var user = _session.User;
        
        if (user is null || user.SelectedBeing is null)
        {
            throw new NullReferenceException();
        }

        return user.SelectedBeing;
    }

    public Room GetCurrentRoom()
    {
        return GetSelectedBeing().InRoom;
    }
}
