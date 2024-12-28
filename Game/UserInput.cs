using System;
using System.Text.RegularExpressions;

namespace MUS.Game;

public class UserInput : IUserInput
{
    public string Text { get; set; } = string.Empty;

    public string GetGroup(Regex regex, int index)
    {
        var match = regex.Match(Text);
        return match.Groups[index].Value;
    }
}
