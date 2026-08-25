namespace Host.Workers;

/// <summary>
/// Polls the outbox table and publishes unpublished domain/integration
/// events to RabbitMQ, per docs/integration/event-driven-architecture.md
/// "Outbox pattern (publish side)". Placeholder loop until Phase 2 wires
/// Platform.Infrastructure's outbox repository and a real RabbitMQ
/// publisher.
/// </summary>
public class OutboxRelayWorker(ILogger<OutboxRelayWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("Outbox relay tick (placeholder — no outbox store wired yet).");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
