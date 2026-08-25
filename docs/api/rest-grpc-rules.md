# REST + gRPC Rules

**Phase:** 15 · Clear, non-optional rules for when each protocol is used —
prevents the drift where gRPC creeps into places it doesn't belong (Master
AI Command: "Do not use gRPC everywhere").

## REST — required for

| Use case | Why |
|---|---|
| React apps (Employee Portal, Agent Workspace, Admin Console) via BFF | Browser-native, cacheable, debuggable with standard tooling, no code-gen coupling between frontend release and backend contract |
| Public/partner-facing APIs (Table API, Domain APIs) | Broadest client compatibility — partners integrate via curl/Postman/any HTTP client, no gRPC toolchain requirement |
| Webhooks (inbound and outbound) | The entire external ecosystem (monitoring tools, ITSM-adjacent SaaS) speaks HTTP/REST, not gRPC |
| Anything needing human-readable, cacheable, browser-network-tab-debuggable traffic | Operational debuggability during incidents (ironically, our own incident response) matters more here than raw throughput |

## gRPC — allowed only for

| Use case | Why gRPC earns its complexity here |
|---|---|
| Internal service-to-service calls between `Host.Api` and an extracted satellite service (Notification, Search, AI inference) where call volume/latency is measured to matter | Binary protocol + HTTP/2 multiplexing measurably reduces overhead versus JSON/REST at high internal call volume |
| Strongly-typed internal contracts where a shared `.proto` definition materially reduces integration bugs between two .NET services we both control | Compile-time contract checking on both ends of a call we own end-to-end |
| Streaming use cases (e.g. a future live event/telemetry stream between the core and an observability satellite) | REST/HTTP polling is a poor fit for genuinely streaming data; gRPC streaming is a good fit |

## gRPC is NOT allowed for

- Any call originating from or terminating in React — browsers do not
  support native gRPC without a gRPC-Web proxy layer that adds complexity
  REST doesn't need for our UI traffic.
- Any public or partner-facing contract — forces every integrator to adopt
  our specific tooling.
- Calls between core modules **within** the modular monolith — those are
  in-process method calls through an Application-layer interface, not
  network calls of any protocol (see `bounded-contexts.md` cross-context
  rules). Using gRPC (or REST) for what should be an in-process call is the
  "distributed monolith" anti-pattern.
- Default choice for a new satellite service "because it's more modern" —
  each gRPC adoption must cite a measured or clearly anticipated
  throughput/latency need per the table above, not aesthetic preference.

## Decision checklist before choosing gRPC for a new internal call

1. Is this call crossing a real process/service boundary (not an in-process
   module boundary)? If no → stop, use an in-process interface call.
2. Do both ends belong to systems we control end-to-end (no external
   partner in the path)? If no → REST.
3. Is there a measured or clearly-anticipated latency/throughput
   requirement that JSON/REST would not comfortably meet? If no → REST is
   simpler and sufficient; use it.
4. Only if all three are "yes": use gRPC, with a versioned `.proto` contract
   reviewed the same way a public REST contract change would be.

## Versioning parity

Both REST and gRPC contracts follow the same versioning discipline as
`docs/api/api-design-rules.md` (Phase 16) — a gRPC service is not exempt
from the deprecation/versioning process just because it's internal-only;
internal consumers still break if a contract changes without warning.

---

*Next: `docs/api/api-design-rules.md` (Phase 16).*
