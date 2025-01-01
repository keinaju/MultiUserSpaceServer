using System;
using System.Text.RegularExpressions;

namespace MUS.Game;

public interface IInputCommand
{
    string GetGroup(Regex regex, int index);
    
    string Text { get; set; }
}
