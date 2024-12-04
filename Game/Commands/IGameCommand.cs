namespace MUS.Game.Commands;

public interface IGameCommand
{
    Prerequisite[] Prerequisites { get; }

    string HelpText { get; }

    bool IsMatch(string userInput);

    Task<string> Invoke();
}
