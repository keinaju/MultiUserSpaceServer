using System;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Show;

public class ShowRoomPoolsCommand : BaseCommand
{
    public override Prerequisite[] Prerequisites => [];

    private readonly IRoomPoolRepository _roomPoolRepository;

    protected override string Description =>
        "Shows all room pools.";

    public ShowRoomPoolsCommand(
        IRoomPoolRepository roomPoolRepository
    )
    : base(regex: @"^show room pools$")
    {
        _roomPoolRepository = roomPoolRepository;
    }

    public override async Task<string> Invoke()
    {
        var pools = await _roomPoolRepository.FindRoomPools();
        if(pools.Count == 0)
        {
            return "There are no room pools.";
        }

        var poolNames = new List<string>();
        foreach(var pool in pools)
        {
            poolNames.Add(pool.Name);
        }

        return $"Room pools are: {MessageStandard.List(poolNames)}.";
    }
}
