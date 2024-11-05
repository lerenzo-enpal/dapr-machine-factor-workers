using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;

namespace Enpal.DaprMachineFactory.Workers.Workflows.AssemblyLine.Activities;

public class CompleteAssetActivity(DaprClient client, ILogger<CompleteAssetActivity> logger) : WorkflowActivity<List<Asset>, ConveyorBeltPayload>
{
    private const string CompletedAssetsStoreName = "factory-warehouse-completed-items";
    private readonly ILogger<CompleteAssetActivity> _logger = logger;

    public override async Task<ConveyorBeltPayload> RunAsync(WorkflowActivityContext context, List<Asset> input)
    {
        string id = Guid.NewGuid().ToString();
        ConveyorBeltPayload conveyorBeltPayload = new(id, input);
        await client.SaveStateAsync(CompletedAssetsStoreName, $"assembled.{id}", conveyorBeltPayload);
        _logger.LogInformation("System Assembled!");
        return conveyorBeltPayload;
    }
}