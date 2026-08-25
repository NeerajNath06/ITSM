using Microsoft.EntityFrameworkCore;
using Platform.Application.Identity;
using Platform.Domain.Identity;

namespace Platform.Infrastructure.Persistence.Repositories;

public sealed class IdentityGroupMemberRepository(PlatformDbContext context) : IIdentityGroupMemberRepository
{
    public Task<bool> ExistsAsync(Guid groupId, Guid userId, CancellationToken cancellationToken) =>
        context.IdentityGroupMembers.AnyAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);

    public void Add(IdentityGroupMember member) => context.IdentityGroupMembers.Add(member);
}
