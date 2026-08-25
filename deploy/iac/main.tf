# Phase 1 skeleton only. This provisions the one resource every later
# module in this directory will depend on — the per-environment namespace
# boundary called for in docs/infrastructure/self-hosted-production-architecture.md
# ("Namespace-per-environment isolation, NetworkPolicies restricting
# pod-to-pod traffic, RBAC scoped per service"). Modules for PostgreSQL
# (CloudNativePG/Zalando operator), Valkey, RabbitMQ, MinIO, OpenSearch,
# Keycloak, OpenBao, Harbor, and the observability stack land incrementally,
# each when the phase that actually needs it is implemented — not
# speculatively here. See docs/roadmap/implementation-roadmap.md.

resource "kubernetes_namespace" "itsm" {
  metadata {
    name = "itsm-${var.environment}"

    labels = {
      "app.kubernetes.io/part-of"    = "itsm"
      "app.kubernetes.io/managed-by" = "opentofu"
      "itsm.io/environment"          = var.environment
    }
  }
}
