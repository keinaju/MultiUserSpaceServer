using System;

namespace MUS.Game.Utilities;

public interface IGameUptime
{
    DateTime GetStartTime();

    TimeSpan GetUptime();

    string GetUptimeText();

    void ResetStartTime();
}
