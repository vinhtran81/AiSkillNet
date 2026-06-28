using FluentAssertions;
using FluentValidation;
using Moq;
using SkillNet.Application.Features.Membership.Commands.Submit;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Tests;

public class SubmitMembershipApplicationValidatorTests
{
    private readonly Mock<IServicePackageRepository> _packageRepo = new();
    private readonly SubmitMembershipApplicationValidator _validator;

    public SubmitMembershipApplicationValidatorTests()
    {
        _packageRepo.Setup(r => r.IsActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);
        _validator = new SubmitMembershipApplicationValidator(_packageRepo.Object);
    }

    private static SubmitMembershipApplicationCommand BuildValid() => new(
        UserId: "user-1",
        UserEmail: "test@test.com",
        UserName: "Nguyễn Văn A",
        ServicePackageId: Guid.NewGuid(),
        FullName: "Nguyễn Văn A",
        DateOfBirth: new DateOnly(1990, 3, 15),
        PhoneNumber: "0901234567",
        Address: "123 Nguyễn Huệ, Q1, TP.HCM",
        Notes: null,
        IdDocumentFile: null,
        PackageName: "Nâng cao");

    [Fact]
    public async Task ValidCommand_PassesValidation()
    {
        var result = await _validator.ValidateAsync(BuildValid());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public async Task FullName_TooShort_FailsValidation(string name)
    {
        var result = await _validator.ValidateAsync(BuildValid() with { FullName = name });
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.FullName));
    }

    [Fact]
    public async Task FullName_Over100Chars_FailsValidation()
    {
        var longName = new string('A', 101);
        var result = await _validator.ValidateAsync(BuildValid() with { FullName = longName });
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.FullName));
    }

    [Theory]
    [InlineData("1234567890")]   // không bắt đầu bằng 0
    [InlineData("090123456")]    // 9 số
    [InlineData("09012345678")]  // 11 số
    [InlineData("0901234abc")]   // có chữ
    public async Task PhoneNumber_InvalidFormat_FailsValidation(string phone)
    {
        var result = await _validator.ValidateAsync(BuildValid() with { PhoneNumber = phone });
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.PhoneNumber));
    }

    [Fact]
    public async Task PhoneNumber_ValidFormat_Passes()
    {
        var result = await _validator.ValidateAsync(BuildValid() with { PhoneNumber = "0912345678" });
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.PhoneNumber));
    }

    [Fact]
    public async Task DateOfBirth_Under16_FailsValidation()
    {
        var dob = DateOnly.FromDateTime(DateTime.Today.AddYears(-15));
        var result = await _validator.ValidateAsync(BuildValid() with { DateOfBirth = dob });
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.DateOfBirth));
    }

    [Fact]
    public async Task DateOfBirth_Exactly16_PassesValidation()
    {
        var dob = DateOnly.FromDateTime(DateTime.Today.AddYears(-16));
        var result = await _validator.ValidateAsync(BuildValid() with { DateOfBirth = dob });
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.DateOfBirth));
    }

    [Fact]
    public async Task Address_Empty_FailsValidation()
    {
        var result = await _validator.ValidateAsync(BuildValid() with { Address = "" });
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.Address));
    }

    [Fact]
    public async Task ServicePackageId_Inactive_FailsValidation()
    {
        _packageRepo.Setup(r => r.IsActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);
        var result = await _validator.ValidateAsync(BuildValid());
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.ServicePackageId));
    }

    [Fact]
    public async Task Notes_Over500Chars_FailsValidation()
    {
        var longNote = new string('x', 501);
        var result = await _validator.ValidateAsync(BuildValid() with { Notes = longNote });
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitMembershipApplicationCommand.Notes));
    }
}
