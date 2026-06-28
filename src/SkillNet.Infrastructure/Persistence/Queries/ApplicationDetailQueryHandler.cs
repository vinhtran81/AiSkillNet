using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillNet.Application.Features.Membership.Queries.GetApplicationDetail;

namespace SkillNet.Infrastructure.Persistence.Queries;

public class ApplicationDetailQueryHandler(SkillNetDbContext db)
    : IRequestHandler<GetApplicationDetailQuery, ApplicationDetailDto?>
{
    public async Task<ApplicationDetailDto?> Handle(GetApplicationDetailQuery request, CancellationToken ct)
    {
        var a = await db.MembershipApplications
            .AsNoTracking()
            .Include(x => x.ServicePackage)
            .FirstOrDefaultAsync(x => x.Id == request.ApplicationId, ct);

        if (a is null) return null;

        return new ApplicationDetailDto(
            a.Id, a.UserId, a.FullName, a.DateOfBirth, a.PhoneNumber, a.Address,
            a.Notes, a.IdDocumentPath, a.Status,
            a.ServicePackage?.Name ?? string.Empty,
            a.ServicePackage?.Price ?? 0,
            a.MembershipCode, a.RejectionReason,
            a.CreatedAt, a.ProcessedAt,
            (int)(DateTime.UtcNow - a.CreatedAt).TotalDays);
    }
}
