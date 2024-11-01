using System.Text.Json;
using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;
using Enpal.DaprMachineFactory.Workers.Workflows;
using Enpal.DaprMachineFactory.Workers.Workflows.Activities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<AssemblyLineWorkflow>();
    options.RegisterActivity<CompleteAssetActivity>();
    options.RegisterActivity<NotifyActivity>();
});
var app = builder.Build();

string? workflowId = null;
app.MapPost("/startWorkflow/{id}", (DaprWorkflowClient daprWorkflowClient, string? id) =>
{
    workflowId = id;
    return daprWorkflowClient.ScheduleNewWorkflowAsync(nameof(AssemblyLineWorkflow), id, true);
});
app.MapPost("/stopWorkflow/{id}", (DaprWorkflowClient daprWorkflowClient, string id) =>
{
    workflowId = null;
    return daprWorkflowClient.TerminateWorkflowAsync(id);
});
app.MapPost("/getWorkflowStatus/{id}", (DaprWorkflowClient daprWorkflowClient, string id) =>
    daprWorkflowClient.GetWorkflowStateAsync(id)
);

app.MapPost("/conveyorBelt",
    [Topic("factory-conveyor-belt-kafka", "conveyor-belt")] 
    [Topic("factory-conveyor-belt-rabbitmq", "conveyor-belt")]
    async (ILogger<Program> logger, DaprClient client, DaprWorkflowClient daprWorkflowClient, Asset asset) =>
    {
        logger.LogInformation("[PubSub Conveyor Belt Handler]: {asset}", JsonSerializer.Serialize(asset));
        await client.SaveStateAsync("factory-warehouse", $"assets.{asset.EuropeanArticleNumber}", asset);
        if (workflowId is not null)
            await daprWorkflowClient.RaiseEventAsync(workflowId, asset.AssetClass.ToString(), asset);
    }
);


/**
 * Start Server
 */
app.UseCloudEvents();
app.MapSubscribeHandler();
app.Run();