using MUS.Game.Data.Models;

namespace MUS.Game.Data.Repositories;

public interface IItemGeneratorRepository
{
    Task<ItemGenerator> CreateItemGenerator(ItemGenerator itemGenerator);
    Task<List<ItemGenerator>> GetAllItemGenerators();
}
