using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;

namespace Platform.Application.Tenancy.ReactivateTenant;

public sealed class ReactivateTenantHandler(ITenantRepository tenants, IUnitOfWork unitOfWork)
    : IRequestHandler<ReactivateTenantCommand, Unit>
{
    public async Task<Unit> Handle(ReactivateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenants.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant {request.TenantId} was not found.");

        tenant.Reactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
