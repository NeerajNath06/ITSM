using ITSM.BuildingBlocks.Exceptions;
using ITSM.BuildingBlocks.Messaging;
using Platform.Application.Common;
using Platform.Domain.Tenancy;

namespace Platform.Application.Tenancy.CreateOrganization;

public sealed class CreateOrganizationHandler(IOrganizationRepository organizations, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrganizationCommand, CreateOrganizationResult>
{
    public async Task<CreateOrganizationResult> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (await organizations.GetByCodeAsync(request.Code, cancellationToken) is not null)
        {
            throw new ConflictException($"An organization with code '{request.Code}' already exists in this tenant.");
        }

        var organization = Organization.Create(request.TenantId, request.Name, request.Code, request.ParentOrganizationId);
        organizations.Add(organization);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrganizationResult(organization.Id);
    }
}
