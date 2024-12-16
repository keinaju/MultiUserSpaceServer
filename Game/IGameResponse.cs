using System;

namespace MUS.Game;

public interface IGameResponse
{
    void AddText(string text);
    ICollection<string> GetTexts();
}
