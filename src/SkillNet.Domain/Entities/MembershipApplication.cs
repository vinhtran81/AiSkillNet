using SkillNet.Domain.Enums;
using SkillNet.Domain.Events;

namespace SkillNet.Domain.Entities;

public class MembershipApplication
{
    private MembershipApplication() { }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public Guid ServicePackageId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public string? IdDocumentPath { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? MembershipCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ProcessedByAdminId { get; private set; }
    public bool IsDeleted { get; private set; }

    public ServicePackage? ServicePackage { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static MembershipApplication Create(
        string userId,
        Guid servicePackageId,
        string fullName,
        DateOnly dateOfBirth,
        string phoneNumber,
        string address,
        string? notes,
        string? idDocumentPath)
    {
        var app = new MembershipApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ServicePackageId = servicePackageId,
            FullName = fullName,
            DateOfBirth = dateOfBirth,
            PhoneNumber = phoneNumber,
            Address = address,
            Notes = notes,
            IdDocumentPath = idDocumentPath,
            Status = ApplicationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        app._domainEvents.Add(new MembershipApplicationCreatedEvent(app.Id, userId, servicePackageId));
        return app;
    }

    public void Approve(string adminId)
    {
        if (Status != ApplicationStatus.Pending)
            throw new InvalidOperationException($"Cannot approve application with status {Status}. Only Pending applications can be approved.");

        Status = ApplicationStatus.Approved;
        MembershipCode = GenerateMembershipCode();
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
        _domainEvents.Add(new MembershipApplicationApprovedEvent(Id, UserId, MembershipCode));
    }

    public void Reject(string adminId, string reason)
    {
        if (Status != ApplicationStatus.Pending)
            throw new InvalidOperationException($"Cannot reject application with status {Status}. Only Pending applications can be rejected.");

        Status = ApplicationStatus.Rejected;
        RejectionReason = reason;
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
        _domainEvents.Add(new MembershipApplicationRejectedEvent(Id, UserId, reason));
    }

    public void Cancel()
    {
        if (Status != ApplicationStatus.Pending)
            throw new InvalidOperationException($"Cannot cancel application with status {Status}. Only Pending applications can be cancelled.");

        Status = ApplicationStatus.Cancelled;
        ProcessedAt = DateTime.UtcNow;
    }

    public void SoftDelete() => IsDeleted = true;

    public void ClearDomainEvents() => _domainEvents.Clear();

    private static string GenerateMembershipCode()
        => $"QMV-{DateTime.UtcNow:yyyyMM}-{Random.Shared.Next(1000, 9999)}";
}
