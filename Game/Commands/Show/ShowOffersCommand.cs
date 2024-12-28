using System;
using System.Text.RegularExpressions;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : IGameCommand
{
    public string HelpText =>
    "Shows offers.";

    public Condition[] Conditions => [];

    public Regex Regex => new("^show (.*) offers$");

    private string ItemNameInInput =>
    _userInput.GetGroup(this.Regex, 1);

    private readonly IOfferManager _offerManager;
    private readonly IResponsePayload _response;
    private readonly IUserInput _userInput;

    public ShowOffersCommand(
        IOfferManager offerManager,
        IResponsePayload response,
        IUserInput userInput
    )
    {
        _offerManager = offerManager;
        _response = response;
        _userInput = userInput;
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
