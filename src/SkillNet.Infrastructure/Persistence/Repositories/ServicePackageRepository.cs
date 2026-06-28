using Microsoft.EntityFrameworkCore;
using SkillNet.Domain.Entities;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Infrastructure.Persistence.Repositories;

public class ServicePackageRepository(SkillNetDbContext db) : IServicePackageRepository
{
    public async Task<List<ServicePackage>> GetActiveAsync(CancellationToken ct = default)
        => await db.ServicePackages
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(ct);

    public async Task<ServicePackage?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.ServicePackages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<bool> IsActiveAsync(Guid id, CancellationToken ct = default)
        => await db.ServicePackages.AnyAsync(p => p.Id == id && p.IsActive, ct);
}
