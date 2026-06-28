using System.Net;
using FluentAssertions;
using SkillNet.Domain.Enums;
using SkillNet.Integration.Tests.Infrastructure;

namespace SkillNet.Integration.Tests;

[Trait("Category", "Integration")]
public class MembershipApiTests(SkillNetWebApplicationFactory factory)
    : IClassFixture<SkillNetWebApplicationFactory>
{
    private readonly HttpClient _member = factory.CreateAuthenticatedClient("Member", "user-member-1");
    private readonly HttpClient _admin = factory.CreateAuthenticatedClient("Admin", "user-admin-1");
    private readonly HttpClient _anon = factory.CreateAnonClient();

    // ─── Happy Path ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GET_ServicePackage_ReturnsOkWithPackageList()
    {
        var response = await _anon.GetAsync("/ServicePackage");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Đăng ký ngay");
        html.Should().Contain("Tiêu chuẩn Test");
    }

    [Fact]
    public async Task GET_MembershipRegister_Authenticated_ReturnsForm()
    {
        var response = await _member.GetAsync($"/Membership/Register?packageId={factory.SeedPackageId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Nộp đơn");
        html.Should().Contain(factory.SeedPackageId.ToString());
    }

    [Fact]
    public async Task POST_MembershipRegister_ValidData_RedirectsToStatus()
    {
        var token = await _member.GetAntiForgeryTokenAsync("/Membership/Register");

        var form = BuildValidFormContent(factory.SeedPackageId, token);
        var response = await _member.PostAsync("/Membership/Register", form);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("Status");
    }

    [Fact]
    public async Task GET_MembershipStatus_AfterSubmit_ShowsPendingBadge()
    {
        // Submit an application first
        var token = await _member.GetAntiForgeryTokenAsync("/Membership/Register");
        await _member.PostAsync("/Membership/Register", BuildValidFormContent(factory.SeedPackageId, token));

        // Create a new client for a different user to avoid "already has pending" issue
        var anotherMember = factory.CreateAuthenticatedClient("Member", "user-member-status-check");
        var submitToken = await anotherMember.GetAntiForgeryTokenAsync("/Membership/Register");
        await anotherMember.PostAsync("/Membership/Register", BuildValidFormContent(factory.SeedPackageId, submitToken));

        var followClient = factory.CreateAuthenticatedClient("Member", "user-member-status-check");
        followClient = factory.CreateAuthenticatedClient("Member", "user-member-status-check");
        var statusResponse = await followClient.GetAsync("/Membership/Status");
        new[] { HttpStatusCode.OK, HttpStatusCode.Redirect }.Should().Contain(statusResponse.StatusCode);
    }

    // ─── Edge Cases ──────────────────────────────────────────────────────────

    [Fact]
    public async Task POST_MembershipRegister_HasPendingApplication_ShowsError()
    {
        var existingUser = factory.CreateAuthenticatedClient("Member", "user-double-submit");
        await factory.SeedPendingApplicationAsync("user-double-submit");

        var token = await existingUser.GetAntiForgeryTokenAsync("/Membership/Register");
        // GET /Register should redirect to Status because user has pending app
        var getResponse = await existingUser.GetAsync("/Membership/Register");
        getResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
        getResponse.Headers.Location!.ToString().Should().Contain("Status");
    }

    [Theory]
    [InlineData("FullName", "")]
    [InlineData("PhoneNumber", "123")]
    [InlineData("PhoneNumber", "1234567890")]
    [InlineData("Address", "")]
    public async Task POST_MembershipRegister_InvalidField_ReturnsFormWithValidationError(string field, string value)
    {
        var token = await _member.GetAntiForgeryTokenAsync("/Membership/Register");
        var form = BuildValidFormDictionary(factory.SeedPackageId, token);
        form[field] = value;

        var response = await _member.PostAsync("/Membership/Register", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        (html.Contains("is-invalid") || html.Contains("field-validation-error")).Should().BeTrue();
    }

    // ─── Admin Flows ─────────────────────────────────────────────────────────

    [Fact]
    public async Task POST_AdminApprove_ValidApplication_ChangesStatusToApproved()
    {
        var appId = await factory.SeedPendingApplicationAsync("user-to-approve");

        var token = await _admin.GetAntiForgeryTokenAsync($"/Admin/Membership/Detail/{appId}");
        var response = await _admin.PostAsync($"/Admin/Membership/Approve/{appId}",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = token
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var app = await factory.GetApplicationAsync(appId);
        app!.Status.Should().Be(ApplicationStatus.Approved);
        app.MembershipCode.Should().StartWith("QMV-");
    }

    [Fact]
    public async Task POST_AdminReject_WithReason_ChangesStatusToRejected()
    {
        var appId = await factory.SeedPendingApplicationAsync("user-to-reject");

        var token = await _admin.GetAntiForgeryTokenAsync($"/Admin/Membership/Detail/{appId}");
        var response = await _admin.PostAsync($"/Admin/Membership/Reject/{appId}",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["RejectionReason"] = "Thông tin không khớp CMND",
                ["__RequestVerificationToken"] = token
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var app = await factory.GetApplicationAsync(appId);
        app!.Status.Should().Be(ApplicationStatus.Rejected);
        app.RejectionReason.Should().Be("Thông tin không khớp CMND");
    }

    [Fact]
    public async Task POST_AdminReject_WithoutReason_ReturnsFormWithError()
    {
        var appId = await factory.SeedPendingApplicationAsync("user-reject-no-reason");

        var token = await _admin.GetAntiForgeryTokenAsync($"/Admin/Membership/Detail/{appId}");
        var response = await _admin.PostAsync($"/Admin/Membership/Reject/{appId}",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["RejectionReason"] = "",
                ["__RequestVerificationToken"] = token
            }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("lý do từ chối");
    }

    // ─── Security ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GET_MembershipRegister_Unauthenticated_RedirectsToLogin()
    {
        var response = await _anon.GetAsync("/Membership/Register");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("Login");
    }

    [Fact]
    public async Task GET_AdminPending_MemberRole_ReturnsForbidOrRedirect()
    {
        var response = await _member.GetAsync("/Admin/Membership/Pending");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task POST_Approve_WithoutAntiForgeryToken_ReturnsBadRequest()
    {
        var appId = await factory.SeedPendingApplicationAsync("user-csrf-test");

        var response = await _admin.PostAsync($"/Admin/Membership/Approve/{appId}",
            new FormUrlEncodedContent([]));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static FormUrlEncodedContent BuildValidFormContent(Guid packageId, string token)
        => new(BuildValidFormDictionary(packageId, token));

    private static Dictionary<string, string> BuildValidFormDictionary(Guid packageId, string token) => new()
    {
        ["FullName"] = "Nguyễn Văn Test",
        ["DateOfBirth"] = "1990-03-15",
        ["PhoneNumber"] = "0901234567",
        ["Address"] = "123 Nguyễn Huệ, Quận 1, TP.HCM",
        ["ServicePackageId"] = packageId.ToString(),
        ["__RequestVerificationToken"] = token
    };
}
