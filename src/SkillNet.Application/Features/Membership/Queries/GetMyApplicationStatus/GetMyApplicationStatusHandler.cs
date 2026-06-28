using MediatR;
using SkillNet.Domain.Enums;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Features.Membership.Queries.GetMyApplicationStatus;

public class GetMyApplicationStatusHandler(
    IMembershipApplicationRepository appRepo) : IRequestHandler<GetMyApplicationStatusQuery, ApplicationStatusDto?>
{
    public async Task<ApplicationStatusDto?> Handle(GetMyApplicationStatusQuery request, CancellationToken ct)
    {
        var app = await appRepo.GetByUserIdAsync(request.UserId, ct);
        if (app is null) return null;

        return new ApplicationStatusDto(
            app.Id,
            app.Status,
            app.ServicePackage?.Name,
            app.ServicePackage?.Price,
            app.MembershipCode,
            app.CreatedAt,
            app.ProcessedAt,
            app.RejectionReason,
            (int)(DateTime.UtcNow - app.CreatedAt).TotalDays);
    }
}
