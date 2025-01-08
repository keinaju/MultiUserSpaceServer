using System;

namespace MUS.Game.Utilities;

public class GameUptime : IGameUptime
{
    private DateTime _startTime;

    public GameUptime()
    {
        _startTime = DateTime.Now;
    }

    public DateTime GetStartTime()
    {
        return _startTime;
    }

    public TimeSpan GetUptime()
    {
        return DateTime.Now - GetStartTime();
    }

    public string GetUptimeText()
    {
        var span = GetUptime();
        return $"{span.Days} days, {span.Hours} hours, {span.Minutes} minutes and {span.Seconds} seconds";
    }

    public void ResetStartTime()
    {
        _startTime = DateTime.Now;
    }
}
