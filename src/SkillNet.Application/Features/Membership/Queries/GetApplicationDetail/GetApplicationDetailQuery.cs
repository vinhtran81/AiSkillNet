using MediatR;
using SkillNet.Domain.Enums;

namespace SkillNet.Application.Features.Membership.Queries.GetApplicationDetail;

public record GetApplicationDetailQuery(Guid ApplicationId) : IRequest<ApplicationDetailDto?>;

public record ApplicationDetailDto(
    Guid Id,
    string UserId,
    string FullName,
    DateOnly DateOfBirth,
    string PhoneNumber,
    string Address,
    string? Notes,
    string? IdDocumentPath,
    ApplicationStatus Status,
    string ServicePackageName,
    decimal ServicePackagePrice,
    string? MembershipCode,
    string? RejectionReason,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    int DaysPending);
