using System.ComponentModel.DataAnnotations;
using MUS.Game.Utilities;

namespace MUS.Game.Data.Models;

public class Offer
{
    [Key]
    public int PrimaryKey { get; set; }

    public required Item ItemToSell { get; set; }
    public required int QuantityToSell { get; set; }

    public required Item ItemToBuy { get; set; }
    public required int QuantityToBuy { get; set; }

    public required Inventory Inventory { get; set; }

    public override string ToString()
        => $"({MessageStandard.Quantity(ItemToSell.Name, QuantityToSell)} "
        + $"for {MessageStandard.Quantity(ItemToBuy.Name, QuantityToBuy)})";
}
