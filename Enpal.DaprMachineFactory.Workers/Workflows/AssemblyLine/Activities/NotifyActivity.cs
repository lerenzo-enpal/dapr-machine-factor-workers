using System.Text.Json;
using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;

namespace Enpal.DaprMachineFactory.Workers.Workflows.AssemblyLine.Activities;

public class NotifyActivity(DaprClient client, ILogger<NotifyActivity> logger)
    : WorkflowActivity<ConveyorBeltPayload, ConveyorBeltPayload>
{
    private const string RabbitPubsubName = "factory-conveyor-belt-rabbitmq";
    private const string KafkaPubsubName = "factory-conveyor-belt-kafka";
    
    private const string CompletedSystemsTopicName = "completed-systems";
    private const string FactoryAnnouncements = "factory-announcements";
    
    public override async Task<ConveyorBeltPayload> RunAsync(WorkflowActivityContext context, ConveyorBeltPayload input)
    {
        //service invocation 
        // or pubsub 
        await client.PublishEventAsync(RabbitPubsubName, CompletedSystemsTopicName, input);
        await client.PublishEventAsync(KafkaPubsubName, CompletedSystemsTopicName, input);
        
        logger.LogInformation($"\n\n\n\n  Completed Item {input} \n\n\n\n");
        return input;
    }
}