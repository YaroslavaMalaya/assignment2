using LastSeenTask;
using Microsoft.AspNetCore.Mvc;

namespace LastSeenTaskAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ShowUsers _showUsers;

    public UsersController(ShowUsers showUsers)
    {
        _showUsers = showUsers;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserResponse>> Get([FromQuery] string lang = "en")
    {
        var users = _showUsers.UsersShow(lang);
        return Ok(users);
    }
}