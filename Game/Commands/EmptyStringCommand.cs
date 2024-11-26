namespace MUS.Game.Commands;

public class EmptyStringCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    public EmptyStringCommand() : base(regex: @"^\s*$") { }

    public override async Task<string> Invoke() =>
        @"Welcome to Multi User Space!
        You don't get far with empty inputs.
        Try to help yourself with 'help' command.";
}
