using Dapr.Client;
using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprWorkflow(options =>
{
    
});

WebApplication app = builder.Build();

string workflowId = "assembly-line-1";

//Start workflow
app.MapPost("/startWorkflow",
    async (ILogger<Program> logger, DaprClient client, DaprWorkflowClient daprWorkflowClient) =>
    {

    }
);
//Stop workflow
app.MapPost("/stopWorkflow", async(DaprClient client,DaprWorkflowClient daprWorkflowClient) =>
    {

    });

//Get Workflow Status
app.MapGet("/workflowStatus",
    (DaprWorkflowClient daprWorkflowClient) =>
    {
        
    });


//Resume Workflow
app.MapPost("/resumeWorkflow",
    (DaprWorkflowClient daprWorkflowClient) =>
    {
        
    });

//Purge Workflow
app.MapPost("/purgeWorkflow", async (DaprClient client, DaprWorkflowClient daprWorkflowClient) =>
    {

    });

//conveyorBelt 
app.MapPost("/conveyorBelt",
    async (ILogger<Program> logger, DaprClient client, DaprWorkflowClient daprWorkflowClient, Asset asset) =>
    {
        
    }
);

/*
 * Start Server
 */
app.UseCloudEvents();
app.MapSubscribeHandler();
app.Run();