namespace MUS.Game.Commands;

public class IntroductionCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    protected override string Description =>
        "Shows an introduction.";

    private readonly IGameResponse _response;
    
    public IntroductionCommand(IGameResponse response)
    : base(regex: @"^(\s*|help)$")
    {
        _response = response;
    }

    public override Task Invoke()
    {
        _response.AddText("Welcome to a Multi User Space -application!");
        _response.AddText("Try to help yourself by typing 'show commands'.");
        
        return Task.CompletedTask;
    }
}
