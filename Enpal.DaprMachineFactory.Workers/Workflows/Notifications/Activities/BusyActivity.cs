using Dapr.Workflow;

namespace Enpal.DaprMachineFactory.Workers.Workflows.Notifications.Activities;

public class BusyActivity : WorkflowActivity<Notification, bool>
{
    private const bool Success = true;

    public override async Task<bool> RunAsync(WorkflowActivityContext context, Notification input)
    {
        Random rand = new();
        await Task.Delay(TimeSpan.FromMilliseconds(rand.Next(100, 1_000)));
        return Success;
    }
}