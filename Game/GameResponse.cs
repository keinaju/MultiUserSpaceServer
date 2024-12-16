using System;

namespace MUS.Game;

public class GameResponse : IGameResponse
{
    private List<string> _texts;

    public GameResponse()
    {
        _texts = [];
    }
    
    public void AddText(string text)
    {
        _texts.Add(text);
    }

    public ICollection<string> GetTexts()
    {
        return _texts;
    }
}
