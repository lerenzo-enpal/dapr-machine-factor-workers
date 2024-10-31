using System.Text.Json;
using Dapr;
using Enpal.DaprMachineFactory.Workers.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/internal/conveyorBelt",
    [Topic("factory-conveyor-belt-kafka", "conveyor-belt")]
    async (ILogger<Program> logger, ConveyorBeltPayload conveyorBeltPayload) =>
    {
        logger.LogInformation(JsonSerializer.Serialize(conveyorBeltPayload));
        logger.LogInformation("wooohhhh");
        await Task.Delay(4);
        return 3;
    });

app.MapPost("/internal/conveyorBelt",
    [Topic("factory-conveyor-belt-rabbitmq", "conveyor-belt")]
    async (ILogger<Program> logger, ConveyorBeltPayload conveyorBeltPayload) =>
    {
        logger.LogInformation(JsonSerializer.Serialize(conveyorBeltPayload));
        logger.LogInformation("wooohhhh");
        await Task.Delay(4);
        return 3;
    });

app.MapPost("/conveyor-belt-scheduler",
    (ILogger<Program> logger) =>
    {
        logger.LogInformation("Prepare a Payload.");

        ConveyorBeltPayload conveyorBeltPayload = new(Guid.NewGuid().ToString(), new List<Asset>()
        {
            new(AssetClass.Battery, Guid.NewGuid().ToString()),
            new(AssetClass.Heatpump, Guid.NewGuid().ToString()),
            new(AssetClass.Inverter, Guid.NewGuid().ToString()),
            new(AssetClass.Wallbox, Guid.NewGuid().ToString())
        });
        
        return Results.Ok();
    });

app.UseCloudEvents();
app.MapSubscribeHandler();
app.Run();