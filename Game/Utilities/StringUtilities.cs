using System;

namespace MUS.Game.Utilities;

public static class StringUtilities
{
    public static string FirstCharToUpper(this string input) =>
    input switch
    {
        null => throw new ArgumentNullException(
            nameof(input)
        ),
        "" => throw new ArgumentException(
            $"{nameof(input)} cannot be empty",
            nameof(input)
        ),
        _ => string.Concat(
            input[0].ToString().ToUpper(),
            input.Substring(1)
        )
    };

    public static string GetRandomCharacter()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var random = new Random();
        int index = random.Next(characters.Length);

        return characters[index].ToString();
    }
}
