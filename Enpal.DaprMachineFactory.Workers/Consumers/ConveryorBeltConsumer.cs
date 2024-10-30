using System.Text.Json;
using Enpal.DaprMachineFactory.Workers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Enpal.DaprMachineFactory.Workers.Consumers;

[ApiController]
public class ConveyorBeltConsumer(ILogger<ConveyorBeltConsumer> logger): Controller
{
    
    [HttpPost("internal/conveyorBelt")]
    public async Task<ActionResult<int>> OnConveyorBeltMessage(ConveyorBeltPayload conveyorBeltPayload)
    {
        logger.LogInformation(JsonSerializer.Serialize(conveyorBeltPayload));
        logger.LogInformation("wooohhhh");
        await Task.Delay(4);
        return 3;
    }
}