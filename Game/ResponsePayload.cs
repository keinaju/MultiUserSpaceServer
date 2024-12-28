using System;

namespace MUS.Game;

public class ResponsePayload : IResponsePayload
{
    private List<string> _texts;
    private string _token;

    public ResponsePayload()
    {
        _texts = [];
        _token = string.Empty;
    }

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

    public void SetToken(string token)
    {
        _token = token;
    }

    public object GetPayload()
    {
        return new {
            Texts = _texts,
            Token = _token
        };
    }
}
