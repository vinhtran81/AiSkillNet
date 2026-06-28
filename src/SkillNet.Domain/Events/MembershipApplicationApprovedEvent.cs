namespace SkillNet.Domain.Events;

public record MembershipApplicationApprovedEvent(
    Guid ApplicationId,
    string UserId,
    string MembershipCode) : IDomainEvent;
