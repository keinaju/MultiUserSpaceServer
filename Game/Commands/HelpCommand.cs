namespace MUS.Game.Commands;

public class HelpCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    public HelpCommand() : base(regex: @"^help$") { }

    public override async Task<string> Invoke() => "signup, login";
}
