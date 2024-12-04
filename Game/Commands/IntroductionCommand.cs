namespace MUS.Game.Commands;

public class IntroductionCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
        "Shows an introduction.";

    public IntroductionCommand() : base(regex: @"^(\s*|help)$") { }

    public override async Task<string> Invoke() =>
        @"Welcome to Multi User Space!
        Try to help yourself by typing 'show commands'.";
}
