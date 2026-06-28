using SkillNet.Domain.Enums;

namespace SkillNet.Application.Features.Membership.Jobs;

public interface IResultNotificationJob
{
    Task ExecuteAsync(Guid applicationId, ApplicationStatus status, string userEmail, string? phoneNumber, string? membershipCode, string? rejectionReason);
}
