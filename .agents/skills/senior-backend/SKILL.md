---
name: senior-backend
description: Thiết kế và implement back-end ASP.NET Core — API Controllers, database schema (SQL Server/EF Core), JWT auth, caching, security. Dùng khi implement API endpoint, xử lý business logic server-side, thiết kế DB schema, review bảo mật backend, hoặc viết CQRS Handler.
---

## Senior Back-end – Project Skill (ASP.NET Core API + MVC)

### 1. Mục tiêu vai trò
- **Tập trung**: Thiết kế và vận hành hệ thống back-end an toàn, ổn định, hiệu năng cao và dễ mở rộng sử dụng **ASP.NET Core (C#)** làm framework phát triển API chính và **Microsoft SQL Server** làm hệ quản trị cơ sở dữ liệu.
- **Thành công**: API thiết kế chuẩn RESTful, sử dụng tối ưu các tính năng của ASP.NET Core, toàn vẹn dữ liệu (Data Integrity) ở mức tối đa, truy vấn SQL Server hiệu quả và đảm bảo an toàn bảo mật thông tin.

### 2. Nguyên tắc cốt lõi
- **API first & contract clear**: API được thiết kế từ use case nghiệp vụ thực tế, contract rõ ràng (Request, Response, Error Code), tài liệu đầy đủ (Swagger/OpenAPI).
- **Clean Architecture & Dependency Injection**: Tổ chức code theo các Layer rõ ràng (API, Application, Domain, Infrastructure), tận dụng tối đa Built-in Dependency Injection của .NET.
- **Asynchronous Programming**: Sử dụng lập trình bất đồng bộ (`async`/`await` và `Task`) xuyên suốt để tối ưu hóa khả năng xử lý đồng thời (Concurrency) của web server.
- **Data integrity & security**: Thiết kế schema chuẩn hóa, ràng buộc chặt chẽ, phân quyền truy cập và phòng chống SQL Injection triệt để.
- **Performance & scalability**: Thiết kế Index thông minh, kiểm soát Transaction Isolation Levels, tránh lỗi N+1, Parameter Sniffing và tối ưu hóa kết nối DB (Connection Pooling).
- **Observability**: Triển khai log (Serilog/Structured Logging), tracing, và giám sát hiệu năng (Application Insights, Query Store) để phát hiện và xử lý sự cố kịp thời.

### 3. Quy trình làm việc đề xuất
1. **Hiểu domain & use case**
   - Làm việc chặt chẽ với PO & FE để làm rõ luồng dữ liệu, dữ liệu cần lưu trữ và các kịch bản truy vấn thực tế.
   - Xác định rõ các thực thể (Entities), mối quan hệ (Relationships) và các ràng buộc nghiệp vụ (Business Rules).
2. **Thiết kế cơ sở dữ liệu & API**
   - Thiết kế schema chi tiết, lựa chọn kiểu dữ liệu phù hợp (ưu tiên kiểu dữ liệu tối ưu kích thước lưu trữ như `VARCHAR` thay vì `NVARCHAR` khi không cần Unicode, dùng `DATETIME2` thay vì `DATETIME`).
   - **Database / SQL Server**: Định nghĩa cấu trúc bảng, khóa chính, khóa ngoại, ràng buộc. 
     - Quản lý cơ sở dữ liệu bằng Entity Framework Core Migrations (hoặc Flyway, DbUp, SSDT / Dacpac). Tuyệt đối không chỉnh sửa schema thủ công trực tiếp trên các môi trường dùng chung.
     - **Diagram**: Luôn tạo ER Diagram bằng Mermaid để trực quan hóa mối quan hệ giữa các bảng.
   - Định nghĩa chi tiết API contract: endpoint, HTTP method, request/response body và mã lỗi (error codes). Tích hợp Swagger để FE dễ dàng tích hợp.
3. **Triển khai (Implement)**
   - Viết Web API (sử dụng Controller hoặc Minimal APIs) bằng C#.
   - Luôn sử dụng Parameterized Query (hoặc thông qua EF Core/Dapper) để phòng ngừa tấn công SQL Injection và giúp SQL Server tái sử dụng Execution Plan hiệu quả.
   - Đảm bảo validate dữ liệu đầu vào sử dụng Model Validation (DataAnnotations) hoặc FluentValidation trước khi xử lý.
   - Xử lý lỗi tập trung thông qua Middleware (như `UseExceptionHandler` hoặc `IExceptionHandler` trong .NET 8+) để trả về định dạng chuẩn `ProblemDetails` (RFC 7807).
4. **Kiểm thử & Tối ưu (Test & Hardening)**
   - Viết unit/integration tests (Sử dụng xUnit/NUnit và WebApplicationFactory) kiểm thử logic nghiệp vụ và tính chính xác của các câu lệnh SQL/EF Core quan trọng.
   - Phân tích Execution Plan của các truy vấn có tần suất sử dụng cao hoặc chậm để tối ưu hóa Index.
   - Kiểm tra và xử lý các vấn đề liên quan đến concurrency, đảm bảo không xảy ra deadlock khi chịu tải cao.

### 4. Checklist trước khi expose API
- **Correctness**
   - Đáp ứng trọn vẹn và chính xác tất cả các use case nghiệp vụ từ phía FE/PO.
   - Xử lý đầy đủ tất cả các trường hợp lỗi (Not Found, Validation, Access Denied, Conflict, Internal Database Error...).
- **Performance**
   - Đã review kỹ câu lệnh SQL/EF Core: có Clustered/Non-Clustered Index phù hợp, sử dụng `INCLUDE` columns để tạo Covering Index nếu cần, không bị lỗi N+1 (luôn nhớ dùng `.Include()` hoặc `.Select()` hợp lý trong EF Core để tránh lazy loading ngoài ý muốn).
   - Hạn chế sử dụng Cursor hoặc Loop dưới database; ưu tiên xử lý dạng Set-based.
   - Có phân trang (Pagination) rõ ràng cho các API trả về danh sách lớn.
- **Security**
   - Đảm bảo tất cả đầu vào của API đều được validate kỹ càng trước khi đưa vào truy vấn.
   - Tài khoản kết nối cơ sở dữ liệu của ứng dụng tuân thủ nguyên tắc đặc quyền tối thiểu (Least Privilege).
   - Xác thực và phân quyền chuẩn xác sử dụng JWT Authentication và Policy-based Authorization.
- **Contract**
   - Cấu trúc Response thống nhất cho cả trường hợp thành công và thất bại.
   - Mã lỗi (Error Code) và thông điệp lỗi rõ ràng giúp FE và QC dễ dàng định vị lỗi.

### 5. Anti-pattern cần tránh
- Để Front-end phải xử lý quá nhiều logic tính toán/aggregate mà đáng lẽ phải do Back-end đảm nhiệm.
- Thiết kế cơ sở dữ liệu phục vụ riêng lẻ cho giao diện hiển thị của màn hình thay vì đi từ mô hình Domain thực tế.
- Sử dụng `SELECT *` bừa bãi, hoặc gọi `.ToList()` quá sớm trong EF Core trước khi áp dụng các bộ lọc (gây kéo toàn bộ bảng về bộ nhớ ứng dụng).
- Thực hiện các thao tác viết/đọc DB liên tục trong các vòng lặp (gây ra N+1 query) thay vì dùng Batch Insert/Update hoặc Join tối ưu.
- Block đồng bộ bằng cách gọi `.Result` hoặc `.Wait()` trên các Task bất đồng bộ (gây hiện tượng Thread Pool Starvation / Deadlock).
- Không cấu hình cơ chế Locking phù hợp, gây ra hiện tượng Block/Deadlock diện rộng trên database.

### 6. Cách phối hợp với các vai trò khác
- **PO (Product Owner)**
   - Làm rõ các ràng buộc kỹ thuật của DB và Framework, đề xuất các giải pháp hoặc đánh giá các trade-off giữa hiệu năng hệ thống, độ phức tạp và tiến độ dự án.
   - Gợi ý đơn giản hóa hoặc điều chỉnh yêu cầu nghiệp vụ để hệ thống hoạt động mượt mà hơn dưới DB.
- **UI/UX & Front-end**
   - Thống nhất API contract từ sớm thông qua Swagger/OpenAPI. Tư vấn cho FE về cách tổ chức phân trang, cơ chế caching dữ liệu, và các điều kiện lọc (Filter/Search/Sort) để đạt hiệu năng tốt nhất ở cả hai phía.
   - Phản hồi khi phát hiện FE gửi các request dư thừa hoặc gọi liên tục các API nặng.
- **QC (Quality Control)**
   - Cung cấp đầy đủ tài liệu API (Swagger), các request/response mẫu và danh sách chi tiết các mã lỗi.
   - Hỗ trợ QC thiết kế các kịch bản test dữ liệu lớn, kiểm thử độ chịu tải (Load Test) và các ca kiểm thử đồng thời (Concurrency/Race Condition).

### 7. Security Audit Checklist — ASP.NET Core & SQL Server
Khi thực hiện kiểm tra bảo mật hệ thống dữ liệu, luôn rà soát:
1.  **SQL Injection Prevention**: Đảm bảo toàn bộ truy vấn đều tham số hóa. Tránh viết raw SQL cộng chuỗi; luôn sử dụng EF Core LINQ parameterized query hoặc tham số hóa trong Dapper.
2.  **Least Privilege Access**: Ứng dụng kết nối tới DB bằng tài khoản có quyền tối thiểu, không dùng `sa` hay `db_owner`.
3.  **Connection Security**: Connection String phải cấu hình mã hóa kết nối bắt buộc (`Encrypt=True`) và chỉ định chứng chỉ bảo mật hợp lệ (`TrustServerCertificate=False` trên production).
4.  **Authorization**: Đảm bảo mọi Endpoint đều được bảo vệ bởi attribute `[Authorize]` hoặc cấu hình authorization policy phù hợp, trừ các endpoint cố ý public (`[AllowAnonymous]`).
5.  **CORS & Security Headers**: Cấu hình CORS chặt chẽ, chỉ cho phép các domain được tin cậy kết nối đến API. Bật các header bảo mật cần thiết (HSTS, X-Content-Type-Options, v.v.).
6.  **Sensitive Data Protection**: Mã hóa dữ liệu nhạy cảm trước khi lưu xuống DB. Không lưu trữ Connection String dạng bản rõ trong `appsettings.json` trên Production (sử dụng User Secrets khi dev và Key Vault / Environment Variables trên production).

### 8. Deployment Readiness Checklist
Truoc khi deploy len moi truong Production, luon kiem tra:
1.  **Pending Migrations and Rollback Plan**: Xac nhan tat ca cac file migration moi da duoc ap dung thanh cong tren moi truong Staging/UAT ma khong gap loi. Chuan bi san kich ban va script rollback.
2.  **Read Committed Snapshot Isolation (RCSI)**: Kiem tra cau hinh SQL Server xem da bat RCSI chua (ALTER DATABASE [DbName] SET READ_COMMITTED_SNAPSHOT ON;). Rat quan trong de tranh hien tuong nguoi doc block nguoi viet, giam thieu Lock/Deadlock.
3.  **Connection String Configuration**: Kiem tra Connection String cho Production, su dung Windows Authentication / Microsoft Entra ID Managed Identity tren Azure SQL, tranh hardcode password.
4.  **Logging Levels**: Cau hinh log level phu hop tren Production (Information hoac Warning), tranh Debug qua chi tiet lam day o dia hoac ro ri thong tin nhay cam.
5.  **Sensitive Data Protection**: Ma hoa du lieu nhay cam truoc khi luu xuong DB, hoac ap dung Always Encrypted / Transparent Data Encryption (TDE) cua SQL Server de bao ve file .mdf/.ldf.
6.  **Maintenance Plan and Index Stats**: Xac minh da len lich bao tri dinh ky: Update Statistics, Reorganize/Rebuild Index va tu dong Backup (Full, Differential, Transaction Log).
7.  **Sensitive Configurations**: Dam bao tat moi tuy chon trace debug hoac hien thi loi chi tiet tu DB tra ve cho client tren Production.

### 9. MVC Render Mode — Khi kết hợp API + MVC

Khi project vừa có **Web API** (trả JSON) vừa có **MVC Controllers** (trả View), cần phân tách rõ:

#### Phân biệt API Controller vs MVC Controller
```csharp
// API Controller — trả JSON, dùng cho AJAX / mobile / external
[ApiController]
[Route("api/[controller]")]
public class MembershipApplicationsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDto>> GetById(Guid id)
        => Ok(await _mediator.Send(new GetApplicationByIdQuery(id)));
}

// MVC Controller — trả View, dùng cho server-side render
[Route("admin/membership")]
public class AdminMembershipController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _mediator.Send(new GetApplicationsPagedQuery(page, 20));
        return View(result);
    }
    
    [HttpPost("approve/{id}")]
    [ValidateAntiForgeryToken]  // Bắt buộc cho MVC form action
    public async Task<IActionResult> Approve(Guid id)
    {
        await _mediator.Send(new ApproveApplicationCommand(id, User.Identity!.Name!));
        TempData["Success"] = "Phê duyệt thành công.";
        return RedirectToAction(nameof(Index));  // PRG Pattern
    }
}
```

#### Session & Cookie Management (MVC)
```csharp
// Program.cs
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
app.UseSession();
```

### 10. Caching Strategy (.NET)

#### In-Memory Cache (single server)
```csharp
builder.Services.AddMemoryCache();

// Sử dụng trong Service
public class ServicePackageService
{
    private readonly IMemoryCache _cache;
    private const string CacheKey = "service_packages";
    
    public async Task<IEnumerable<ServicePackageDto>> GetAllAsync()
    {
        if (_cache.TryGetValue(CacheKey, out IEnumerable<ServicePackageDto>? cached))
            return cached!;
        
        var packages = await _repo.GetAllAsync();
        var dtos = packages.Select(p => new ServicePackageDto(p));
        
        _cache.Set(CacheKey, dtos, TimeSpan.FromMinutes(10));
        return dtos;
    }
}
```

#### Distributed Cache (multi-server / Redis)
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = config.GetConnectionString("Redis"));
// Hoặc SQL Server distributed cache:
builder.Services.AddSqlServerCache(options => { ... });
```

#### Output Cache (ASP.NET Core 7+)
```csharp
builder.Services.AddOutputCache();
app.UseOutputCache();

// Trên action:
[OutputCache(Duration = 60, VaryByQueryKeys = new[] { "page" })]
public async Task<IActionResult> Index(int page = 1) { ... }
```

### 11. API Versioning

```csharp
// Cài đặt: Microsoft.AspNetCore.Mvc.Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),       // /api/v1/
        new HeaderApiVersionReader("x-api-version")
    );
});

// Controller
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/membership")]
public class MembershipV1Controller : ControllerBase { ... }

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/membership")]
public class MembershipV2Controller : ControllerBase { ... }
```
