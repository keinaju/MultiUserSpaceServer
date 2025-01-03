using System;

namespace MUS.Game;

public interface IResponsePayload
{
    void AddList(IEnumerable<string> list);

    void AddText(string text);

    void Break();

    object GetPayload();

    bool IsBreaked();

    void SetToken(string token);
}
