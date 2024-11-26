using System.Text.RegularExpressions;

namespace MUS.Game.Commands;

public abstract class BaseCommand : IGameCommand
{
    public abstract Prerequisite[] Prerequisites { get; }

    protected Match? RegularExpressionMatch = null;

    private readonly Regex _regularExpression;

    protected BaseCommand(string regex)
    {
        _regularExpression = new Regex(regex);
    }

    protected string GetParameter(int index)
    {
        return RegularExpressionMatch!.Groups[index].Value;
    }

    public bool IsMatch(string userInput)
    {
        var match = _regularExpression.Match(userInput);

        if (match.Success)
        {
            RegularExpressionMatch = match;
            return true;
        }

        return false;
    }

    public abstract Task<string> Invoke();
}
