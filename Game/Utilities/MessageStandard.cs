namespace MUS.Game.Utilities;

static public class MessageStandard
{
    static public string Created(string type, string name)
        => $"New {type} (named '{name}') has been created.";
    
    static public string Described(string item, string description)
        => $"{item} is now described as '{description}'.";

    static public string DoesNotContain(string container, string item)
        => $"{container} does not contain {item}.";

    static public string DoesNotExist(string input)
        => $"'{input}' does not exist.";

    static public string Invalid(string input, string type)
        => $"'{input}' is not valid {type}.";

    static public string List(IEnumerable<string?> enumarable) 
        => string.Join(", ", enumarable);

    static public string Quantity(string item, int quantity)
        => $"{item} ({quantity})";

    static public string Renamed(string old, string @new)
        => $"{old} is renamed {@new}.";
}