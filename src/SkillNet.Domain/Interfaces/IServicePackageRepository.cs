using SkillNet.Domain.Entities;

namespace SkillNet.Domain.Interfaces;

public interface IServicePackageRepository
{
    Task<List<ServicePackage>> GetActiveAsync(CancellationToken ct = default);
    Task<ServicePackage?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> IsActiveAsync(Guid id, CancellationToken ct = default);
}
