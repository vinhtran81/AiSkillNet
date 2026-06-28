namespace SkillNet.Application.Features.Membership.Jobs;

public interface IConfirmationEmailJob
{
    Task ExecuteAsync(Guid applicationId, string userEmail, string userName, string packageName);
}
