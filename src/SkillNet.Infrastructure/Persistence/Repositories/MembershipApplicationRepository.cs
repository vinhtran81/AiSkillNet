using Microsoft.EntityFrameworkCore;
using SkillNet.Domain.Entities;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Infrastructure.Persistence.Repositories;

public class MembershipApplicationRepository(SkillNetDbContext db) : IMembershipApplicationRepository
{
    public async Task<MembershipApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.MembershipApplications
            .Include(a => a.ServicePackage)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<MembershipApplication?> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => await db.MembershipApplications
            .Include(a => a.ServicePackage)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(a => a.UserId == userId, ct);

    public async Task<bool> HasPendingAsync(string userId, CancellationToken ct = default)
        => await db.MembershipApplications
            .AnyAsync(a => a.UserId == userId, ct);

    public async Task AddAsync(MembershipApplication application, CancellationToken ct = default)
        => await db.MembershipApplications.AddAsync(application, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
