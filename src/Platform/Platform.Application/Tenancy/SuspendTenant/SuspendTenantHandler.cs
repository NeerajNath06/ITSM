using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Tenancy.SuspendTenant;

public sealed class SuspendTenantHandler(ITenantRepository tenants, IUnitOfWork unitOfWork)
    : IRequestHandler<SuspendTenantCommand, Unit>
{
    public async Task<Unit> Handle(SuspendTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenants.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant {request.TenantId} was not found.");

        tenant.Suspend();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
