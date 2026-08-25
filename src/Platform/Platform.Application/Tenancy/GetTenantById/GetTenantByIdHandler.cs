using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;

namespace Platform.Application.Tenancy.GetTenantById;

public sealed class GetTenantByIdHandler(ITenantRepository tenants) : IRequestHandler<GetTenantByIdQuery, TenantDto>
{
    public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await tenants.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant {request.TenantId} was not found.");

        return new TenantDto(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            tenant.IsolationTier.ToString(),
            tenant.Status.ToString(),
            tenant.CreatedAt);
    }
}
