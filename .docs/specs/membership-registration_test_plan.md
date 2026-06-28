---
type: Test Plan
feature_id: FEAT-001
status: DRAFT
version: 1.0
created_at: 2026-06-27
depends_on: membership-registration_api_spec.md
---

# Test Plan — FEAT-001 Đăng ký Hội viên mới

---

## 1. Phạm vi & Chiến lược

| Tier | Loại test | Tool | Ngưỡng CI block |
|------|-----------|------|----------------|
| **Unit** | Domain logic, Validators, Handler logic (mock infra) | xUnit, Moq, FluentAssertions | Bất kỳ fail |
| **Integration** | HTTP endpoints end-to-end với real DB (test container) | xUnit + WebApplicationFactory + SQL Server | Bất kỳ fail |
| **Security** | CSRF, Auth, Authorization | xUnit integration | Bất kỳ fail |
| **Performance** | Load test critical path | k6 | p95 > 500ms |
| **Manual UAT** | Happy path trên Staging | QC / PO | Sign-off |

---

## 2. Unit Tests — Domain & Application

### 2.1 MembershipApplication Entity

```csharp
// tests/SkillNet.Domain.Tests/MembershipApplicationTests.cs
public class MembershipApplicationTests
{
    // --- Create ---
    [Fact]
    public void Create_ValidData_ReturnsApplicationWithPendingStatus()
    {
        var app = MembershipApplication.Create(
            userId: "user-1", servicePackageId: Guid.NewGuid(),
            fullName: "Nguyễn Văn A", dateOfBirth: new DateOnly(1990, 3, 15),
            phoneNumber: "0901234567", address: "123 Nguyễn Huệ",
            notes: null, idDocumentPath: null);

        app.Status.Should().Be(ApplicationStatus.Pending);
        app.MembershipCode.Should().BeNull();
        app.IsDeleted.Should().BeFalse();
        app.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
    }

    // --- Approve ---
    [Fact]
    public void Approve_WhenPending_SetsStatusAndGeneratesCode()
    {
        var app = CreatePendingApplication();
        app.Approve(adminId: "admin-1");

        app.Status.Should().Be(ApplicationStatus.Approved);
        app.MembershipCode.Should().StartWith("QMV-");
        app.ProcessedAt.Should().NotBeNull();
        app.ProcessedByAdminId.Should().Be("admin-1");
    }

    [Fact]
    public void Approve_WhenAlreadyApproved_ThrowsInvalidOperationException()
    {
        var app = CreatePendingApplication();
        app.Approve("admin-1");

        var act = () => app.Approve("admin-1");
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Pending*");
    }

    [Theory]
    [InlineData(ApplicationStatus.Approved)]
    [InlineData(ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Cancelled)]
    public void Approve_WhenNotPending_Throws(ApplicationStatus initial)
    {
        var app = CreateApplicationWithStatus(initial);
        var act = () => app.Approve("admin-1");
        act.Should().Throw<InvalidOperationException>();
    }

    // --- Reject ---
    [Fact]
    public void Reject_WhenPending_SetsStatusAndReason()
    {
        var app = CreatePendingApplication();
        app.Reject(adminId: "admin-1", reason: "Thông tin không hợp lệ");

        app.Status.Should().Be(ApplicationStatus.Rejected);
        app.RejectionReason.Should().Be("Thông tin không hợp lệ");
        app.MembershipCode.Should().BeNull();
    }

    // --- Cancel ---
    [Fact]
    public void Cancel_WhenPending_SetsStatusCancelled()
    {
        var app = CreatePendingApplication();
        app.Cancel();
        app.Status.Should().Be(ApplicationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenApproved_Throws()
    {
        var app = CreatePendingApplication();
        app.Approve("admin-1");
        var act = () => app.Cancel();
        act.Should().Throw<InvalidOperationException>();
    }
}
```

### 2.2 FluentValidation — SubmitMembershipApplicationValidator

```csharp
// tests/SkillNet.Application.Tests/Validators/SubmitMembershipApplicationValidatorTests.cs
public class SubmitMembershipApplicationValidatorTests
{
    private readonly Mock<IServicePackageRepository> _packageRepoMock = new();
    private readonly SubmitMembershipApplicationValidator _validator;

    public SubmitMembershipApplicationValidatorTests()
    {
        _packageRepoMock.Setup(r => r.IsActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);
        _validator = new SubmitMembershipApplicationValidator(_packageRepoMock.Object);
    }

    [Fact]
    public async Task ValidCommand_PassesValidation()
    {
        var cmd = BuildValidCommand();
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]       // too short
    [InlineData(null)]
    public async Task FullName_Invalid_FailsValidation(string? name)
    {
        var cmd = BuildValidCommand() with { FullName = name! };
        var result = await _validator.ValidateAsync(cmd);
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.FullName));
    }

    [Theory]
    [InlineData("1234567890")]  // không bắt đầu bằng 0
    [InlineData("090123456")]   // 9 số
    [InlineData("09012345678")] // 11 số
    [InlineData("0901234abc")]  // có chữ
    public async Task PhoneNumber_InvalidFormat_FailsValidation(string phone)
    {
        var cmd = BuildValidCommand() with { PhoneNumber = phone };
        var result = await _validator.ValidateAsync(cmd);
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.PhoneNumber));
    }

    [Fact]
    public async Task DateOfBirth_Under16_FailsValidation()
    {
        var cmd = BuildValidCommand() with
        {
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-15))
        };
        var result = await _validator.ValidateAsync(cmd);
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.DateOfBirth));
    }

    [Fact]
    public async Task ServicePackageId_Inactive_FailsValidation()
    {
        _packageRepoMock.Setup(r => r.IsActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);
        var cmd = BuildValidCommand();
        var result = await _validator.ValidateAsync(cmd);
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.ServicePackageId));
    }

    private static SubmitMembershipApplicationCommand BuildValidCommand() => new(
        UserId: "user-1", UserEmail: "test@test.com", UserName: "Nguyễn Văn A",
        ServicePackageId: Guid.NewGuid(),
        FullName: "Nguyễn Văn A",
        DateOfBirth: new DateOnly(1990, 3, 15),
        PhoneNumber: "0901234567",
        Address: "123 Nguyễn Huệ, Q1",
        Notes: null, IdDocumentFile: null, PackageName: "Nâng cao");
}
```

### 2.3 SubmitMembershipApplicationHandler

```csharp
// tests/SkillNet.Application.Tests/Handlers/SubmitMembershipApplicationHandlerTests.cs
public class SubmitMembershipApplicationHandlerTests
{
    private readonly Mock<IMembershipApplicationRepository> _appRepoMock = new();
    private readonly Mock<IBackgroundJobClient> _jobsMock = new();
    private readonly Mock<IFileStorageService> _fileMock = new();

    [Fact]
    public async Task Handle_NoPendingApplication_CreatesAndEnqueuesJob()
    {
        _appRepoMock.Setup(r => r.HasPendingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);
        _appRepoMock.Setup(r => r.AddAsync(It.IsAny<MembershipApplication>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var handler = new SubmitMembershipApplicationHandler(
            _appRepoMock.Object, Mock.Of<IServicePackageRepository>(),
            _fileMock.Object, _jobsMock.Object);

        var result = await handler.Handle(BuildValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _appRepoMock.Verify(r => r.AddAsync(It.IsAny<MembershipApplication>(), It.IsAny<CancellationToken>()), Times.Once);
        _jobsMock.Verify(j => j.Enqueue(It.IsAny<Expression<Action<SendApplicationConfirmationEmailJob>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HasPendingApplication_ReturnsFailure()
    {
        _appRepoMock.Setup(r => r.HasPendingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

        var handler = new SubmitMembershipApplicationHandler(
            _appRepoMock.Object, Mock.Of<IServicePackageRepository>(),
            _fileMock.Object, _jobsMock.Object);

        var result = await handler.Handle(BuildValidCommand(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Pending");
        _appRepoMock.Verify(r => r.AddAsync(It.IsAny<MembershipApplication>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
```

---

## 3. Integration Tests — WebApplicationFactory

```csharp
// tests/SkillNet.Integration.Tests/MembershipRegistrationTests.cs
[Trait("Category", "Integration")]
public class MembershipRegistrationTests(SkillNetWebApplicationFactory factory)
    : IClassFixture<SkillNetWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateAuthenticatedClient(role: "Member");
    private readonly HttpClient _adminClient = factory.CreateAuthenticatedClient(role: "Admin");
    private readonly HttpClient _anonClient = factory.CreateClient();

    // ─── Happy Path ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GET_ServicePackage_ReturnsPackageList()
    {
        var response = await _anonClient.GetAsync("/ServicePackage");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Đăng ký ngay");
    }

    [Fact]
    public async Task GET_Register_WithValidPackageId_PreFillsPackage()
    {
        var packageId = factory.SeedPackageId;
        var response = await _client.GetAsync($"/Membership/Register?packageId={packageId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain(packageId.ToString());
    }

    [Fact]
    public async Task POST_Register_ValidData_RedirectsToStatus()
    {
        var token = await _client.GetAntiForgeryTokenAsync("/Membership/Register");
        var form = new Dictionary<string, string>
        {
            ["FullName"]         = "Nguyễn Văn Test",
            ["DateOfBirth"]      = "1990-03-15",
            ["PhoneNumber"]      = "0901234567",
            ["Address"]          = "123 Nguyễn Huệ, Quận 1, TP.HCM",
            ["ServicePackageId"] = factory.SeedPackageId.ToString(),
            ["__RequestVerificationToken"] = token
        };
        var response = await _client.PostAsync("/Membership/Register",
            new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/Membership/Status");
    }

    // ─── Edge Cases ───────────────────────────────────────────────────────────

    [Fact]
    public async Task POST_Register_WhenHasPendingApplication_ShowsError()
    {
        // Seed: user đã có đơn Pending
        await factory.SeedPendingApplicationAsync(_client.UserId());

        var token = await _client.GetAntiForgeryTokenAsync("/Membership/Register");
        var response = await _client.PostAsync("/Membership/Register",
            BuildValidFormContent(factory.SeedPackageId, token));

        // Không tạo thêm đơn mới
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("đang chờ xét duyệt");
    }

    [Fact]
    public async Task GET_Register_WhenHasPendingApplication_RedirectsToStatus()
    {
        await factory.SeedPendingApplicationAsync(_client.UserId());
        var response = await _client.GetAsync("/Membership/Register");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("Status");
    }

    [Theory]
    [InlineData("FullName", "")]
    [InlineData("PhoneNumber", "123")]          // wrong format
    [InlineData("PhoneNumber", "1234567890")]   // not starting with 0
    [InlineData("Address", "")]
    public async Task POST_Register_InvalidField_ReturnsFormWithError(string field, string value)
    {
        var token = await _client.GetAntiForgeryTokenAsync("/Membership/Register");
        var form = BuildValidFormDictionary(factory.SeedPackageId, token);
        form[field] = value;

        var response = await _client.PostAsync("/Membership/Register",
            new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("form-control is-invalid");
    }

    // ─── Admin Flows ──────────────────────────────────────────────────────────

    [Fact]
    public async Task POST_Approve_ValidApplication_RedirectsWithSuccess()
    {
        var appId = await factory.SeedPendingApplicationAsync("other-user");
        var token = await _adminClient.GetAntiForgeryTokenAsync($"/Admin/Membership/Detail/{appId}");

        var response = await _adminClient.PostAsync($"/Admin/Membership/Approve/{appId}",
            new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("__RequestVerificationToken", token)
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        // Verify DB status changed
        var app = await factory.GetApplicationAsync(appId);
        app!.Status.Should().Be(ApplicationStatus.Approved);
        app.MembershipCode.Should().StartWith("QMV-");
    }

    [Fact]
    public async Task POST_Reject_WithReason_UpdatesStatusAndReason()
    {
        var appId = await factory.SeedPendingApplicationAsync("other-user");
        var token = await _adminClient.GetAntiForgeryTokenAsync($"/Admin/Membership/Detail/{appId}");

        var response = await _adminClient.PostAsync($"/Admin/Membership/Reject/{appId}",
            new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("RejectionReason", "Thông tin không khớp"),
                new KeyValuePair<string,string>("__RequestVerificationToken", token)
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        var app = await factory.GetApplicationAsync(appId);
        app!.Status.Should().Be(ApplicationStatus.Rejected);
        app.RejectionReason.Should().Be("Thông tin không khớp");
    }

    [Fact]
    public async Task POST_Reject_WithoutReason_ReturnsBadRequest()
    {
        var appId = await factory.SeedPendingApplicationAsync("other-user");
        var token = await _adminClient.GetAntiForgeryTokenAsync($"/Admin/Membership/Detail/{appId}");

        var response = await _adminClient.PostAsync($"/Admin/Membership/Reject/{appId}",
            new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("RejectionReason", ""),
                new KeyValuePair<string,string>("__RequestVerificationToken", token)
            }));

        // Form re-renders với validation error
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Vui lòng nhập lý do từ chối");
    }

    // ─── Security Tests ───────────────────────────────────────────────────────

    [Fact]
    public async Task GET_Register_Unauthenticated_RedirectsToLogin()
    {
        var response = await _anonClient.GetAsync("/Membership/Register");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/Account/Login");
    }

    [Fact]
    public async Task GET_AdminPending_MemberRole_ReturnsForbidden()
    {
        var response = await _client.GetAsync("/Admin/Membership/Pending");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task POST_Approve_WithoutAntiForgeryToken_ReturnsBadRequest()
    {
        var appId = await factory.SeedPendingApplicationAsync("other-user");
        // Không có CSRF token
        var response = await _adminClient.PostAsync($"/Admin/Membership/Approve/{appId}",
            new FormUrlEncodedContent([]));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Cancel_AnotherUserApplication_ReturnsForbidden()
    {
        var appId = await factory.SeedPendingApplicationAsync("other-user");
        var token = await _client.GetAntiForgeryTokenAsync("/Membership/Status");

        var response = await _client.PostAsync($"/Membership/Cancel/{appId}",
            new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("__RequestVerificationToken", token)
            }));

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.NotFound);
    }
}
```

---

## 4. WebApplicationFactory — Test Infrastructure

```csharp
// tests/SkillNet.Integration.Tests/Infrastructure/SkillNetWebApplicationFactory.cs
public class SkillNetWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public Guid SeedPackageId { get; private set; }
    private SkillNetDbContext _db = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            // Thay SQL Server thật bằng LocalDB test instance
            services.RemoveAll<DbContextOptions<SkillNetDbContext>>();
            services.AddDbContext<SkillNetDbContext>(opts =>
                opts.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=SkillNetTest;Trusted_Connection=True"));

            // Disable Hangfire trong test
            services.AddHangfire(cfg => cfg.UseInMemoryStorage());

            // Mock email và Zalo — không gửi thật
            services.AddTransient<IEmailService, FakeEmailService>();
            services.AddTransient<IZaloOAService, FakeZaloService>();
        });
    }

    public async Task InitializeAsync()
    {
        _db = Services.CreateScope().ServiceProvider.GetRequiredService<SkillNetDbContext>();
        await _db.Database.EnsureCreatedAsync();

        // Seed gói dịch vụ
        var pkg = new ServicePackage { /* ... */ };
        SeedPackageId = pkg.Id;
        _db.ServicePackages.Add(pkg);
        await _db.SaveChangesAsync();
    }

    public HttpClient CreateAuthenticatedClient(string role = "Member")
    {
        var client = CreateClient();
        // Inject fake auth cookie với claims theo role
        client.DefaultRequestHeaders.Add("X-Test-UserId", $"test-user-{role}");
        client.DefaultRequestHeaders.Add("X-Test-Role", role);
        return client;
    }

    public async Task<Guid> SeedPendingApplicationAsync(string userId)
    {
        var app = MembershipApplication.Create(userId, SeedPackageId,
            "Test User", new DateOnly(1990,1,1), "0901234567", "Test address", null, null);
        _db.MembershipApplications.Add(app);
        await _db.SaveChangesAsync();
        return app.Id;
    }

    public async Task<MembershipApplication?> GetApplicationAsync(Guid id)
        => await _db.MembershipApplications.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == id);

    public Task DisposeAsync() => _db.Database.EnsureDeletedAsync();
}
```

---

## 5. Performance — k6 Load Test

```javascript
// tests/k6/membership-registration-load.js
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 10 },   // ramp-up
        { duration: '1m',  target: 50 },   // steady
        { duration: '30s', target: 0 },    // ramp-down
    ],
    thresholds: {
        'http_req_duration{endpoint:service-package}': ['p95<500'],
        'http_req_duration{endpoint:register-get}':    ['p95<500'],
        'http_req_duration{endpoint:admin-pending}':   ['p95<800'],
        'http_req_failed': ['rate<0.01'],   // error rate < 1%
    },
};

export default function () {
    // 1. Anonymous: Xem gói dịch vụ
    const pkg = http.get(`${__ENV.BASE_URL}/ServicePackage`,
        { tags: { endpoint: 'service-package' } });
    check(pkg, { 'packages page 200': r => r.status === 200 });

    sleep(1);

    // 2. Authenticated: GET form đăng ký (cần cookie từ fixture)
    const form = http.get(`${__ENV.BASE_URL}/Membership/Register`,
        { tags: { endpoint: 'register-get' }, headers: { Cookie: __ENV.AUTH_COOKIE } });
    check(form, { 'register form 200': r => r.status === 200 });

    sleep(2);
}

// Chạy: k6 run --env BASE_URL=http://localhost:5000 --env AUTH_COOKIE=... tests/k6/membership-registration-load.js
```

---

## 6. Test Data & Seed Fixtures

```csharp
// tests/SkillNet.Integration.Tests/Infrastructure/TestDataSeeder.cs
public static class TestDataSeeder
{
    // Accounts để test (tạo sẵn trong InitializeAsync)
    public const string MemberUserId    = "test-member-user-id";
    public const string AdminUserId     = "test-admin-user-id";
    public const string AnotherUserId   = "test-another-user-id";

    public static ServicePackage StandardPackage => new()
    {
        Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
        Name = "Tiêu chuẩn Test", Price = 500_000m,
        DurationMonths = 12, Benefits = "[\"Test benefit\"]",
        IsActive = true, DisplayOrder = 1,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };
}
```

---

## 7. Acceptance Test Checklist (UAT — PO sign-off)

### Happy Path
- [ ] Guest vào `/ServicePackage` → thấy 3 gói với giá, quyền lợi và nút "Đăng ký ngay".
- [ ] Click "Đăng ký ngay" khi chưa đăng nhập → redirect login → sau login về đúng gói đã chọn.
- [ ] Điền form hợp lệ, submit → redirect Status page với toast "Đơn đã được gửi".
- [ ] Vào `/Membership/Status` → thấy badge Pending + ngày nộp.
- [ ] Admin vào `/Admin/Membership/Pending` → thấy đơn vừa nộp.
- [ ] Admin click "Phê duyệt" → badge count giảm, status trên Status page chuyển Approved + có MembershipCode.
- [ ] Kiểm tra Hangfire dashboard `/hangfire` → Confirmation email job = Succeeded.

### Edge Cases
- [ ] Submit lần 2 khi đã có đơn Pending → thông báo "Đã có đơn chờ duyệt", không tạo thêm.
- [ ] Admin từ chối không nhập lý do → form báo lỗi, không reject.
- [ ] Admin từ chối có lý do → Status page member hiển thị lý do + CTA "Nộp đơn mới".
- [ ] Upload file > 5MB → báo lỗi ngay dưới field, không submit.
- [ ] Upload file `.exe` → báo lỗi định dạng không hợp lệ.

### Security
- [ ] Truy cập `/Admin/Membership/Pending` với account Member → 403 hoặc redirect.
- [ ] POST Approve không có CSRF token → 400 Bad Request.
- [ ] User A cố cancel đơn của User B → không thành công (403 hoặc 404).
- [ ] Trang `/Membership/Register` xuất hiện `<meta name="robots" content="noindex">`.

### Performance
- [ ] `/ServicePackage` TTFB < 500ms (đo từ Chrome DevTools).
- [ ] POST submit đơn hoàn tất trong < 1 giây (không tính email background job).
