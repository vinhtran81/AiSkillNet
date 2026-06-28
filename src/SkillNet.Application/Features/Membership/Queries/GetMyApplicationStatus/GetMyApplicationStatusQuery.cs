using MediatR;
using SkillNet.Domain.Enums;

namespace SkillNet.Application.Features.Membership.Queries.GetMyApplicationStatus;

public record GetMyApplicationStatusQuery(string UserId) : IRequest<ApplicationStatusDto?>;

public record ApplicationStatusDto(
    Guid ApplicationId,
    ApplicationStatus Status,
    string? ServicePackageName,
    decimal? ServicePackagePrice,
    string? MembershipCode,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    string? RejectionReason,
    int DaysPending);
