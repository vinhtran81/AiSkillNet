using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using SkillNet.Application.Features.Membership.Jobs;
using SkillNet.Domain.Common;
using SkillNet.Domain.Enums;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Features.Membership.Commands.Approve;

public class ApproveMembershipApplicationHandler(
    IMembershipApplicationRepository appRepo,
    IBackgroundJobClient backgroundJobs,
    ILogger<ApproveMembershipApplicationHandler> logger) : IRequestHandler<ApproveMembershipApplicationCommand, Result>
{
    public async Task<Result> Handle(ApproveMembershipApplicationCommand cmd, CancellationToken ct)
    {
        var application = await appRepo.GetByIdAsync(cmd.ApplicationId, ct);
        if (application is null)
            return Result.Failure("Không tìm thấy đơn đăng ký.");

        try
        {
            application.Approve(cmd.AdminId);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Attempt to approve non-Pending application. ApplicationId={Id} AdminId={AdminId} Error={Error}",
                cmd.ApplicationId, cmd.AdminId, ex.Message);
            return Result.Failure(ex.Message);
        }

        await appRepo.SaveChangesAsync(ct);

        backgroundJobs.Enqueue<IResultNotificationJob>(
            job => job.ExecuteAsync(application.Id, ApplicationStatus.Approved,
                string.Empty, application.PhoneNumber, application.MembershipCode, null));

        logger.LogInformation(
            "Membership application approved. ApplicationId={ApplicationId} AdminId={AdminId} MembershipCode={MembershipCode}",
            cmd.ApplicationId, cmd.AdminId, application.MembershipCode);

        return Result.Success();
    }
}
