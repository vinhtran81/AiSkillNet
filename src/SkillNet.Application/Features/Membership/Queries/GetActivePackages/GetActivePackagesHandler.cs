using MediatR;
using Microsoft.Extensions.Caching.Memory;
using SkillNet.Domain.Interfaces;
using System.Text.Json;

namespace SkillNet.Application.Features.Membership.Queries.GetActivePackages;

public class GetActivePackagesHandler(
    IServicePackageRepository packageRepo,
    IMemoryCache cache) : IRequestHandler<GetActivePackagesQuery, List<ServicePackageDto>>
{
    private const string CacheKey = "active-service-packages";

    public async Task<List<ServicePackageDto>> Handle(GetActivePackagesQuery request, CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out List<ServicePackageDto>? cached) && cached is not null)
            return cached;

        var packages = await packageRepo.GetActiveAsync(ct);
        var dtos = packages.Select((p, i) => new ServicePackageDto(
            p.Id, p.Name, p.Description, p.Price, p.DurationMonths,
            DeserializeBenefits(p.Benefits),
            IsPopular: p.DisplayOrder == 2)).ToList();

        cache.Set(CacheKey, dtos, TimeSpan.FromMinutes(5));
        return dtos;
    }

    private static List<string> DeserializeBenefits(string json)
    {
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? []; }
        catch { return []; }
    }
}
