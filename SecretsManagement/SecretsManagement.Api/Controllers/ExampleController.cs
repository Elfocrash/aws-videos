using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SecretsManagement.Api.Configuration;

namespace SecretsManagement.Api.Controllers;

[ApiController]
public class ExampleController : ControllerBase
{
    private readonly IOptionsMonitor<DatabaseSettings> _databaseSettings;

    public ExampleController(IOptionsMonitor<DatabaseSettings> databaseSettings)
    {
        _databaseSettings = databaseSettings;
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        var settings = _databaseSettings.CurrentValue;
        return Ok(settings);
    }
}
