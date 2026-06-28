using SkillNet.Domain.Entities;

namespace SkillNet.Domain.Interfaces;

public interface IMembershipApplicationRepository
{
    Task<MembershipApplication?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MembershipApplication?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<bool> HasPendingAsync(string userId, CancellationToken ct = default);
    Task AddAsync(MembershipApplication application, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
