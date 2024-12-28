using System;

namespace MUS.Game;

public interface IResponsePayload
{
    void AddList(IEnumerable<string> list);

    void AddText(string text);

    object GetPayload();

    void SetToken(string token);    
}
