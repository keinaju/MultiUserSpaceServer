namespace MUS.Game.Clock;

public interface IGameClockListener
{
    Task GetTask(object sender, TickEventArgs eventArgs);
}
