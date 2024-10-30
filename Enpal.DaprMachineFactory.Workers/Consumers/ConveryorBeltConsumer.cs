using System.Text.Json;
using Dapr;
using Enpal.DaprMachineFactory.Workers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Enpal.DaprMachineFactory.Workers.Consumers;

[ApiController]
public class ConveyorBeltConsumer(ILogger<ConveyorBeltConsumer> logger): Controller
{
    const string PubSubName = "factory-conveyor-belt-rabbitmq";
    private const string PubSubTopic = "conveyor-belt";
    
    [HttpPost("internal/conveyorBelt")]
    [Topic(PubSubName, PubSubTopic)]
    public async Task<ActionResult<int>> OnConveyorBeltMessage(ConveyorBeltPayload conveyorBeltPayload)
    {
        logger.LogInformation(JsonSerializer.Serialize(conveyorBeltPayload));
        logger.LogInformation("wooohhhh");
        await Task.Delay(4);
        return 3;
    }
}