using System;
using System.Text.RegularExpressions;
using MUS.Game.Commands;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data;

public static class NameSanitation
{
    private const int MINIMUM_LENGTH = 2;
    private const int MAXIMUM_LENGTH = 20;
    private const string ALLOWED_CHARACTERS =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";

    public static string Clean(string name)
    {
        return name.Trim().ToUpper();
    }

    public static CommandResult Validate(string name)
    {
        name = Clean(name);

        var errors = new List<string>();

        if(name.Length > MAXIMUM_LENGTH)
        {
            errors.Add(
                $"'{name}' is longer than the maximum length of {MAXIMUM_LENGTH} characters."
            );
        }

        if(name.Length < MINIMUM_LENGTH)
        {
            errors.Add(
                $"'{name}' is shorter than the minimum length of {MINIMUM_LENGTH} characters."
            );
        }

        var illegalCharacter = FindIllegalCharacter(name);
        if(illegalCharacter is not null)
        {
            errors.Add(
                $"'{name}' contains a character that is not allowed: '{illegalCharacter}'."
                + $" The allowed characters are: {GetAllowedCharactersList()}."
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

    private static string? FindIllegalCharacter(string name)
    {
        var regex = new Regex($"[^{ALLOWED_CHARACTERS}]");

        var match = regex.Match(name);

        if(match.Success)
        {
            return match.Value;
        }
        else
        {
            return null;
        }
    }

    private static string GetAllowedCharactersList()
    {
        var allowedCharacters = new List<string>();

        foreach(var character in ALLOWED_CHARACTERS)
        {
            allowedCharacters.Add(character.ToString());
        }

        return Message.List(allowedCharacters);
    }
}
