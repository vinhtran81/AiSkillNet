using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillNet.Application.Features.Membership.Queries.GetPendingCount;
using SkillNet.Domain.Enums;

namespace SkillNet.Infrastructure.Persistence.Queries;

public class PendingCountQueryHandler(SkillNetDbContext db)
    : IRequestHandler<GetPendingCountQuery, int>
{
    public async Task<int> Handle(GetPendingCountQuery request, CancellationToken ct)
        => await db.MembershipApplications
            .CountAsync(a => a.Status == ApplicationStatus.Pending, ct);
}
