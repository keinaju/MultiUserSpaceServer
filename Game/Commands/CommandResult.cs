using System;

namespace MUS.Game.Commands;

public class CommandResult
{
    public enum StatusCode
    {
        Success,
        Fail
    }

    private StatusCode _status;

    private List<string> _messages = new();

    public CommandResult(StatusCode status)
    {
        _status = status;
    }

    public CommandResult AddMessage(string message)
    {
        _messages.Add(message);

        return this;
    }

    public List<string> GetMessages()
    {
        return _messages;
    }

    public StatusCode GetStatus()
    {
        return _status;
    }

    public static CommandResult UserIsNotBuilder()
    {
        return new CommandResult(
            StatusCode.Fail
        ).AddMessage("You do not have the builder role.");
    }

    public static CommandResult UserIsNotSignedIn()
    {
        return new CommandResult(
            StatusCode.Fail
        ).AddMessage("You are not signed in.");
    }
}
