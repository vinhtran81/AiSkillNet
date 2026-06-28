---
name: senior-logging
description: Thiết lập observability toàn diện cho ASP.NET Core — Serilog structured logging, Correlation ID, Health Checks (/health/ready/live), Global Exception Handler, MediatR LoggingBehavior. Dùng khi cấu hình logging, debug log issue trên production, hoặc thiết lập health monitoring.
---

## Senior Logging & Observability – Project Skill

### 1. Mục tiêu vai trò
- **Tập trung**: Thiết lập hệ thống logging, monitoring và health check toàn diện cho ứng dụng ASP.NET Core — sử dụng **Serilog** (structured logging), **Health Checks**, và tích hợp với **Application Insights** hoặc **Seq**.
- **Thành công**: Mọi lỗi production đều có log đủ context để debug trong < 10 phút, health check tự động phát hiện vấn đề trước khi user báo cáo.

### 2. Serilog Setup

#### Cài đặt
```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.*" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.*" />
<PackageReference Include="Serilog.Sinks.File" Version="5.*" />
<PackageReference Include="Serilog.Sinks.Seq" Version="7.*" />
<!-- Nếu dùng Azure Application Insights -->
<PackageReference Include="Serilog.Sinks.ApplicationInsights" Version="4.*" />
```

#### Cấu hình (Program.cs)
```csharp
// Program.cs — cấu hình Serilog TRƯỚC khi build app
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(outputTemplate: 
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/app-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
    .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();
```

#### Structured Logging — chuẩn
```csharp
// ✅ ĐÚNG: Dùng message template với named properties
_logger.LogInformation(
    "Đơn đăng ký {ApplicationId} của {ApplicantEmail} đã được phê duyệt bởi {ApprovedBy}",
    applicationId, applicantEmail, approvedBy);

_logger.LogWarning(
    "Gửi email thất bại lần {RetryCount} cho {Email}. Lỗi: {ErrorMessage}",
    retryCount, email, ex.Message);

_logger.LogError(ex,
    "Lỗi xử lý đơn {ApplicationId}. UserId: {UserId}",
    applicationId, userId);

// ❌ SAI: String concatenation — mất structured properties
_logger.LogInformation("Đơn " + applicationId + " đã duyệt"); // Không searchable!
```

#### Request Logging Middleware
```csharp
// Sau builder.Build(), trước các middleware khác
app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        diagnosticContext.Set("UserId", httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous");
    };
});
```

### 3. Correlation ID (Trace Requests)

```csharp
// Thêm package
// <PackageReference Include="CorrelationId" Version="3.*" />

// Program.cs
builder.Services.AddDefaultCorrelationId(options =>
{
    options.AddToLoggingScope = true;
    options.EnforceHeader = false;
    options.IncludeInResponse = true;
    options.RequestHeader = "X-Correlation-Id";
    options.ResponseHeader = "X-Correlation-Id";
});

app.UseCorrelationId();

// Kết quả: mọi log trong 1 request sẽ có CorrelationId chung → dễ trace
```

### 4. Health Checks

#### Cài đặt
```xml
<PackageReference Include="AspNetCore.HealthChecks.SqlServer" Version="8.*" />
<PackageReference Include="AspNetCore.HealthChecks.UI" Version="8.*" />
<PackageReference Include="AspNetCore.HealthChecks.UI.Client" Version="8.*" />
```

#### Cấu hình
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    // Database
    .AddSqlServer(
        connectionString: config.GetConnectionString("DefaultConnection")!,
        healthQuery: "SELECT 1",
        name: "sql-server",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "ready" })
    // Zalo API (external)
    .AddUrlGroup(
        uri: new Uri("https://openapi.zalo.me"),
        name: "zalo-api",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "external" })
    // Disk space
    .AddDiskStorageHealthCheck(setup => setup.AddDrive("C:\\", 1024), // 1GB min
        name: "disk-space",
        tags: new[] { "infrastructure" })
    // Custom business check
    .AddCheck<HangfireHealthCheck>("hangfire", tags: new[] { "jobs" });

// Map endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

// Chỉ kiểm tra DB — dùng cho load balancer readiness probe
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Luôn 200 nếu process còn sống — dùng cho liveness probe
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
```

#### Custom Health Check
```csharp
// Infrastructure/HealthChecks/HangfireHealthCheck.cs
public class HangfireHealthCheck : IHealthCheck
{
    private readonly IMonitoringApi _monitoringApi;
    
    public HangfireHealthCheck(IMonitoringApi monitoringApi)
        => _monitoringApi = monitoringApi;
    
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        var failedJobs = _monitoringApi.FailedCount();
        
        return Task.FromResult(failedJobs switch
        {
            0 => HealthCheckResult.Healthy("Hangfire hoạt động bình thường"),
            <= 10 => HealthCheckResult.Degraded($"Có {failedJobs} job đang thất bại"),
            _ => HealthCheckResult.Unhealthy($"Có {failedJobs} job thất bại — cần kiểm tra ngay")
        });
    }
}
```

### 5. Global Exception Handler

```csharp
// Infrastructure/Middleware/GlobalExceptionMiddleware.cs
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(new { errors = ex.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred. Path: {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            // Không lộ stack trace trên production
            var message = context.RequestServices
                .GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                ? ex.Message
                : "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";
            
            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }
}
```

### 6. Performance Logging (MediatR Behavior)

```csharp
// Application/Behaviors/LoggingBehavior.cs
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();
        
        _logger.LogInformation("Bắt đầu xử lý {RequestName}: {@Request}", requestName, request);
        
        try
        {
            var response = await next();
            sw.Stop();
            
            if (sw.ElapsedMilliseconds > 500)
                _logger.LogWarning("Request chậm! {RequestName} mất {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            else
                _logger.LogInformation("Hoàn thành {RequestName} trong {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Lỗi xử lý {RequestName} sau {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### 7. appsettings Logging Configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithEnvironmentName"]
  }
}
```

### 8. Checklist Observability

- [ ] Serilog đã cấu hình với structured logging (message templates, không string concat).
- [ ] Log level phù hợp theo môi trường: Debug (dev), Information (staging), Warning (prod).
- [ ] Correlation ID được thêm vào tất cả request log.
- [ ] Global Exception Handler che stack trace trên production.
- [ ] Health Check endpoint `/health`, `/health/ready`, `/health/live` hoạt động.
- [ ] SQL slow query được log (query > 500ms).
- [ ] Hangfire failed jobs được monitor.
- [ ] Log file có rotation policy (tránh đầy đĩa).
- [ ] Dashboard Seq hoặc Application Insights được cấu hình (nếu dùng).
