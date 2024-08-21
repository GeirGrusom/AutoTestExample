using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace WebProShop.Controllers;

[ApiController]
[Route("/")]
public sealed class HealthCheckController : ControllerBase
{
    [ExcludeFromCodeCoverage]
    [HttpGet]
    public ActionResult Get() => Ok();
}
