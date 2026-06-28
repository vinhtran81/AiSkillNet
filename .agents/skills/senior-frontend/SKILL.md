---
name: senior-frontend
description: Xây dựng UI với ASP.NET Core MVC — Razor Views, Tag Helpers, ViewComponent, PRG Pattern, CSRF, Bootstrap 5. Dùng khi implement Razor View mới, viết Partial View/ViewComponent, handle form validation, hoặc tích hợp AJAX với backend API.
---

## Senior Front-end – Project Skill (ASP.NET MVC)

### 1. Mục tiêu vai trò
- **Tập trung**: Xây dựng UI/UX chuẩn design trên nền tảng **ASP.NET Core MVC** (Razor Views, Tag Helpers, Controllers), đảm bảo performance tốt, dễ maintain, tích hợp chặt với Web API backend.
- **Thành công**: Razor Views sạch, reusable Partial Views/ViewComponents, ít bug UI/logic, trải nghiệm người dùng mượt mà, DX tốt cho cả team .NET.

### 2. Stack & Công nghệ chính
- **View Engine**: Razor (`.cshtml`), Layout (`_Layout.cshtml`), Partial Views, ViewComponents
- **Server-side**: ASP.NET Core MVC Controllers, Tag Helpers, HTML Helpers, Model Binding
- **Client-side**: Vanilla JavaScript / jQuery / Alpine.js (tuỳ theo quy định nhóm)
- **CSS Framework**: Bootstrap 5 + CSS Custom Properties (xem Design System)
- **HTTP Client**: Fetch API / jQuery AJAX để gọi về .NET Web API
- **Bundling**: LibMan hoặc npm + Webpack/Vite cho asset pipeline, output vào `wwwroot/`
- **State Management**: PRG Pattern (Post-Redirect-Get), TempData, ViewBag/ViewData, Session

### 3. Nguyên tắc cốt lõi
- **Design-system driven**: Luôn tuân thủ `.agents/master/design-system.md` — màu sắc, spacing, typography, component states.
- **Mobile-first & accessible**: Responsive Bootstrap grid, touch target tối thiểu 44px, semantic HTML, ARIA attributes.
- **Separation of concerns**:
  - Razor View: thuần render, không nhét logic business.
  - Logic data: thực hiện tại Controller hoặc Application Service Layer.
  - Client-side interaction: JavaScript file riêng biệt, không inline trong `.cshtml`.
- **PRG Pattern**: Luôn áp dụng Post-Redirect-Get để tránh double-submit và giữ URL sạch.
- **CSRF Protection**: Mọi form POST phải có `@Html.AntiForgeryToken()` và Controller phải có `[ValidateAntiForgeryToken]`.

### 4. Cấu trúc thư mục chuẩn

```
wwwroot/
├── css/
│   ├── site.css          # CSS chính dự án
│   └── {module}.css      # CSS riêng theo module
├── js/
│   ├── site.js           # JS chung toàn app
│   └── {module}.js       # JS riêng theo module/page
└── lib/                  # Third-party libraries (Bootstrap, jQuery…)

Views/
├── Shared/
│   ├── _Layout.cshtml    # Layout chính
│   ├── _ValidationScriptsPartial.cshtml
│   ├── _Notification.cshtml  # Toast/Alert partial
│   └── Error.cshtml
├── {ControllerName}/
│   ├── Index.cshtml
│   ├── Create.cshtml
│   ├── Edit.cshtml
│   └── _PartialCard.cshtml  # Prefix _ cho Partial Views
└── Components/           # ViewComponents
    └── {ComponentName}/
        └── Default.cshtml
```

### 5. Quy trình làm việc đề xuất

1. **Hiểu requirement & thiết kế**
   - Đọc user story, acceptance criteria, design mockup.
   - Xác định ViewModel cần thiết, luồng dữ liệu Controller → View.
   - Xác định Partial Views, ViewComponents có thể tái sử dụng.

2. **Thiết kế ViewModel**
   - Mỗi View có ViewModel riêng: `{Feature}ViewModel.cs`, `{Feature}FormModel.cs`.
   - Không truyền Entity trực tiếp ra View — luôn mapping sang ViewModel.
   - Dùng **DataAnnotations** trên ViewModel cho client-side + server-side validation.

3. **Implement Razor Views**
   - Khai báo `@model` rõ ràng ở đầu mỗi View.
   - Dùng **Tag Helpers** thay cho HTML Helpers cũ: `asp-for`, `asp-action`, `asp-controller`, `asp-validation-for`.
   - Sử dụng `@section Scripts { ... }` để inject JS đặc thù của từng page vào layout.
   - Tách Partial View khi: (a) block HTML lặp lại ≥ 2 lần, (b) component phức tạp có state riêng.

4. **Kết nối dữ liệu**
   - **Server-side (Form POST)**: Model Binding tự động, validate `ModelState.IsValid`.
   - **Client-side AJAX**: Gọi về `/api/...` hoặc action trả `JsonResult` để cập nhật UI không reload trang.
   - **Phân trang**: Dùng `PagedList` hoặc tự implement, truyền `pageNumber`/`pageSize` qua query string.

5. **Testing & refinement**
   - Kiểm tra UI trên mobile (≤768px) & desktop, các state: loading, empty, error.
   - Test form validation: client-side (jQuery Validation) và server-side.
   - Test CSRF với `[ValidateAntiForgeryToken]`.
   - Kiểm tra hiển thị đúng trên IE Edge, Chrome, Firefox.

### 6. Checklist code chất lượng

- **Structure**
  - View có khai báo `@model` đúng kiểu.
  - Partial View tái sử dụng, đặt tên prefix `_`.
  - JavaScript không inline trong `.cshtml` (ngoại lệ: script khởi tạo component nhỏ ≤5 dòng).

- **Forms & Validation**
  - Mọi form có `asp-antiforgery="true"` hoặc `@Html.AntiForgeryToken()`.
  - Validation message hiển thị rõ ràng: `asp-validation-for`, `asp-validation-summary`.
  - Áp dụng PRG: sau POST thành công → `return RedirectToAction(...)`.
  - TempData dùng để truyền thông báo thành công/thất bại qua redirect.

- **UI/UX**
  - Loading state cho AJAX action: spinner, disable button trong lúc call.
  - Empty state cho danh sách trống: hiển thị thông điệp + CTA.
  - Error state: hiển thị thông báo thân thiện, không lộ stack trace.
  - Tuân thủ Design System: màu sắc, spacing, typography, responsive.

- **Security (Frontend)**
  - Không render dữ liệu người dùng trực tiếp mà không encode: dùng `@Html.DisplayFor()` hoặc `@Model.Property` (Razor tự encode).
  - Không dùng `@Html.Raw()` trừ khi dữ liệu đã được sanitize kỹ lưỡng phía server.
  - Content Security Policy headers kiểm tra: không có inline script trái phép.

### 7. Anti-pattern cần tránh
- Truyền **Entity** (từ EF Core) trực tiếp ra View thay vì ViewModel — gây over-exposure dữ liệu và tight coupling.
- Nhét logic nghiệp vụ phức tạp vào View (`.cshtml`) — View chỉ dùng để render.
- Dùng `ViewBag`/`ViewData` quá nhiều thay vì ViewModel typed — khó refactor, mất IntelliSense.
- **Quên CSRF token** trên form POST — lỗ hổng bảo mật nghiêm trọng.
- Hard-code URL trong JavaScript — dùng `@Url.Action()` hoặc `data-*` attribute.
- Gọi API trực tiếp từ View (code block `@{ ... }`) thay vì để Controller xử lý.
- Không có Loading/Empty/Error state cho các section load AJAX.

### 8. Cách phối hợp với các role khác

- **PO**: Làm rõ behavior, edge case, priority UI/UX; góp ý khi requirement quá phức tạp.
- **UI/UX**: Trao đổi sớm về feasibility, component reuse, responsive behavior.
- **Back-end**:
  - Thống nhất API contract sớm (swagger/OpenAPI).
  - Dùng typed `HttpClient` hoặc `IHttpClientFactory` để gọi API từ Controller (không fetch trực tiếp từ View).
  - Thống nhất ViewModel / DTO schema để tránh mapping dư thừa.
- **QC**: Cung cấp URL preview, tài liệu các state đặc biệt, hỗ trợ test AJAX flow.

### 9. Performance Checklist (MVC)
1. **Bundle & Minify**: Đảm bảo CSS/JS production được minify (`ASPNETCORE_ENVIRONMENT=Production`).
2. **Static files Caching**: Cấu hình `Cache-Control` headers cho `wwwroot/` assets.
3. **Partial View**: Dùng `PartialAsync` và cân nhắc Output Caching cho partial tốn kém.
4. **Minimize ViewBag**: Chuyển sang ViewModel typed để tránh boxing/unboxing.
5. **Response Compression**: Bật Gzip/Brotli trong `Startup.cs` / `Program.cs`.
6. **Image Optimization**: Tối ưu kích thước, dùng WebP khi có thể, lazy loading với `loading="lazy"`.
7. **Script loading**: `defer` hoặc `async` cho script không critical. Đặt script cuối `</body>` hoặc trong `@section Scripts`.

### 10. Security Quick Reference (MVC Frontend)

| Vấn đề | Giải pháp |
|--------|-----------|
| CSRF | `@Html.AntiForgeryToken()` + `[ValidateAntiForgeryToken]` |
| XSS | Razor tự encode; tránh `@Html.Raw()` |
| Open Redirect | Validate `returnUrl` với `Url.IsLocalUrl()` |
| Sensitive data in URL | Dùng POST, không GET cho dữ liệu nhạy cảm |
| Session fixation | Gọi `HttpContext.Session.Clear()` khi logout |

---

### 11. ViewComponent — Patterns & Code Examples

ViewComponent dùng khi Partial View cần inject **service từ DI container** hoặc khi có logic phức tạp không nên đặt trong Controller.

#### 11.1. Ví dụ: PendingApprovalsBadge

```csharp
// ViewComponents/PendingApprovalsBadgeViewComponent.cs
public class PendingApprovalsBadgeViewComponent(IMediator mediator) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var count = await mediator.Send(new GetPendingApprovalsCountQuery());
        return View(count);
    }
}

// Views/Shared/Components/PendingApprovalsBadge/Default.cshtml
@model int
@if (Model > 0)
{
    <span class="badge bg-danger rounded-pill">@Model</span>
}
```

```html
<!-- Dùng trong _Layout.cshtml hoặc bất kỳ View nào -->
<a asp-controller="Membership" asp-action="PendingList">
    Chờ phê duyệt
    @await Component.InvokeAsync("PendingApprovalsBadge")
</a>
```

#### 11.2. Ví dụ: ServicePackageCard (reusable card)

```csharp
// ViewComponents/ServicePackageCardViewComponent.cs
public class ServicePackageCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ServicePackageViewModel package, bool showSelectButton = true)
        => View(new ServicePackageCardModel(package, showSelectButton));
}
```

```html
<!-- Views/ServicePackage/Index.cshtml — dùng ViewComponent trong vòng lặp -->
@foreach (var package in Model.Packages)
{
    @await Component.InvokeAsync("ServicePackageCard", new { package, showSelectButton = true })
}
```

---

### 12. AJAX Patterns với Loading State

#### 12.1. Fetch với loading state chuẩn

```javascript
// wwwroot/js/membership.js
async function approveApplication(membershipId) {
    const btn = document.getElementById(`btn-approve-${membershipId}`);
    const originalText = btn.textContent;

    // Loading state
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Đang xử lý...';

    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const response = await fetch(`/Membership/Approve/${membershipId}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const err = await response.json();
            showToast('error', err.message ?? 'Có lỗi xảy ra. Vui lòng thử lại.');
            return;
        }

        const data = await response.json();
        showToast('success', data.message);
        // Reload danh sách hoặc xoá dòng
        document.getElementById(`row-${membershipId}`)?.remove();

    } catch {
        showToast('error', 'Không thể kết nối máy chủ. Kiểm tra mạng và thử lại.');
    } finally {
        btn.disabled = false;
        btn.textContent = originalText;
    }
}
```

#### 12.2. Toast/Notification từ TempData (Server-side)

```csharp
// Controllers — sau PRG redirect, set TempData
TempData["Success"] = "Hội viên đã được phê duyệt thành công.";
// hoặc
TempData["Error"] = "Không thể phê duyệt. Đơn đã bị huỷ.";
return RedirectToAction("PendingList");
```

```html
<!-- Views/Shared/_Notification.cshtml — include trong _Layout.cshtml -->
@if (TempData["Success"] != null)
{
    <div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index:9999">
        <div class="toast align-items-center text-bg-success border-0 show" role="alert">
            <div class="d-flex">
                <div class="toast-body">@TempData["Success"]</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    </div>
}
@if (TempData["Error"] != null)
{
    <div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index:9999">
        <div class="toast align-items-center text-bg-danger border-0 show" role="alert">
            <div class="d-flex">
                <div class="toast-body">@TempData["Error"]</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    </div>
}
```

#### 12.3. Empty State Pattern

```html
<!-- Dùng cho mọi danh sách có thể trống -->
@if (!Model.Items.Any())
{
    <div class="text-center py-5 text-muted">
        <i class="bi bi-inbox fs-1 d-block mb-2"></i>
        <p class="mb-1">Chưa có đơn nào đang chờ duyệt.</p>
        <a asp-action="Index" class="btn btn-sm btn-outline-primary mt-2">Xem tất cả hội viên</a>
    </div>
}
else
{
    @* render danh sách *@
}
