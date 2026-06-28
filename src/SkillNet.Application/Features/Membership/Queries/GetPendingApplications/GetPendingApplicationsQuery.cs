using MediatR;
using SkillNet.Domain.Enums;

namespace SkillNet.Application.Features.Membership.Queries.GetPendingApplications;

public record GetPendingApplicationsQuery(int Page = 1, int PageSize = 20) : IRequest<PendingApplicationsResult>;

public record PendingApplicationsResult(
    List<PendingApplicationDto> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public record PendingApplicationDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string ServicePackageName,
    DateTime CreatedAt,
    int DaysPending);
