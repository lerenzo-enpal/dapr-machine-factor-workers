using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;

namespace Enpal.DaprMachineFactory.Workers.Workflows.Activities;

public class CompleteAssetActivity(DaprClient client) : WorkflowActivity<List<Asset>, ConveyorBeltPayload>
{
    private const string StoreName = "factory-warehouse-completed-items";
    
    public override async Task<ConveyorBeltPayload> RunAsync(WorkflowActivityContext context, List<Asset> input)
    {
        var id = Guid.NewGuid();
        var conveyorBeltPayload = new ConveyorBeltPayload(id.ToString(), input);
        await client.SaveStateAsync(StoreName, $"assembled.{id}", conveyorBeltPayload);

        return conveyorBeltPayload;
    }
}