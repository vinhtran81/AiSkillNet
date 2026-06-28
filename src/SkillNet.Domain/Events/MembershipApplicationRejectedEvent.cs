namespace SkillNet.Domain.Events;

public record MembershipApplicationRejectedEvent(
    Guid ApplicationId,
    string UserId,
    string RejectionReason) : IDomainEvent;
