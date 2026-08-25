using Platform.Domain.Identity;

namespace Platform.Application.Identity;

public interface IIdentityGroupMemberRepository
{
    Task<bool> ExistsAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);

    void Add(IdentityGroupMember member);
}
