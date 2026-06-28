using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using SkillNet.Application.Features.Membership.Jobs;
using SkillNet.Domain.Common;
using SkillNet.Domain.Enums;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Features.Membership.Commands.Reject;

public class RejectMembershipApplicationHandler(
    IMembershipApplicationRepository appRepo,
    IBackgroundJobClient backgroundJobs,
    ILogger<RejectMembershipApplicationHandler> logger) : IRequestHandler<RejectMembershipApplicationCommand, Result>
{
    public async Task<Result> Handle(RejectMembershipApplicationCommand cmd, CancellationToken ct)
    {
        var application = await appRepo.GetByIdAsync(cmd.ApplicationId, ct);
        if (application is null)
            return Result.Failure("Không tìm thấy đơn đăng ký.");

        try
        {
            application.Reject(cmd.AdminId, cmd.RejectionReason);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await appRepo.SaveChangesAsync(ct);

        backgroundJobs.Enqueue<IResultNotificationJob>(
            job => job.ExecuteAsync(application.Id, ApplicationStatus.Rejected,
                string.Empty, application.PhoneNumber, null, cmd.RejectionReason));

        logger.LogInformation(
            "Membership application rejected. ApplicationId={ApplicationId} AdminId={AdminId} ReasonLength={Length}",
            cmd.ApplicationId, cmd.AdminId, cmd.RejectionReason.Length);

        return Result.Success();
    }
}
