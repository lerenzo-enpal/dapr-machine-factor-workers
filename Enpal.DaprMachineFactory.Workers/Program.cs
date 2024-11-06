using System.Diagnostics;
using System.Text.Json;
using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;
using Enpal.DaprMachineFactory.Workers.Workflows;
using Enpal.DaprMachineFactory.Workers.Workflows.AssemblyLine;
using Enpal.DaprMachineFactory.Workers.Workflows.AssemblyLine.Activities;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<AssemblyLineWorkflow>();
    options.RegisterActivity<CompleteAssetActivity>();
    options.RegisterActivity<NotifyActivity>();
});

WebApplication app = builder.Build();

string workflowId = "assembly-line-1";

//Start workflow
app.MapPost("/startWorkflow",
    async (ILogger<Program> logger, DaprClient client, DaprWorkflowClient daprWorkflowClient) =>
    {
        await daprWorkflowClient.ScheduleNewWorkflowAsync(nameof(AssemblyLineWorkflow), workflowId, true);
        await client.SaveStateAsync("worker-schedule", "conveyor-belt-active", true);
        logger.LogInformation(
            "Workflow [{workflowId}] started. Call `/workflowStatus` to get workflow details.", workflowId);
    }
);
//Stop workflow
app.MapPost("/stopWorkflow", async(DaprClient client,DaprWorkflowClient daprWorkflowClient) =>
    {
        await client.SaveStateAsync("worker-schedule", "conveyor-belt-active", false);
        return daprWorkflowClient.TerminateWorkflowAsync(workflowId);
    });

//Get Workflow Status
app.MapGet("/workflowStatus",
    (DaprWorkflowClient daprWorkflowClient) => daprWorkflowClient.GetWorkflowStateAsync(workflowId));


//Resume Workflow
app.MapPost("/resumeWorkflow",
    (DaprWorkflowClient daprWorkflowClient) => daprWorkflowClient.ResumeWorkflowAsync(workflowId));

//Purge Workflow
app.MapPost("/purgeWorkflow", async (DaprClient client, DaprWorkflowClient daprWorkflowClient) =>
    {
        await client.SaveStateAsync("worker-schedule", "conveyor-belt-active", false);
        await daprWorkflowClient.PurgeInstanceAsync(workflowId);
        await Task.Delay(TimeSpan.FromSeconds(3));
        await daprWorkflowClient.ScheduleNewWorkflowAsync(nameof(AssemblyLineWorkflow), workflowId, true);
    });

//conveyorBelt 
app.MapPost("/conveyorBelt",
    [Topic("factory-conveyor-belt-kafka", "conveyor-belt")] [Topic("factory-conveyor-belt-rabbitmq", "conveyor-belt")]
    async (ILogger<Program> logger, DaprClient client, DaprWorkflowClient daprWorkflowClient, Asset asset) =>
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
        bool conveyorBeltActive = await client.GetStateAsync<bool>("worker-schedule", "conveyor-belt-active");
        if (!conveyorBeltActive)
        {
            logger.LogInformation("Workflow [{workflowId}] does not yet exists. Call `/startWorkflow`", workflowId);
            await Task.Delay(TimeSpan.FromSeconds(3));
            return;
        }

        logger.LogInformation("[PubSub Conveyor Belt Handler]: {asset}", JsonSerializer.Serialize(asset));
        await client.SaveStateAsync("factory-warehouse", $"assets.{asset.EuropeanArticleNumber}", asset);
        await daprWorkflowClient.RaiseEventAsync(workflowId, asset.AssetClass.ToString(), asset);
    }
);

/*
 * Start Server
 */
app.UseCloudEvents();
app.MapSubscribeHandler();
app.Run();