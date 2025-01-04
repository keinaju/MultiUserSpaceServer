namespace MUS.Game.Utilities;

static public class Message
{
    static public string Contains(
        string container, string content
    ) => $"{container.FirstCharToUpper()} contains {content}.";

    static public string Created(
        string type, string name = ""
    ) => name == "" ? 
    $"A new {type} has been created." :
    $"A new {type} (named '{name}') has been created.";

    static public string Deleted(
        string type, string name = ""
    ) => name == "" ?
    $"{type.FirstCharToUpper()} has been deleted." : 
    $"{type.FirstCharToUpper()} (named '{name}') has been deleted.";

    static public string DoesNotContain(
        string container, string content
    ) => $"{container.FirstCharToUpper()} does not contain {content}.";

    static public string DoesNotExist(
        string type, string input
    ) => $"{type.FirstCharToUpper()} '{input}' does not exist.";

    static public string DoesNotHave(
        string container, string content
    ) => $"{container.FirstCharToUpper()} does not have {content}.";

    static public string Exists(
        string type, string input
    ) => $"{type.FirstCharToUpper()} '{input}' exists.";

    static public string Has(
        string container, string content
    ) => $"{container.FirstCharToUpper()} has {content}.";

    static public string Invalid(
        string input, string type
    ) => $"'{input}' is not a valid {type}.";

    static public string List(
        IEnumerable<string?> enumarable
    ) => string.Join(", ", enumarable);

    static public string Quantity(
        string item, int quantity
    ) => $"[{quantity}] {item}";

    static public string Renamed(
        string old, string @new
    ) => $"'{old}' is renamed '{@new}'.";

    static public string Reserved(
        string type, string input
    ) => $"'{input}' is a reserved {type}.";

    static public string Set(
        string type, string to
    ) => $"{type.FirstCharToUpper()} is now set to '{to}'.";
}