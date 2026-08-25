using Microsoft.EntityFrameworkCore;
using Platform.Application.Identity;
using Platform.Domain.Identity;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class UserRoleAssignmentRepository(PlatformDbContext context) : IUserRoleAssignmentRepository
{
    public Task<UserRoleAssignment?> FindAsync(Guid userId, Guid roleId, CancellationToken cancellationToken) =>
        context.UserRoleAssignments.SingleOrDefaultAsync(a => a.UserId == userId && a.RoleId == roleId, cancellationToken);

    public void Add(UserRoleAssignment assignment) => context.UserRoleAssignments.Add(assignment);

    public void Remove(UserRoleAssignment assignment) => context.UserRoleAssignments.Remove(assignment);
}
