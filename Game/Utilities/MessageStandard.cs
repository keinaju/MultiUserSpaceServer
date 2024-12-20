namespace MUS.Game.Utilities;

static public class MessageStandard
{
    static public string Contains(string container, string content)
        => $"{container} contains {content}.";

    static public string Created(string type, string name = "")
        => name == "" ?
            $"New {type} has been created." :
            $"New {type} (named '{name}') has been created.";

    static public string DoesNotContain(string container, string content)
        => $"{container} does not contain {content}.";

    static public string DoesNotExist(string type, string input)
        => $"{type} '{input}' does not exist.";

    static public string Invalid(string input, string type)
        => $"'{input}' is not valid {type}.";

    static public string List(IEnumerable<string?> enumarable) 
        => string.Join(", ", enumarable);

    static public string Quantity(string item, int quantity)
        => $"[{quantity}] {item}";

    static public string Renamed(string old, string @new)
        => $"{old} is renamed {@new}.";

    static public string Set(string type, string to)
        => $"{type} is now set to {to}.";
}