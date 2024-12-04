using System;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowOffersCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IOfferManager _offerManager;

    private string ItemName => GetParameter(1);

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
