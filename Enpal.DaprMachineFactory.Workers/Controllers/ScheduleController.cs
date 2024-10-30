using Dapr.Client;
using Enpal.DaprMachineFactory.Workers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Enpal.DaprMachineFactory.Workers.Controllers;

[ApiController]
[Route("conveyor-belt-scheduler")]
public class ScheduleController(ILogger<ScheduleController> logger, DaprClient client) : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly DaprClient _client = client;

    [HttpPost]
    public async Task<IResult> Exec()
    {
        _logger.LogInformation("Prepare a Payload.");

        ConveyorBeltPayload conveyorBeltPayload = new(Guid.NewGuid().ToString(), new List<Asset>()
        {
            new(AssetClass.Battery, Guid.NewGuid().ToString()),
            new(AssetClass.Heatpump, Guid.NewGuid().ToString()),
            new(AssetClass.Inverter, Guid.NewGuid().ToString()),
            new(AssetClass.Wallbox, Guid.NewGuid().ToString())
        });
        
        await _client.PublishEventAsync("factory-conveyor-belt-rabbitmq", "conveyor-belt", conveyorBeltPayload);
        return Results.Ok();
    }
}