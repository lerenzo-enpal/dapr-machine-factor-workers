using System.Text.Json;
using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Enpal.DaprMachineFactory.Workers.Consumers;

public class Consumers
{
    public static string? WorkflowId;
    
    [Topic("factory-conveyor-belt-kafka", "conveyor-belt")]
    [Topic("factory-conveyor-belt-rabbitmq", "conveyor-belt")]
    public static async Task ConveyorBeltConsumer (ILogger<Consumers> logger, DaprClient client, DaprWorkflowClient daprWorkflowClient, Asset asset)
    {
        logger.LogInformation($"wooohhhh kafka {JsonSerializer.Serialize(asset)}");
        await client.SaveStateAsync("factory-warehouse", $"assets.{asset.EuropeanArticleNumber}", asset);
        if(WorkflowId is not null)
            await daprWorkflowClient.RaiseEventAsync(WorkflowId, asset.AssetClass.ToString(), asset);
    }
}