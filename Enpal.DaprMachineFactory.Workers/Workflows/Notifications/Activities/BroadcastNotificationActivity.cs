using Dapr.Client;
using Dapr.Workflow;

namespace Enpal.DaprMachineFactory.Workers.Workflows.Notifications.Activities;

public class BroadcastNotificationActivity(DaprClient daprClient) : WorkflowActivity<Notification, bool>
{
    private readonly DaprClient _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private const bool Success = true;
    private const string FactoryAnnouncements = "factory-announcements";

    public override async Task<bool> RunAsync(WorkflowActivityContext context, Notification input)
    {
        await _daprClient.InvokeMethodAsync(HttpMethod.Get, FactoryAnnouncements, string.Empty);
        return Success;
    }
}