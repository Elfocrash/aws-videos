using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ParameterStoreExample.Api.Controllers;

[ApiController]
public class ExamplesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ExampleSettings _settings;
    private readonly ExampleSettings _optionsMonitorSettings;
    private readonly ExampleSettings _optionsSnapshotSettings;

    public ExamplesController(IConfiguration configuration,
        IOptions<ExampleSettings> settings,
        IOptionsMonitor<ExampleSettings> optionsMonitor,
        IOptionsSnapshot<ExampleSettings> optionsSnapshot)
    {
        _configuration = configuration;
        _settings = settings.Value;
        _optionsMonitorSettings = optionsMonitor.CurrentValue;
        _optionsSnapshotSettings = optionsSnapshot.Value;
    }

    [HttpGet("example")]
    public IActionResult Get()
    {
        var value = _configuration.GetValue<string>("ExampleSettings:ConnectionString");
        return Ok(new
        {
            fromOption = _settings.ConnectionString,
            fromOptionMonitor = _optionsMonitorSettings.ConnectionString,
            fromOptionSnapshot = _optionsSnapshotSettings.ConnectionString,
            fromConfig = value
        });
    }
}
