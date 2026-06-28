namespace SkillNet.Domain.Events;

public record MembershipApplicationCreatedEvent(
    Guid ApplicationId,
    string UserId,
    Guid ServicePackageId) : IDomainEvent;
