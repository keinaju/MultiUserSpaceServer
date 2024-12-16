using System;
using Microsoft.VisualBasic;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IGameResponse _response;
    private readonly IOfferManager _offerManager;

    private string ItemName => GetParameter(1);

    protected override string Description =>
        "Shows offers. Pass * to show all offers. "
        + "Pass an item name to show only offers that sell those items.";

    public ShowOffersCommand(
        IGameResponse response,
        IOfferManager offerManager
    )
    : base(regex: @"^show (.*) offers$")
    {
        _offerManager = offerManager;
        _response = response;
    }

    public override async Task Invoke()
    {
        _response.AddText(
            await _offerManager.FindOffers(ItemName == "*" ? "" : ItemName)
        );
    }
}
