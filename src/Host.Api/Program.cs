using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Wave 0: bare composition root. Module registration (IModule per
// docs/backend/dotnet-architecture.md "Dependency injection") lands
// starting Phase 2 (Identity/Tenancy/ACL) — see
// docs/roadmap/implementation-roadmap.md.
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
// TODO(Phase 2): AddNpgSql / AddRabbitMQ / AddRedis (Valkey) readiness
// checks once Platform.Infrastructure wires real connections
// (docs/observability/observability.md "Health checks").

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Liveness: process is up, no dependency checks — used for container
// restart decisions, never gates traffic admission.
app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false
});

// Readiness: dependency checks — gates traffic admission at the
// gateway/load balancer. Empty predicate until Phase 2 adds real checks.
app.MapHealthChecks("/healthz/ready");

app.Run();
