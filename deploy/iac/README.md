# IaC (OpenTofu)

Phase 1 skeleton for the self-hosted Kubernetes/K3s target described in
`docs/infrastructure/self-hosted-production-architecture.md`. `dev`,
`staging`, and `prod` run the same modules, parameterized per environment
per `docs/devops/cicd.md` ("Environment parity").

Today this only provisions the per-environment namespace. Modules for the
stateful services and platform services in the topology diagram
(PostgreSQL, Valkey, RabbitMQ, MinIO, OpenSearch, Qdrant, Keycloak,
OpenBao, Harbor, the Prometheus/Loki/Tempo/Grafana stack) are added
incrementally as the phase that needs them lands, not provisioned ahead of
a real consumer.

## Usage

```sh
tofu init
tofu plan  -var-file=environments/dev.tfvars
tofu apply -var-file=environments/dev.tfvars
```

Swap the `-var-file` for `staging.tfvars` / `prod.tfvars` for the other
environments. `kubeconfig_path` (see `variables.tf`) points at the target
cluster's kubeconfig; it is not committed here — each operator/CI runner
supplies its own.
