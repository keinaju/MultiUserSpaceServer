using System;
using System.Text.RegularExpressions;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : IGameCommand
{
    public string HelpText =>
    "Shows offers.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^(show|s) (.*) offers$");

    private string ItemNameInInput =>
    _input.GetGroup(this.Regex, 2);

    private readonly IOfferManager _offerManager;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public ShowOffersCommand(
        IOfferManager offerManager,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _offerManager = offerManager;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        _response.AddText(
            await _offerManager.FindOffers(
                ItemNameInInput == "*" ?
                "" : ItemNameInInput
            )
        );
    }
}
