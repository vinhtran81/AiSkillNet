using MediatR;
using Microsoft.Extensions.Logging;
using SkillNet.Domain.Common;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Features.Membership.Commands.Cancel;

public class CancelMembershipApplicationHandler(
    IMembershipApplicationRepository appRepo,
    ILogger<CancelMembershipApplicationHandler> logger) : IRequestHandler<CancelMembershipApplicationCommand, Result>
{
    public async Task<Result> Handle(CancelMembershipApplicationCommand cmd, CancellationToken ct)
    {
        var application = await appRepo.GetByIdAsync(cmd.ApplicationId, ct);
        if (application is null || application.UserId != cmd.UserId)
            return Result.Failure("Không tìm thấy đơn đăng ký.");

        try
        {
            application.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await appRepo.SaveChangesAsync(ct);

        logger.LogInformation(
            "Membership application cancelled by user. ApplicationId={ApplicationId} UserId={UserId}",
            cmd.ApplicationId, cmd.UserId);

        return Result.Success();
    }
}
