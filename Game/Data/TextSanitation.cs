using System;
using System.Text.RegularExpressions;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data;

public static class TextSanitation
{
    private const string ALPHABET_LOWERCASE =
    "abcdefghijklmnopqrstuvwxyz";
    private const string ALPHABET_UPPERCASE =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string NUMERICS =
    "0123456789";
    private const string WHITE_SPACE = " ";
    private const string HYPHEN = "-";

    private const int DESCRIPTION_MINIMUM_LENGTH = 2;
    private const int DESCRIPTION_MAXIMUM_LENGTH = 1000;
    private const string DESCRIPTION_ALLOWED_CHARACTERS =
    ALPHABET_LOWERCASE +
    ALPHABET_UPPERCASE +
    NUMERICS +
    WHITE_SPACE +
    "\"'.,!?()" +
    // The hyphen must be listed first or last because it is part
    // of a character group in a regular expression.
    // Hyphen in the middle of a character group
    // would instead define a range of allowed characters.
    HYPHEN;
    
    private const int NAME_MINIMUM_LENGTH = 2;
    private const int NAME_MAXIMUM_LENGTH = 20;
    private const string NAME_ALLOWED_CHARACTERS =
    ALPHABET_UPPERCASE +
    NUMERICS +
    WHITE_SPACE;

    public static string GetCleanDescription(string description)
    {
        return description.Trim();
    }

    public static string GetCleanName(string name)
    {
        return name.Trim().ToUpper();
    }

    public static CommandResult ValidateDescription(string description)
    {
        description = GetCleanDescription(description);

        var errors = new List<string>();

        if(description.Length < DESCRIPTION_MINIMUM_LENGTH)
        {
            errors.Add(
                $"'{description}' is shorter than the minimum length of {DESCRIPTION_MINIMUM_LENGTH} characters."
            );
        }

        if(description.Length > DESCRIPTION_MAXIMUM_LENGTH)
        {
            errors.Add(
                $"'{description}' is longer than the maximum length of {DESCRIPTION_MAXIMUM_LENGTH} characters."
            );
        }

        var illegalCharacter = FindIllegalCharacter(
            DESCRIPTION_ALLOWED_CHARACTERS, description
        );
        if(illegalCharacter is not null)
        {
            errors.Add(
                $"'{description}' contains a character that is not allowed: '{illegalCharacter}'."
                + $" The allowed characters are: {GetAllowedCharactersList(DESCRIPTION_ALLOWED_CHARACTERS)}."
            );
        }

        if(errors.Count == 0)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"'{description}' passed the validation.");
        }
        else
        {
            var result = new CommandResult(StatusCode.Fail);

            foreach(var error in errors)
            {
                result.AddMessage(error);
            }

            return result;
        }
    }

    public static CommandResult ValidateName(string name)
    {
        name = GetCleanName(name);

        var errors = new List<string>();

        if(name.Length < NAME_MINIMUM_LENGTH)
        {
            errors.Add(
                $"'{name}' is shorter than the minimum length of {NAME_MINIMUM_LENGTH} characters."
            );
        }

        if(name.Length > NAME_MAXIMUM_LENGTH)
        {
            errors.Add(
                $"'{name}' is longer than the maximum length of {NAME_MAXIMUM_LENGTH} characters."
            );
        }

        var illegalCharacter = FindIllegalCharacter(
            NAME_ALLOWED_CHARACTERS, name
        );
        if(illegalCharacter is not null)
        {
            errors.Add(
                $"'{name}' contains a character that is not allowed: '{illegalCharacter}'."
                + $" The allowed characters are: {GetAllowedCharactersList(NAME_ALLOWED_CHARACTERS)}."
            );
        }

        if(errors.Count == 0)
        {
            return new CommandResult(StatusCode.Success)
            .AddMessage($"'{name}' passed the validation.");
        }
        else
        {
            var result = new CommandResult(StatusCode.Fail);

            foreach(var error in errors)
            {
                result.AddMessage(error);
            }

            return result;
        }
    }

    private static string? FindIllegalCharacter(
        string allowedCharacters, string input
    )
    {
        // Negative character group defines a list of characters that
        // must not appear in an input string for a match to occur.
        var regex = new Regex($"[^{allowedCharacters}]");
        var match = regex.Match(input);
        if(match.Success)
        {
            // Illegal character is found
            return match.Value;
        }
        else
        {
            // Illegal character is not found
            return null;
        }
    }

    private static string GetAllowedCharactersList(
        string allowedCharacters
    )
    {
        var characterList = new List<string>();

        foreach(var character in allowedCharacters)
        {
            characterList.Add(character.ToString());
        }

        return Message.List(characterList);
    }
}
