using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowRoomPoolsCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IGameResponse _response;
    private readonly IRoomPoolRepository _roomPoolRepository;

    protected override string Description =>
        "Shows all room pools.";

    public ShowRoomPoolsCommand(
        IGameResponse response,
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^show room pools$")
    {
        _response = response;
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task Invoke()
    {
        var pools = await _roomPoolRepository.FindRoomPools();
        if(pools.Count == 0)
        {
            _response.AddText("There are no room pools.");
            return;
        }

        var poolNames = new List<string>();
        foreach(var pool in pools)
        {
            poolNames.Add(pool.Name);
        }

        _response.AddText(
            $"Room pools are: {MessageStandard.List(poolNames)}."
        );
    }
}
