using System;
using MUS.Game.Utilities;

namespace MUS.Game.Commands;

public class OffersCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IOfferManager _offerManager;

    private string ItemName => GetParameter(1);

    public OffersCommand(IOfferManager offerManager)
    : base(regex: @"^(.*) offers$")
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
