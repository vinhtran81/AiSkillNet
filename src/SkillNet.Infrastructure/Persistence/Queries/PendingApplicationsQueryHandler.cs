using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillNet.Application.Features.Membership.Queries.GetPendingApplications;
using SkillNet.Domain.Enums;

namespace SkillNet.Infrastructure.Persistence.Queries;

public class PendingApplicationsQueryHandler(SkillNetDbContext db)
    : IRequestHandler<GetPendingApplicationsQuery, PendingApplicationsResult>
{
    public async Task<PendingApplicationsResult> Handle(GetPendingApplicationsQuery request, CancellationToken ct)
    {
        var query = db.MembershipApplications
            .AsNoTracking()
            .Include(a => a.ServicePackage)
            .Where(a => a.Status == ApplicationStatus.Pending)
            .OrderBy(a => a.CreatedAt);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new PendingApplicationDto(
                a.Id,
                a.FullName,
                a.PhoneNumber,
                a.ServicePackage!.Name,
                a.CreatedAt,
                (int)(DateTime.UtcNow - a.CreatedAt).TotalDays))
            .ToListAsync(ct);

        return new PendingApplicationsResult(items, total, request.Page, request.PageSize);
    }
}
