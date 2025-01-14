using Microsoft.AspNetCore.Mvc;
using MUS.Game;
using MUS.Game.Session;

namespace MUS.Controllers;

[Route("")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _game;
    private readonly IResponsePayload _response;
    private readonly IUserSession _session;

    public GameController(
        IGameService game,
        IResponsePayload response,
        IUserSession session
    )
    {
        _game = game;
        _response = response;
        _session = session;
    }

    [HttpPost]
    public async Task<ActionResult> ParseRequest(RequestPayload payload)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest();
        }

        // Identify user from token
        await _session.AuthenticateUser(payload.Token);

        await _game.ResolveCommands(payload.Commands);

        return Ok(_response.GetPayload());
    }
}
