using Microsoft.AspNetCore.Mvc;

namespace WebProShop.Controllers;

[ApiController]
[Route("/")]
public sealed class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
}
