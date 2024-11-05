using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Models;
using Enpal.DaprMachineFactory.Workers.Workflows.AssemblyLine.Activities;

namespace Enpal.DaprMachineFactory.Workers.Workflows.AssemblyLine;

public class AssemblyLineWorkflow : Workflow<bool, bool>
{
    public override async Task<bool> RunAsync(WorkflowContext context, bool input)
    {
        context.SetCustomStatus("WAITING_FOR_SYSTEM_PARTS");
        
        Task<Asset> battery = context.WaitForExternalEventAsync<Asset>(AssetClass.Battery.ToString());
        Task<Asset> wallbox = context.WaitForExternalEventAsync<Asset>(AssetClass.Wallbox.ToString());
        Task<Asset> heatpump = context.WaitForExternalEventAsync<Asset>(AssetClass.Heatpump.ToString());
        Task<Asset> inverter = context.WaitForExternalEventAsync<Asset>(AssetClass.Inverter.ToString());

        Asset[] assets = await Task.WhenAll([battery, wallbox, heatpump, inverter]);

        ConveyorBeltPayload conveyorBeltPayload =
            await context.CallActivityAsync<ConveyorBeltPayload>(nameof(CompleteAssetActivity), assets);

        context.SetCustomStatus($"SYSTEM_ASSEMBLY_COMPLETED_AT_{DateTime.Now:t}: {conveyorBeltPayload}");
        await context.CallActivityAsync<ConveyorBeltPayload>(nameof(NotifyActivity), conveyorBeltPayload);
        context.ContinueAsNew(true);
        return true;
    }
}