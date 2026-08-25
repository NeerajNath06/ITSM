using Host.Workers;

// Fully qualified: this project's own root namespace is "Host.Workers",
// which shadows Microsoft.Extensions.Hosting.Host if referenced bare.
var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OutboxRelayWorker>();
builder.Services.AddHealthChecks();
// TODO(Phase 2): recurring/scheduled jobs (SLA clock evaluation, scheduled
// reindex triggers) via Hangfire or Quartz.NET, per
// docs/backend/dotnet-architecture.md and ADR-022. Workflow timers are
// Temporal's responsibility (ADR-023), not this host's.

var host = builder.Build();
host.Run();
