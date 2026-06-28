---
type: Logging & Observability Plan
feature_id: FEAT-001
status: DRAFT
version: 1.0
created_at: 2026-06-27
depends_on: membership-registration_architecture.md
---

# Logging & Observability Plan — FEAT-001

---

## 1. Log Points by Layer

### 1.1 MediatR Handlers

```csharp
// SubmitMembershipApplicationHandler
LogInformation("Membership application submitted. UserId={UserId} PackageId={PackageId} ApplicationId={ApplicationId}",
    cmd.UserId, cmd.ServicePackageId, application.Id);

LogWarning("Membership application blocked: user {UserId} already has a Pending application.",
    cmd.UserId);

// ApproveMembershipApplicationHandler
LogInformation("Membership application approved. ApplicationId={ApplicationId} AdminId={AdminId} MembershipCode={MembershipCode}",
    cmd.ApplicationId, cmd.AdminId, application.MembershipCode);

// RejectMembershipApplicationHandler
LogInformation("Membership application rejected. ApplicationId={ApplicationId} AdminId={AdminId} ReasonLength={Length}",
    cmd.ApplicationId, cmd.AdminId, cmd.RejectionReason.Length);

// CancelMembershipApplicationHandler
LogInformation("Membership application cancelled by user. ApplicationId={ApplicationId} UserId={UserId}",
    cmd.ApplicationId, cmd.UserId);
```

### 1.2 Hangfire Jobs

```csharp
// SendApplicationConfirmationEmailJob
LogInformation("Sending confirmation email. ApplicationId={ApplicationId} To={Email}",
    applicationId, MaskEmail(userEmail));
// Sau khi gửi xong:
LogInformation("Confirmation email sent successfully. ApplicationId={ApplicationId} DurationMs={Ms}",
    applicationId, stopwatch.ElapsedMilliseconds);

// SendApplicationResultNotificationJob
LogInformation("Sending result notification. ApplicationId={ApplicationId} Status={Status} Channels=[Email,Zalo]",
    applicationId, status);

LogWarning("Zalo OA notification skipped — no phone number on record. ApplicationId={ApplicationId}",
    applicationId);

LogError(ex, "Failed to send result notification. ApplicationId={ApplicationId} Status={Status} Attempt={Attempt}",
    applicationId, status, attempt);

// RemindAdminPendingApplicationsJob
LogInformation("Admin reminder: {Count} applications pending > 3 days. Sending alert to {AdminEmail}",
    overduePending.Count, MaskEmail(adminEmail));

LogInformation("No overdue pending applications. Skipping admin reminder.");
```

### 1.3 MVC Controllers

```csharp
// MembershipController — chỉ log exception/warning, không log mỗi GET request
LogWarning("Membership form submitted with invalid ModelState. UserId={UserId} Errors={Errors}",
    userId, string.Join(", ", ModelState.GetErrors()));

// AdminMembershipController
LogWarning("Attempt to approve non-existent or non-Pending application. ApplicationId={Id} AdminId={AdminId}",
    id, adminId);
```

### 1.4 Log Level Convention

| Tình huống | Level | Ghi chú |
|-----------|-------|---------|
| Business action thành công (submit/approve/reject) | `Information` | Luôn có `ApplicationId` |
| Business rule block (has Pending, not found) | `Warning` | Không phải lỗi hệ thống |
| External service fail (Email/Zalo SMTP timeout) | `Error` | Kèm exception + attempt count |
| Security violation (unauthorized access attempt) | `Warning` | Từ middleware/auth filter |
| Unhandled exception | `Error` / `Critical` | Global exception handler |

---

## 2. Structured Logging — Serilog Format

```csharp
// src/SkillNet.Web/Program.cs — Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "SkillNet")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq(builder.Configuration["Seq:Url"]!)   // local dev / staging
    // Production: .WriteTo.ApplicationInsights(...)
    .CreateLogger();
```

**Bắt buộc có trong mọi log liên quan FEAT-001:**

| Property | Ý nghĩa | Ví dụ |
|----------|---------|-------|
| `ApplicationId` | Guid của đơn | `"3fa85f64-..."` |
| `UserId` | Identity user ID | `"auth0\|abc..."` |
| `AdminId` | ID admin xử lý | `"auth0\|xyz..."` |
| `Status` | Trạng thái đơn | `"Approved"` |
| `DurationMs` | Thời gian xử lý | `142` |

**KHÔNG log:**
```
❌ Email plaintext:  user@example.com  → dùng MaskEmail() → "u***@e***.com"
❌ Phone plaintext:  0901234567        → dùng MaskPhone() → "090***4567"
❌ FullName:         Nguyễn Văn A      → không cần thiết trong log
❌ Stack trace trong response body     → chỉ log server-side
```

```csharp
// src/SkillNet.Infrastructure/Helpers/LogMaskHelper.cs
public static class LogMaskHelper
{
    public static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return "***";
        return $"{parts[0][0]}***@{parts[1][0]}***.{parts[1].Split('.').Last()}";
    }

    public static string MaskPhone(string phone)
        => phone.Length >= 10 ? $"{phone[..3]}***{phone[^4..]}" : "***";
}
```

---

## 3. Health Checks

```csharp
// src/SkillNet.Web/Program.cs — thêm Health Check cho dependencies của FEAT-001
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("Default")!,
        name: "sql-server",
        tags: ["db", "ready"])
    .AddHangfire(
        options => options.MinimumAvailableServers = 1,
        name: "hangfire",
        tags: ["jobs", "ready"])
    .AddUrlGroup(
        uri: new Uri(builder.Configuration["Zalo:HealthCheckUrl"]!),
        name: "zalo-oa",
        tags: ["external", "live"])
    .AddSmtpHealthCheck(
        options =>
        {
            options.Host = builder.Configuration["Email:Host"]!;
            options.Port = int.Parse(builder.Configuration["Email:Port"]!);
        },
        name: "smtp-email",
        tags: ["external", "live"]);

// Health Check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Tách /health/ready (DB + Hangfire) và /health/live (external services)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("live")
});
```

**Expected response `/health` khi khỏe:**
```json
{
  "status": "Healthy",
  "results": {
    "sql-server":  { "status": "Healthy", "duration": "00:00:00.012" },
    "hangfire":    { "status": "Healthy" },
    "zalo-oa":     { "status": "Healthy" },
    "smtp-email":  { "status": "Healthy" }
  }
}
```

---

## 4. Alert Thresholds

### 4.1 Application Insights / Seq Alerts

| Metric | Ngưỡng cảnh báo | Hành động |
|--------|----------------|-----------|
| Error log rate (FEAT-001) | > 5 errors trong 5 phút | PagerDuty / email alert to on-call |
| `SendApplicationResultNotificationJob` Failed | Bất kỳ job Failed state | Alert admin qua email |
| `http_req_duration` POST `/Membership/Register` | p95 > 1s | Review DB query plan, EF Core N+1 |
| `http_req_duration` GET `/Admin/Membership/Pending` | p95 > 800ms | Review pagination, add index |
| `MembershipApplications` Pending count | > 50 đơn đang Pending | Alert Admin Email (RemindAdminJob) |

### 4.2 Hangfire Job Monitoring

```csharp
// Hangfire Dashboard filter — chỉ Admin mới vào được
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        return http.User.IsInRole("Admin");
    }
}
```

**Jobs cần monitor sau FEAT-001 go-live:**

| Job | Thời gian chạy kỳ vọng | Alert nếu |
|-----|----------------------|-----------|
| `SendApplicationConfirmationEmailJob` | < 3s | Failed sau 3 retry |
| `SendApplicationResultNotificationJob` | < 5s | Failed sau 3 retry |
| `RemindAdminPendingApplicationsJob` | < 10s (daily) | Không chạy > 25h liên tiếp |

### 4.3 Custom Application Insights Metrics

```csharp
// Trong ApproveMembershipApplicationHandler — track business metric
_telemetryClient.TrackEvent("membership_approved", new Dictionary<string, string>
{
    ["application_id"] = application.Id.ToString(),
    ["package_id"]     = application.ServicePackageId.ToString(),
    ["days_pending"]   = ((DateTime.UtcNow - application.CreatedAt).Days).ToString()
});
_telemetryClient.TrackMetric("membership.days_to_approve",
    (DateTime.UtcNow - application.CreatedAt).TotalDays);
```

---

## 5. Post-Deploy Monitoring Checklist

Ngay sau khi deploy FEAT-001 lên Production, kiểm tra trong **15 phút đầu**:

- [ ] `GET /health` → tất cả `Healthy`.
- [ ] `GET /ServicePackage` → trang load đúng, không có error trong Seq/AppInsights.
- [ ] Thực hiện 1 đơn đăng ký test → kiểm tra Hangfire dashboard: `SendApplicationConfirmationEmailJob` = Succeeded.
- [ ] Admin thực hiện 1 lần Approve test → `SendApplicationResultNotificationJob` = Succeeded.
- [ ] Kiểm tra Serilog: không có `Error` hay `Critical` log mới phát sinh.
- [ ] Kiểm tra `/hangfire` dashboard: không có job ở trạng thái `Failed`.
- [ ] Kiểm tra Application Insights: error rate = 0%, p95 latency < 500ms trên `/Membership/Register`.
