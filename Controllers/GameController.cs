using Microsoft.AspNetCore.Mvc;
using MUS.Game;

namespace MUS.Controllers;

[Route("")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _game;

    public GameController(IGameService game) {
        _game = game;
    }

    [HttpPost]
    public async Task<ActionResult> ParseRequest(UserInputPayload payload)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest();
        }

        var response = await _game.Respond(payload.UserInput);

        return Ok(response.GetTexts());
    }
}
