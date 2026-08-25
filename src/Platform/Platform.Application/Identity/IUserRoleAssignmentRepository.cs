using Platform.Domain.Identity;

namespace Platform.Application.Identity;

public interface IUserRoleAssignmentRepository
{
    Task<UserRoleAssignment?> FindAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);

    void Add(UserRoleAssignment assignment);

    void Remove(UserRoleAssignment assignment);
}
