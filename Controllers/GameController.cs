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
    private readonly IInputCommand _input;

    public GameController(
        IGameService game,
        IResponsePayload response,
        IUserSession session,
        IInputCommand input
    )
    {
        _game = game;
        _response = response;
        _session = session;
        _input = input;
    }

    [HttpPost]
    public async Task<ActionResult> ParseRequest(
        RequestPayload payload
    )
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest();
        }

        // Identify user from token
        await _session.AuthenticateUser(payload.Token);

        for(int i = 0; i < payload.Commands.Length; i++)
        {
            var commandNow = payload.Commands[i];

            _response.AddText($"[{i}] {commandNow}:");

            _input.Text = commandNow;
            await _game.Respond();

            if(_response.IsBreaked())
            {
                break;
            }
        }

        return Ok(_response.GetPayload());
    }
}
