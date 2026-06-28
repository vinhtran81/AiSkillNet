using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SkillNet.Application.Features.Membership.Commands.Submit;
using SkillNet.Application.Interfaces;
using SkillNet.Domain.Entities;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Tests;

public class SubmitMembershipApplicationHandlerTests
{
    private readonly Mock<IMembershipApplicationRepository> _appRepo = new();
    private readonly Mock<IServicePackageRepository> _packageRepo = new();
    private readonly Mock<IFileStorageService> _fileStorage = new();
    private readonly Mock<IBackgroundJobClient> _jobs = new();

    private SubmitMembershipApplicationHandler BuildHandler() => new(
        _appRepo.Object,
        _packageRepo.Object,
        _fileStorage.Object,
        _jobs.Object,
        NullLogger<SubmitMembershipApplicationHandler>.Instance);

    private static SubmitMembershipApplicationCommand ValidCommand() => new(
        UserId: "user-1",
        UserEmail: "user@test.com",
        UserName: "Test User",
        ServicePackageId: Guid.NewGuid(),
        FullName: "Test User",
        DateOfBirth: new DateOnly(1990, 1, 1),
        PhoneNumber: "0901234567",
        Address: "123 Test Street",
        Notes: null,
        IdDocumentFile: null,
        PackageName: "Standard");

    [Fact]
    public async Task Handle_NoPendingApplication_CreatesApplicationAndEnqueuesJob()
    {
        _appRepo.Setup(r => r.HasPendingAsync("user-1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
        _packageRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((ServicePackage?)null);

        var result = await BuildHandler().Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _appRepo.Verify(r => r.AddAsync(It.IsAny<MembershipApplication>(), It.IsAny<CancellationToken>()), Times.Once);
        _appRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        // Enqueue is an extension over IBackgroundJobClient.Create() — verify the underlying call
        _jobs.Verify(j => j.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HasPendingApplication_ReturnsFailure_DoesNotCreate()
    {
        _appRepo.Setup(r => r.HasPendingAsync("user-1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

        var result = await BuildHandler().Handle(ValidCommand(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("đang chờ xét duyệt");
        _appRepo.Verify(r => r.AddAsync(It.IsAny<MembershipApplication>(), It.IsAny<CancellationToken>()), Times.Never);
        _jobs.Verify(j => j.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Never);
    }
}
