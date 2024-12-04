using System;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IOfferManager _offerManager;

    private string ItemName => GetParameter(1);

    protected override string Description =>
        "Shows offers. Pass * to show all offers. "
        + "Pass an item name to show only offers that sell those items.";

    public ShowOffersCommand(IOfferManager offerManager)
    : base(regex: @"^show (.*) offers$")
    {
        _offerManager = offerManager;
    }

    public override async Task<string> Invoke()
    {
        return await _offerManager.FindOffers(
            ItemName == "*" ? "" : ItemName
        );
    }
}
