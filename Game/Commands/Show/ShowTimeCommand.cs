using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Repositories;

namespace MUS.Game.Commands.Show;

public class ShowTimeCommand : IGameCommand
{
    public string HelpText => "Shows game's time.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^show time$");

    private readonly IResponsePayload _response;
    private readonly ITickCounterRepository _tickRepo;

    public ShowTimeCommand(
        IResponsePayload response,
        ITickCounterRepository tickRepo
    )
    {
        _response = response;
        _tickRepo = tickRepo;
    }

    public async Task Run()
    {
        var tickCounter = await _tickRepo.GetTickCount();

        _response.AddText(
            $"Time is {tickCounter!.TickCount}."
        );
    }
}
