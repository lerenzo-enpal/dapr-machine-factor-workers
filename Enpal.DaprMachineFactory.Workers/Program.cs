using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Consumers;
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
app.MapPost("/startWorkflow/{id}", (DaprWorkflowClient daprWorkflowClient, string id) =>
{
    Consumers.WorkflowId = id;
    return daprWorkflowClient.ScheduleNewWorkflowAsync(nameof(AssemblyLineWorkflow), id, true);
});
app.MapPost("/stopWorkflow/{id}", (DaprWorkflowClient daprWorkflowClient, string id) =>
{
    Consumers.WorkflowId = null;
    return daprWorkflowClient.TerminateWorkflowAsync(id);
});
app.MapPost("/getWorkflowStatus/{id}", (DaprWorkflowClient daprWorkflowClient, string id) =>
    daprWorkflowClient.GetWorkflowStateAsync(id)
);
app.MapPost("/conveyorBelt", Consumers.ConveyorBeltConsumer);

app.UseCloudEvents();
app.MapSubscribeHandler();
app.Run();