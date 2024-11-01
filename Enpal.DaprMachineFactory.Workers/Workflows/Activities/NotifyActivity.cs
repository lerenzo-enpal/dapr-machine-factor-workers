using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;

namespace Enpal.DaprMachineFactory.Workers.Workflows.Activities;

public class NotifyActivity(DaprClient client, ILogger<NotifyActivity> logger) : WorkflowActivity<ConveyorBeltPayload, ConveyorBeltPayload>
{
    private const string PubsubName = "factory-conveyor-belt-rabbitmq";
    private const string PubsubName2 = "factory-conveyor-belt-kafka";
    public override async Task<ConveyorBeltPayload> RunAsync(WorkflowActivityContext context, ConveyorBeltPayload input)
    {
        //service invocation 
        // or pubsub 
        await client.PublishEventAsync(PubsubName, "completed-systems", input);
        await client.PublishEventAsync(PubsubName2, "completed-systems", input);
        logger.LogInformation($"\n\n\n\n  Completed Item {input} \n\n\n\n");
        return input;
    }
}