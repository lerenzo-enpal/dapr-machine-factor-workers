using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;
using Enpal.DaprMachineFactory.Workers.Workflows.Activities;

namespace Enpal.DaprMachineFactory.Workers.Workflows;

public class AssemblyLineWorkflow : Workflow<bool, bool>
{
    public override async Task<bool> RunAsync(WorkflowContext context, bool input)
    {
        /*
         * Create Commands
         */

        var battery = context.WaitForExternalEventAsync<Asset>(AssetClass.Battery.ToString());
        var wallbox = context.WaitForExternalEventAsync<Asset>(AssetClass.Wallbox.ToString());
        var heatpump = context.WaitForExternalEventAsync<Asset>(AssetClass.Heatpump.ToString());
        var inverter = context.WaitForExternalEventAsync<Asset>(AssetClass.Inverter.ToString());
        var assets = await Task.WhenAll([battery, wallbox, heatpump, inverter]);

        var conveyorBeltPayload = await context.CallActivityAsync<ConveyorBeltPayload>(nameof(CompleteAssetActivity), assets);
        await context.CallActivityAsync<ConveyorBeltPayload>(nameof(NotifyActivity), conveyorBeltPayload);
        context.ContinueAsNew();

        return true;
    }
}