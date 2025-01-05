using System;
using MUS.Game.Utilities;

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

    public static CommandResult BeingDoesNotExist(string beingName)
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage(Message.DoesNotExist("being", beingName));
    }

    public static CommandResult FeatureDoesNotExist(string featureName)
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage(Message.DoesNotExist("feature", featureName));
    }

    public static CommandResult ItemDoesNotExist(string itemName)
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage(Message.DoesNotExist("item", itemName));
    }

    public static CommandResult RoomDoesNotExist(string roomName)
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage(Message.DoesNotExist("room", roomName));
    }

    public static CommandResult RoomPoolDoesNotExist(string poolName)
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage(Message.DoesNotExist("room pool", poolName));
    }

    public static CommandResult UserIsNotSignedIn()
    {
        return new CommandResult(StatusCode.Fail)
        .AddMessage("You are not signed in.");
    }
}
