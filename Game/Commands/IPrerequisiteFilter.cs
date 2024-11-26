namespace MUS.Game.Commands;

public interface IPrerequisiteFilter
{
    string? Complain(Prerequisite[] prerequisites);
}
