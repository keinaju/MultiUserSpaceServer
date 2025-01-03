using System;

namespace MUS.Game;

public class ResponsePayload : IResponsePayload
{
    private List<string> _texts = [];
    private string _token = string.Empty;
    private bool _isBreaked = false;

    public ResponsePayload() {}

    public void AddList(IEnumerable<string> list)
    {
        foreach(var item in list)
        {
            _texts.Add(item);
        }
    }

    public void AddText(string text)
    {
        _texts.Add(text);
    }

    public void Break()
    {
        AddText("The command process has been breaked.");

        _isBreaked = true;
    }

    public object GetPayload()
    {
        return new {
            Texts = _texts,
            Token = _token
        };
    }

    public bool IsBreaked()
    {
        return _isBreaked;
    }

    public void SetToken(string token)
    {
        _token = token;
    }
}
