using FluentAssertions;
using SkillNet.Domain.Entities;
using SkillNet.Domain.Enums;
using SkillNet.Domain.Events;

namespace SkillNet.Domain.Tests;

public class MembershipApplicationTests
{
    private static MembershipApplication CreatePending() =>
        MembershipApplication.Create(
            userId: "user-1",
            servicePackageId: Guid.NewGuid(),
            fullName: "Nguyễn Văn A",
            dateOfBirth: new DateOnly(1990, 3, 15),
            phoneNumber: "0901234567",
            address: "123 Nguyễn Huệ, Q1",
            notes: null,
            idDocumentPath: null);

    // ─── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ValidData_ReturnsPendingApplication()
    {
        var app = CreatePending();

        app.Status.Should().Be(ApplicationStatus.Pending);
        app.MembershipCode.Should().BeNull();
        app.IsDeleted.Should().BeFalse();
        app.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_RaisesApplicationCreatedEvent()
    {
        var app = CreatePending();
        app.DomainEvents.Should().ContainSingle(e => e is MembershipApplicationCreatedEvent);
    }

    // ─── Approve ─────────────────────────────────────────────────────────────

    [Fact]
    public void Approve_WhenPending_SetsStatusAndGeneratesCode()
    {
        var app = CreatePending();
        app.Approve("admin-1");

        app.Status.Should().Be(ApplicationStatus.Approved);
        app.MembershipCode.Should().StartWith("QMV-");
        app.ProcessedByAdminId.Should().Be("admin-1");
        app.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_WhenPending_RaisesApprovedEvent()
    {
        var app = CreatePending();
        app.Approve("admin-1");
        app.DomainEvents.Should().Contain(e => e is MembershipApplicationApprovedEvent);
    }

    [Theory]
    [InlineData(ApplicationStatus.Approved)]
    [InlineData(ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Cancelled)]
    public void Approve_WhenNotPending_ThrowsInvalidOperationException(ApplicationStatus initial)
    {
        var app = CreatePending();
        // bring to desired state
        if (initial == ApplicationStatus.Approved) app.Approve("admin");
        if (initial == ApplicationStatus.Rejected) app.Reject("admin", "reason");
        if (initial == ApplicationStatus.Cancelled) app.Cancel();

        var act = () => app.Approve("admin-2");
        act.Should().Throw<InvalidOperationException>()
           .WithMessage($"*{initial}*");
    }

    // ─── Reject ──────────────────────────────────────────────────────────────

    [Fact]
    public void Reject_WhenPending_SetsStatusAndReason()
    {
        var app = CreatePending();
        app.Reject("admin-1", "Thông tin không hợp lệ");

        app.Status.Should().Be(ApplicationStatus.Rejected);
        app.RejectionReason.Should().Be("Thông tin không hợp lệ");
        app.MembershipCode.Should().BeNull();
        app.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reject_WhenPending_RaisesRejectedEvent()
    {
        var app = CreatePending();
        app.Reject("admin-1", "reason");
        app.DomainEvents.Should().Contain(e => e is MembershipApplicationRejectedEvent);
    }

    [Fact]
    public void Reject_WhenAlreadyApproved_Throws()
    {
        var app = CreatePending();
        app.Approve("admin-1");

        var act = () => app.Reject("admin-1", "late change");
        act.Should().Throw<InvalidOperationException>();
    }

    // ─── Cancel ───────────────────────────────────────────────────────────────

    [Fact]
    public void Cancel_WhenPending_SetsStatusCancelled()
    {
        var app = CreatePending();
        app.Cancel();
        app.Status.Should().Be(ApplicationStatus.Cancelled);
        app.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_WhenApproved_Throws()
    {
        var app = CreatePending();
        app.Approve("admin-1");

        var act = () => app.Cancel();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_WhenRejected_Throws()
    {
        var app = CreatePending();
        app.Reject("admin-1", "reason");

        var act = () => app.Cancel();
        act.Should().Throw<InvalidOperationException>();
    }

    // ─── SoftDelete ───────────────────────────────────────────────────────────

    [Fact]
    public void SoftDelete_SetsIsDeletedTrue()
    {
        var app = CreatePending();
        app.SoftDelete();
        app.IsDeleted.Should().BeTrue();
    }

    // ─── MembershipCode uniqueness ───────────────────────────────────────────

    [Fact]
    public void Approve_TwoApplications_GenerateDifferentCodes()
    {
        var app1 = CreatePending();
        var app2 = CreatePending();
        app1.Approve("admin-1");
        app2.Approve("admin-1");

        app1.MembershipCode.Should().NotBe(app2.MembershipCode);
    }
}
