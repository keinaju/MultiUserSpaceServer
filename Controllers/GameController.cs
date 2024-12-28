using Microsoft.AspNetCore.Mvc;
using MUS.Game;
using MUS.Game.Session;

namespace MUS.Controllers;

[Route("")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _game;
    private readonly ISessionService _session;
    private readonly IUserInput _userInput;

    public GameController(
        IGameService game,
        ISessionService session,
        IUserInput userInput
    )
    {
        _game = game;
        _session = session;
        _userInput = userInput;
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

        _userInput.Text = payload.UserInput;
        var response = await _game.Respond();
        return Ok(response.GetPayload());
    }
}
