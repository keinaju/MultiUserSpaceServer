using System;
using MUS.Game.Commands;

namespace MUS.Game;

public interface IResponsePayload
{
    void AddList(IEnumerable<string> list);

    void AddResult(CommandResult result);

    void AddText(string text);

    void Break();

    object GetPayload();

    bool IsBreaked();

    void PrependText(string text);

    void SetToken(string token);
}
