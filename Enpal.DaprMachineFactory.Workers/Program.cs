using System.Text.Json;
using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;
using Enpal.DaprMachineFactory.Workers.Workflows;
using Enpal.DaprMachineFactory.Workers.Workflows.Activities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient(b =>
{
    b.UseJsonSerializationOptions(new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    });
});
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<AssemblyLineWorkflow>();
    options.RegisterActivity<CompleteAssetActivity>();
    options.RegisterActivity<NotifyActivity>();
});
var app = builder.Build();
var client = new DaprClientBuilder().UseTimeout(TimeSpan.FromSeconds(2)).Build();
var storeName = "factory-warehouse";
var workflowId = new Guid();
var daprWorkflowClient = app.Services.GetRequiredService<DaprWorkflowClient>();

app.MapPost("/internal/conveyorBelt-kafka",
    [Topic("factory-conveyor-belt-kafka", "conveyor-belt")]
    (ILogger<Program> logger, ConveyorBeltPayload conveyorBeltPayload) =>
    {
        logger.LogInformation($"wooohhhh kafka {JsonSerializer.Serialize(conveyorBeltPayload)}");
        return 3;
    });

app.MapPost("/internal/conveyorBelt-rabbitmq",
    [Topic("factory-conveyor-belt-rabbitmq", "conveyor-belt")]
    async (ILogger<Program> logger, Asset asset) =>
    {
        logger.LogInformation($"wooohhhh rabbitmq {JsonSerializer.Serialize(asset)}");
        await client.SaveStateAsync(storeName, $"assets.{asset.EuropeanArticleNumber}", asset);

        var wf = await daprWorkflowClient.GetWorkflowStateAsync(workflowId.ToString(), false);
        if (!wf.Exists)
        {
            await daprWorkflowClient.ScheduleNewWorkflowAsync(
                nameof(AssemblyLineWorkflow), 
                workflowId.ToString(), 
                true
            );

            await daprWorkflowClient.WaitForWorkflowStartAsync(workflowId.ToString(), false);
        }
        
        //call workflow event 
        await daprWorkflowClient.RaiseEventAsync(
            workflowId.ToString(), 
            asset.AssetClass.ToString(),
            asset
        );
        
        return 3;
    });

app.UseCloudEvents();
app.MapSubscribeHandler();

// await daprWorkflowClient.ScheduleNewWorkflowAsync(
//     nameof(AssemblyLineWorkflow), 
//     workflowId.ToString(), 
//     true
// );

app.Run();



//start workflow (my_workflow)
    //for each new item 
    //add type 1/2/3 arrived 
    //when [1, 2, 3] 
    //  -> save bundle (to a new table) 
    //  -> send service invocation back to main factory 