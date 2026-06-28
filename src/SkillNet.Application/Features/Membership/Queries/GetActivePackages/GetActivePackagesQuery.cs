using MediatR;

namespace SkillNet.Application.Features.Membership.Queries.GetActivePackages;

public record GetActivePackagesQuery : IRequest<List<ServicePackageDto>>;

public record ServicePackageDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int DurationMonths,
    List<string> Benefits,
    bool IsPopular);
