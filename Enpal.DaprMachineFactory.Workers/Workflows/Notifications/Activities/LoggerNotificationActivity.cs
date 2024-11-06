using Dapr.Workflow;

namespace Enpal.DaprMachineFactory.Workers.Workflows.Notifications.Activities;

public class LoggerNotificationActivity(ILogger<LoggerNotificationActivity> logger)
    : WorkflowActivity<Notification, bool>
{
    private const bool Success = true;

    private readonly ILogger<LoggerNotificationActivity> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    public override Task<bool> RunAsync(WorkflowActivityContext context, Notification input)
    {
        _logger.LogInformation("Notification: [{id}], Payload: {payload}", input.TriggerId, input.Payload);

        return Task.FromResult<bool>(Success);
    }
}