namespace MUS.Game.Utilities;

static public class MessageStandard
{
    static public string DoesNotContain(string container, string item)
        => $"{container} does not contain {item}.";

    static public string DoesNotExist(string input)
        => $"'{input}' does not exist.";

    static public string Invalid(string input, string property)
        => $"'{input}' is not valid {property}.";

    static public string Quantity(string item, int quantity)
        => $"{item} ({quantity})";

    static public string List(IEnumerable<string?> enumarable) 
        => string.Join(", ", enumarable);
}
