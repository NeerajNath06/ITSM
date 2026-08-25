variable "environment" {
  description = "Deployment environment (dev, staging, prod). Drives naming and namespace isolation — see docs/architecture/multi-tenancy.md and the namespace-per-environment note in docs/infrastructure/self-hosted-production-architecture.md."
  type        = string

  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "environment must be one of: dev, staging, prod."
  }
}

variable "kubeconfig_path" {
  description = "Path to the kubeconfig for the target K3s cluster."
  type        = string
  default     = "~/.kube/config"
}
