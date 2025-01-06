using System;
using System.Text.RegularExpressions;
using MUS.Game.Data.Models;
using MUS.Game.Data.Repositories;
using MUS.Game.Utilities;

namespace MUS.Game.Commands.Is;

public class ItemIsMadeOfCommand : IGameCommand
{
    public string HelpText => "Sets components in a craft plan.";

    public Condition[] Conditions =>
    [
        Condition.UserIsSignedIn,
        Condition.UserIsBuilder
    ];

    public Regex Regex => new(@"^item (.+) is made of (\d+) (.+)$");

    private string ProductNameInInput => _input.GetGroup(this.Regex, 1);
    private string QuantityInInput => _input.GetGroup(this.Regex, 2);
    private string ComponentNameInInput => _input.GetGroup(this.Regex, 3);

    private readonly IItemRepository _itemRepo;
    private readonly IResponsePayload _response;
    private readonly IInputCommand _input;

    public ItemIsMadeOfCommand(
        IItemRepository itemRepo,
        IResponsePayload response,
        IInputCommand input
    )
    {
        _itemRepo = itemRepo;
        _response = response;
        _input = input;
    }

    public async Task Run()
    {
        var component = await _itemRepo.FindItem(ComponentNameInInput);
        if(component is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ComponentNameInInput)
            );
            return;
        }

        var product = await _itemRepo.FindItem(ProductNameInInput);
        if(product is null)
        {
            _response.AddText(
                Message.DoesNotExist("item", ProductNameInInput)
            );
            return;
        }

        if(component == product)
        {
            _response.AddText(
                "Component and product can not be the same item."
            );
            return;
        }

        // Validate quantity
        bool success = int.TryParse(
            QuantityInInput,
            out int quantity
        );
        if(!success || quantity < 0)
        {
            _response.AddText(
                Message.Invalid(QuantityInInput, "quantity")
            );
            return;
        }

        await SetComponent(product, component, quantity);

        _response.AddText(
            $"{Message.Quantity(component.Name, quantity)} was set to {product.Name}'s craft plan."
        );
    }

    private async Task SetComponent(
        Item product,
        Item component,
        int quantity
    )
    {
        product.SetComponent(component, quantity);

        await _itemRepo.UpdateItem(product);
    }
}
