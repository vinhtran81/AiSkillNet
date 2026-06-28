using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using SkillNet.Application.Features.Membership.Jobs;
using SkillNet.Application.Interfaces;
using SkillNet.Domain.Common;
using SkillNet.Domain.Entities;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Features.Membership.Commands.Submit;

public class SubmitMembershipApplicationHandler(
    IMembershipApplicationRepository appRepo,
    IServicePackageRepository packageRepo,
    IFileStorageService fileStorage,
    IBackgroundJobClient backgroundJobs,
    ILogger<SubmitMembershipApplicationHandler> logger) : IRequestHandler<SubmitMembershipApplicationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SubmitMembershipApplicationCommand cmd, CancellationToken ct)
    {
        if (await appRepo.HasPendingAsync(cmd.UserId, ct))
        {
            logger.LogWarning("Membership application blocked: user {UserId} already has a Pending application.", cmd.UserId);
            return Result.Failure<Guid>("Bạn đã có đơn đăng ký đang chờ xét duyệt.");
        }

        string? documentPath = null;
        if (cmd.IdDocumentFile is not null)
            documentPath = await fileStorage.UploadAsync(cmd.IdDocumentFile, "id-documents", ct);

        var pkg = await packageRepo.GetByIdAsync(cmd.ServicePackageId, ct);

        var application = MembershipApplication.Create(
            cmd.UserId, cmd.ServicePackageId,
            cmd.FullName, cmd.DateOfBirth,
            cmd.PhoneNumber, cmd.Address,
            cmd.Notes, documentPath);

        await appRepo.AddAsync(application, ct);
        await appRepo.SaveChangesAsync(ct);

        var packageName = pkg?.Name ?? cmd.PackageName;
        backgroundJobs.Enqueue<IConfirmationEmailJob>(
            job => job.ExecuteAsync(application.Id, cmd.UserEmail, cmd.UserName, packageName));

        logger.LogInformation(
            "Membership application submitted. UserId={UserId} PackageId={PackageId} ApplicationId={ApplicationId}",
            cmd.UserId, cmd.ServicePackageId, application.Id);

        return Result.Success(application.Id);
    }
}
