---
name: senior-integration
description: Tích hợp Email (MailKit/SMTP), Zalo OA API và Hangfire background jobs cho ASP.NET Core — queue-based notification, retry mechanism, Polly resilience. Dùng khi implement email/Zalo notification, background job, scheduled task, hoặc tích hợp external service.
---

## Senior Integration – Project Skill (Email, Zalo, Background Jobs)

### 1. Mục tiêu vai trò
- **Tập trung**: Thiết kế và triển khai tích hợp các hệ thống bên ngoài cho ứng dụng ASP.NET Core: **Email (MailKit/SendGrid)**, **Zalo OA API**, **Background Jobs (Hangfire)** — đảm bảo giao tiếp bất đồng bộ, retry mechanism và không mất notification.
- **Thành công**: Email và Zalo notification gửi thành công > 99%, có retry khi thất bại, không block luồng chính nghiệp vụ.

### 2. Architecture — Queue-based Notification

```
User Action (POST)
    → Application Handler
        → Enqueue Job vào Hangfire Queue
            → Background Worker gửi Email
            → Background Worker gửi Zalo
```

**Nguyên tắc**: Không gọi Email/Zalo trực tiếp trong HTTP request handler. Luôn enqueue job → worker xử lý async.

### 3. Email — MailKit (SMTP)

#### Cài đặt
```xml
<PackageReference Include="MailKit" Version="4.*" />
```

#### Interface (Application Layer)
```csharp
// Application/Interfaces/IEmailService.cs
public interface IEmailService
{
    Task SendMembershipApprovalAsync(string toEmail, string fullName, string membershipCode, CancellationToken ct = default);
    Task SendMembershipRejectionAsync(string toEmail, string fullName, string reason, CancellationToken ct = default);
    Task SendOtpEmailAsync(string toEmail, string otp, CancellationToken ct = default);
}
```

#### Implementation (Infrastructure Layer)
```csharp
// Infrastructure/Services/Email/MailKitEmailService.cs
public class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<MailKitEmailService> _logger;
    
    public MailKitEmailService(IOptions<EmailSettings> settings, ILogger<MailKitEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }
    
    public async Task SendMembershipApprovalAsync(
        string toEmail, string fullName, string membershipCode, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(fullName, toEmail));
        message.Subject = "Chúc mừng! Đơn đăng ký hội viên của bạn đã được phê duyệt";
        
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <h2>Xin chào {fullName},</h2>
                <p>Đơn đăng ký hội viên của bạn đã được <strong>phê duyệt</strong>.</p>
                <p>Mã hội viên: <strong>{membershipCode}</strong></p>
                <p>Trân trọng,<br/>Ban Quản lý QMV</p>"
        };
        message.Body = bodyBuilder.ToMessageBody();
        
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl, ct);
            await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
            await client.SendAsync(message, ct);
            _logger.LogInformation("Email phê duyệt gửi thành công tới {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi gửi email tới {Email}", toEmail);
            throw;  // Để Hangfire retry
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}

// Infrastructure/Settings/EmailSettings.cs
public class EmailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
}
```

#### appsettings.json (không commit password)
```json
{
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "dangky@qmv.com",
    "SenderEmail": "dangky@qmv.com",
    "SenderName": "QMV System"
  }
}
```

### 4. Zalo OA API

#### Interface
```csharp
// Application/Interfaces/IZaloService.cs
public interface IZaloService
{
    Task SendNewApplicationNotificationAsync(
        string applicantName, string email, string packageName, CancellationToken ct = default);
    Task SendApprovalNotificationAsync(
        string applicantName, string membershipCode, CancellationToken ct = default);
}
```

#### Implementation
```csharp
// Infrastructure/Services/Zalo/ZaloOaService.cs
public class ZaloOaService : IZaloService
{
    private readonly HttpClient _httpClient;
    private readonly ZaloSettings _settings;
    private readonly ILogger<ZaloOaService> _logger;
    
    public ZaloOaService(
        HttpClient httpClient,
        IOptions<ZaloSettings> settings,
        ILogger<ZaloOaService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }
    
    public async Task SendNewApplicationNotificationAsync(
        string applicantName, string email, string packageName, CancellationToken ct = default)
    {
        var payload = new
        {
            recipient = new { user_id = _settings.AccountingZaloUserId },
            message = new
            {
                text = $"🔔 Đơn đăng ký hội viên mới!\n" +
                       $"👤 Họ tên: {applicantName}\n" +
                       $"📧 Email: {email}\n" +
                       $"📦 Gói: {packageName}\n" +
                       $"⏰ Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}"
            }
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post, "https://openapi.zalo.me/v3.0/oa/message/cs")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Add("access_token", _settings.OaAccessToken);
        
        var response = await _httpClient.SendAsync(request, ct);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Zalo API lỗi: {StatusCode} — {Body}", response.StatusCode, errorBody);
            throw new ZaloServiceException($"Zalo API trả lỗi {response.StatusCode}");
        }
        
        _logger.LogInformation("Zalo notification gửi thành công cho đơn của {Name}", applicantName);
    }
}

// Infrastructure/Settings/ZaloSettings.cs
public class ZaloSettings
{
    public string OaAccessToken { get; set; } = string.Empty;
    public string AccountingZaloUserId { get; set; } = string.Empty;
}
```

#### Đăng ký HttpClient (với Polly retry)
```csharp
// Infrastructure/DependencyInjection.cs
services.AddHttpClient<ZaloOaService>()
    .AddStandardResilienceHandler(); // .NET 8+ Polly retry tích hợp sẵn
    // Hoặc tùy chỉnh:
    // .AddPolicyHandler(GetZaloRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetZaloRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
```

### 5. Hangfire — Background Jobs

#### Cài đặt
```xml
<PackageReference Include="Hangfire.AspNetCore" Version="1.*" />
<PackageReference Include="Hangfire.SqlServer" Version="1.*" />
```

#### Cấu hình
```csharp
// Program.cs
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.FromSeconds(15),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
    options.Queues = new[] { "critical", "default", "low" };
});

// Dashboard (bảo vệ bằng Authorization)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});
```

#### Job Definitions
```csharp
// Application/Jobs/NotificationJobs.cs
public class NotificationJobs
{
    private readonly IEmailService _emailService;
    private readonly IZaloService _zaloService;
    
    public NotificationJobs(IEmailService emailService, IZaloService zaloService)
    {
        _emailService = emailService;
        _zaloService = zaloService;
    }
    
    [Queue("default")]
    public async Task SendMembershipApprovalNotificationsAsync(
        string email, string fullName, string membershipCode, string packageName)
    {
        // Gửi cả Email và Zalo notification
        await _emailService.SendMembershipApprovalAsync(email, fullName, membershipCode);
        await _zaloService.SendApprovalNotificationAsync(fullName, membershipCode);
    }
    
    [Queue("low")]
    public async Task SendWeeklyReportAsync(string reportRecipientEmail)
    {
        // Weekly scheduled job
    }
}
```

#### Enqueue Job từ Application Handler
```csharp
// Application/Handlers/ApproveApplicationCommandHandler.cs
public class ApproveApplicationCommandHandler : IRequestHandler<ApproveApplicationCommand, Result>
{
    private readonly IApplicationRepository _repo;
    private readonly IBackgroundJobClient _jobs;
    
    public async Task<Result> Handle(ApproveApplicationCommand cmd, CancellationToken ct)
    {
        var application = await _repo.GetByIdAsync(cmd.ApplicationId, ct)
            ?? throw new NotFoundException(nameof(MembershipApplication), cmd.ApplicationId);
        
        application.Approve(cmd.ApprovedBy);
        await _repo.SaveChangesAsync(ct);
        
        // Enqueue job — không await, chạy async background
        _jobs.Enqueue<NotificationJobs>(jobs =>
            jobs.SendMembershipApprovalNotificationsAsync(
                application.Email,
                application.FullName,
                application.MembershipCode,
                application.ServicePackage.Name));
        
        return Result.Success();
    }
}
```

#### Recurring Jobs (Scheduled Tasks)
```csharp
// Trong Program.cs sau khi app build
using var scope = app.Services.CreateScope();
var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

// Gửi báo cáo hàng tuần — Thứ Hai 8:00 AM (cron expression)
recurringJobs.AddOrUpdate<NotificationJobs>(
    "weekly-report",
    jobs => jobs.SendWeeklyReportAsync("ketoan@qmv.com"),
    "0 8 * * 1");  // Cron: 8AM every Monday
```

### 6. DI Registration tổng hợp

```csharp
// Infrastructure/DependencyInjection.cs
public static IServiceCollection AddIntegrationServices(
    this IServiceCollection services, IConfiguration config)
{
    // Email
    services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
    services.AddTransient<IEmailService, MailKitEmailService>();
    
    // Zalo
    services.Configure<ZaloSettings>(config.GetSection("ZaloSettings"));
    services.AddHttpClient<ZaloOaService>()
        .AddStandardResilienceHandler();
    services.AddTransient<IZaloService, ZaloOaService>();
    
    // Background Jobs
    services.AddHangfire(/* ... */);
    services.AddHangfireServer();
    
    return services;
}
```

### 7. Checklist Integration

- [ ] Không gọi Email/Zalo trực tiếp trong HTTP request handler — luôn qua Hangfire queue.
- [ ] Retry mechanism được cấu hình cho Hangfire (mặc định 10 lần với exponential backoff).
- [ ] Secrets (SMTP password, Zalo token) không hardcode — dùng environment variables / Key Vault.
- [ ] Hangfire Dashboard được bảo vệ bằng Authorization.
- [ ] Log đầy đủ: success, failure, retry attempts.
- [ ] Email template có fallback text nếu HTML không hiển thị.
- [ ] Test gửi email/Zalo trên môi trường Staging với account test riêng.
- [ ] Monitor Hangfire failed jobs queue.

### 8. Anti-pattern cần tránh

| Anti-pattern | Vấn đề | Giải pháp |
|---|---|---|
| Gọi Email trong HTTP handler (await trực tiếp) | User phải chờ, nếu SMTP chậm → timeout | Hangfire enqueue |
| Không có retry | Mạng tạm thời lỗi → mất notification | Hangfire auto-retry |
| Secret trong appsettings.json | Lộ credential khi commit | Environment variables |
| Không log kết quả | Khó debug khi notification thất bại | Structured logging |
| Gọi Zalo API trực tiếp không có timeout | Thread bị block nếu Zalo chậm | HttpClient timeout config |
