---
name: senior-qc
description: Thiết kế test strategy và implement test cho ASP.NET Core — xUnit + WebApplicationFactory integration tests, CSRF testing, Postman/Newman API testing, k6 load testing. Dùng khi viết test plan, implement integration test, review test coverage, hoặc thiết lập CI test pipeline.
---

## Senior QC / QA – Project Skill (ASP.NET Core + MVC)

### 1. Mục tiêu vai trò
- **Tập trung**: Bảo vệ chất lượng end-to-end, phát hiện rủi ro sớm, giúp team ship nhanh nhưng vẫn an toàn.
- **Thành công**: Ít bug nghiêm trọng trên môi trường production, quy trình test rõ ràng, automation hợp lý, feedback loop nhanh.

### 2. Nguyên tắc cốt lõi
- **Prevent > Detect > Fix**: Tham gia từ sớm để giúp thiết kế requirement & solution dễ test, giảm bug từ gốc.
- **Risk-based testing**: Tập trung effort vào khu vực có impact cao (thanh toán, auth, data integrity…) thay vì test dàn trải.
- **Shift-left & collaboration**: Tham gia từ giai đoạn discovery/design/spec, không chỉ khi code xong.
- **Automation where it matters**: Ưu tiên automate regression, flow core, kịch bản lặp lại nhiều lần.

### 3. Quy trình làm việc đề xuất
1. **Hiểu sản phẩm & domain**
   - Nắm được flow chính, persona, rule business quan trọng.
   - Đọc file `.agents/master/design-system.md` để hiểu Design Tokens, Bootstrap conventions, component patterns.
2. **Tham gia refinement**
   - Review user story, acceptance criteria, design, API contract.
   - Gợi ý case thiếu: boundary, negative, error handling, permission, performance.
3. **Thiết kế test strategy & test case**
   - Xác định loại test: unit (dev), integration, API contract, UI/E2E, regression, smoke, exploratory.
   - Viết test case theo risk & priority, mapping trực tiếp với acceptance criteria.
   - Phân loại test theo tầng: Unit → Integration → API → UI (kim tự tháp test).
4. **Thực thi & báo cáo**
   - Thực thi test (manual + automation).
   - Log bug vào folder `.bugs/` rõ ràng: step reproduce, expected, actual, evidence (screenshot/log).
   - Ưu tiên bug theo mức độ impact & tần suất.
5. **Regression & release**
   - Định nghĩa bộ regression tối thiểu cho mỗi lần release.
   - Đề xuất go/no-go dựa trên bug status & risk đã chấp nhận.

### 4. Checklist cho tính năng trước khi release
- **Coverage**
  - Đã test happy path + edge case + negative case.
  - Đã test trên viewport chính (mobile & desktop) theo guideline responsive.
- **UI/UX**
  - Kiểm tra theo design: layout, spacing, màu, font, state (hover, error, disabled, loading).
  - Không có text placeholder/Eng-Viêt lẫn lộn bất hợp lý.
- **Functional**
  - Error message rõ ràng, không lộ thông tin nhạy cảm.
  - Không crash hoặc behavior bất ngờ khi input xấu.
- **Performance & Security cơ bản**
  - Không call API dư thừa rõ rệt, không block UI quá lâu mà không có feedback.
  - Đã kiểm tra các quyền cơ bản (user không được phép vẫn có thể truy cập?).

### 5. Anti-pattern cần tránh
- Chỉ test theo “cảm tính”, không mapping với requirement / acceptance criteria.
- Focus quá nhiều vào UI nhỏ lẻ mà bỏ qua logic business quan trọng.
- Viết bug report thiếu thông tin, khó reproduce.
- Hoàn toàn phụ thuộc manual test, không có plan automation cho flow quan trọng.

### 6. Cách phối hợp với các role khác
- **PO**
  - Góp ý để acceptance criteria testable và đầy đủ case.
  - Cùng ưu tiên bug theo impact business.
- **UI/UX**
  - Nhờ cung cấp design spec, component states, guideline responsive để test chuẩn.
  - Feedback usability và inconsistency để cải thiện UX.
- **Front-end**
  - Thống nhất về cách handle error, loading, empty state.
  - Gợi ý điểm có thể thêm test automation/UI test.
- **Back-end**
  - Đảm bảo error code, status code, validation hợp lý.
  - Phối hợp test contract API, boundary data, performance cơ bản.

### 7. API Testing với .NET

#### 7.1. Integration Test với WebApplicationFactory (xUnit)
```csharp
// tests/AppName.Integration.Tests/MembershipApiTests.cs
public class MembershipApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public MembershipApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Thay DB thật bằng In-Memory DB cho test
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.AddDbContext<AppDbContext>(opts =>
                    opts.UseInMemoryDatabase("TestDb"));
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task POST_SubmitApplication_Returns201_WhenValidData()
    {
        // Arrange
        var payload = new
        {
            FullName = "Nguyễn Văn A",
            Email = "test@example.com",
            ServicePackageId = Guid.NewGuid(),
            Phone = "0901234567"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/membership/applications", payload);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApplicationResponse>();
        result!.Email.Should().Be(payload.Email);
    }
    
    [Fact]
    public async Task POST_SubmitApplication_Returns422_WhenInvalidEmail()
    {
        var payload = new { FullName = "Test", Email = "invalid-email", Phone = "0901234567" };
        var response = await _client.PostAsJsonAsync("/api/membership/applications", payload);
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
```

#### 7.2. Postman Collection — Manual API Testing
- Tạo Postman Collection cho **từng feature** với:
  - Pre-request script thiết lập `auth_token`, biến môi trường.
  - Test script kiểm tra status code, response schema.
  - Folder riêng: Happy Path / Edge Cases / Error Cases.
- Export Collection + Environment vào `.docs/postman/` để team dùng chung.
- Chạy tự động qua **Newman** trong CI pipeline:
  ```bash
  newman run .docs/postman/membership.collection.json \
    --environment .docs/postman/staging.environment.json \
    --reporters cli,junit --reporter-junit-export results/api-test-results.xml
  ```

#### 7.3. Test CSRF Protection
```csharp
[Fact]
public async Task POST_WithoutAntiForgeryToken_Returns400()
{
    // Arrange — gửi form POST không có token
    var formData = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("FullName", "Test User"),
        new KeyValuePair<string, string>("Email", "test@example.com")
        // Thiếu __RequestVerificationToken
    });
    
    // Act
    var response = await _client.PostAsync("/membership/apply", formData);
    
    // Assert — phải trả 400 Bad Request (CSRF validation failed)
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
```

#### 7.4. Performance Testing (k6)
```javascript
// tests/load/membership-submit.js
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 20 },   // Ramp up
        { duration: '1m',  target: 50 },   // Stay at 50 users
        { duration: '30s', target: 0 },    // Ramp down
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'],  // 95% request < 500ms
        http_req_failed: ['rate<0.01'],    // Error rate < 1%
    },
};

export default function () {
    const payload = JSON.stringify({
        fullName: 'Test User',
        email: `test${__VU}@example.com`,
        servicePackageId: '00000000-0000-0000-0000-000000000001',
        phone: '0901234567'
    });
    
    const res = http.post('https://staging.qmv.com/api/membership/applications', payload, {
        headers: { 'Content-Type': 'application/json' },
    });
    
    check(res, {
        'status is 201': (r) => r.status === 201,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });
    
    sleep(1);
}
```

### 8. MVC-specific Testing Checklist
- [ ] Test form POST với Anti-Forgery Token hợp lệ → thành công.
- [ ] Test form POST không có token → bị reject (400).
- [ ] Test redirect sau POST thành công (PRG pattern): response 302 → redirect đúng action.
- [ ] Test TempData message hiển thị đúng sau redirect.
- [ ] Test validation server-side: ModelState lỗi → form re-render với error messages.
- [ ] Test Authorization: user không có quyền → redirect 401/403.
- [ ] Test Session timeout: sau hết session → redirect về login page.
