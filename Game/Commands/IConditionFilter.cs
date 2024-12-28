using System;

namespace MUS.Game.Commands;

public interface IConditionFilter
{
    bool MeetsConditions(Condition[] conditions);
}
