using System;
using System.Text.RegularExpressions;
using MUS.Game.Data;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : IGameCommand
{
    public string HelpText => "Shows all offers.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(show|s) offers$");

    private readonly GameContext _context;
    private readonly IResponsePayload _response;

    public ShowOffersCommand(
        GameContext context,
        IResponsePayload response
    )
    {
        _context = context;
        _response = response;
    }

    public async Task Run()
    {
        _response.AddResult(
            await ShowOffers()
        );
    }

    private async Task<CommandResult> ShowOffers()
    {
        var offers = await _context.FindAllOffers();
        if(offers.Count == 0)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage("There are no offers.");
        }
        else
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"All offers are: {GetOfferDetails(offers)}.");
        }
    }

    private string GetOfferDetails(IEnumerable<Offer> offers)
    {
        var offerDetails = new List<string>();

        foreach(var offer in offers)
        {
            offerDetails.Add(offer.GetDetails());
        }

        return Message.List(offerDetails);
    }
}
