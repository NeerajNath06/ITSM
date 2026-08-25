using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Tenancy.CreateOrganization;

public sealed record CreateOrganizationCommand(
    Guid TenantId,
    string Name,
    string Code,
    Guid? ParentOrganizationId) : IRequest<CreateOrganizationResult>;

public sealed record CreateOrganizationResult(Guid OrganizationId);
