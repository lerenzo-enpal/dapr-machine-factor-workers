using Dapr.Workflow;
using Enpal.DaprMachineFactory.Workers.Workflows.Notifications.Activities;


namespace Enpal.DaprMachineFactory.Workers.Workflows.Notifications;

public record Notification(string TriggerId, string Payload);

public class NotificationWorkflow : Workflow<Notification, bool>
{
    private const bool Success = true;
    public override async Task<bool> RunAsync(WorkflowContext context, Notification input)
    {
        context.SetCustomStatus("Notifying stakeholders...");

        Task loggerTask = context.CallActivityAsync(nameof(LoggerNotificationActivity), input);
        Task busyBoxTask = context.CallActivityAsync(nameof(BusyActivity), input);
        Task broadcastTask = context.CallActivityAsync(nameof(BroadcastNotificationActivity), input);

        await Task.WhenAll([busyBoxTask, loggerTask, broadcastTask]).ConfigureAwait(false);

        context.SetCustomStatus("Finished Notifying stakeholders");
        
        return Success;
    }
}