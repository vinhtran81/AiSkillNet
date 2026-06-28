---
type: UX & Razor View Specification
feature_id: FEAT-001
status: DRAFT
version: 1.0
created_at: 2026-06-27
design_tokens: .agents/master/design-system.md
---

# UX & Razor View Specification — FEAT-001

> Tất cả màu sắc, spacing, typography theo `.agents/master/design-system.md`.
> Primary: `#FF5000` · Background: `#F8F9FA` · Font: Inter · Spacing scale: 4px

---

## View 1 — `/ServicePackage` (Package Listing)

### Layout

```
┌─────────────────────────────────────────────────────┐
│  [Header / Navbar — _Layout.cshtml]                  │
├─────────────────────────────────────────────────────┤
│                                                      │
│  <h1> Chọn gói hội viên phù hợp với bạn </h1>       │
│  <p class="lead text-muted"> Tham gia cộng đồng...   │
│                                                      │
│  ┌──────────┐  ┌────────────────┐  ┌──────────┐    │
│  │ Tiêu     │  │  Nâng cao      │  │  VIP     │    │
│  │ chuẩn    │  │ [Phổ biến nhất]│  │          │    │
│  │          │  │                │  │          │    │
│  │ 500.000đ │  │ 1.200.000đ     │  │3.000.000đ│    │
│  │ /năm     │  │ /năm           │  │ /năm     │    │
│  │          │  │                │  │          │    │
│  │ ✓ Quyền 1│  │ ✓ Tất cả QL TC │  │ ✓ Tất cả │    │
│  │ ✓ Quyền 2│  │ ✓ Ưu tiên KH   │  │ ✓ Badge  │    │
│  │          │  │ ✓ Mentoring/Q  │  │ ✓ Support│    │
│  │          │  │                │  │ 24/7     │    │
│  │[Đăng ký] │  │  [Đăng ký]     │  │[Đăng ký] │    │
│  └──────────┘  └────────────────┘  └──────────┘    │
│                                                      │
│  (Mobile: 1 column stack)                            │
└─────────────────────────────────────────────────────┘
```

### Razor View Spec

```html
<!-- Views/ServicePackage/Index.cshtml -->
@model ServicePackageListViewModel
@{
    ViewData["Title"] = "Gói dịch vụ hội viên";
    ViewData["MetaDescription"] = "Chọn gói hội viên phù hợp — Tiêu chuẩn, Nâng cao hoặc VIP.";
}

<main class="container py-5">
    <div class="text-center mb-5">
        <h1 class="fw-bold">Chọn gói hội viên phù hợp với bạn</h1>
        <p class="lead text-muted">Tham gia cộng đồng QMV và hưởng đặc quyền hội viên ngay hôm nay.</p>
    </div>

    @if (!Model.Packages.Any())
    {
        <!-- Empty State -->
        <div class="text-center py-5 text-muted">
            <i class="bi bi-box-seam fs-1 d-block mb-2 opacity-50"></i>
            <p>Chưa có gói dịch vụ nào. Vui lòng quay lại sau.</p>
        </div>
    }
    else
    {
        <div class="row g-4 justify-content-center">
            @foreach (var pkg in Model.Packages)
            {
                <div class="col-12 col-md-6 col-lg-4">
                    @await Component.InvokeAsync("ServicePackageCard", new { package = pkg })
                </div>
            }
        </div>
    }
</main>
```

### ServicePackageCard ViewComponent

```html
<!-- Views/Shared/Components/ServicePackageCard/Default.cshtml -->
@model ServicePackageCardViewModel

<div class="card h-100 @(Model.IsPopular ? "border-primary shadow" : "border-0 shadow-sm")"
     style="border-radius: 12px; @(Model.IsPopular ? "border-width: 2px !important;" : "")">
    <div class="card-body p-4 d-flex flex-column">
        @if (Model.IsPopular)
        {
            <span class="badge mb-2 align-self-start"
                  style="background-color: var(--color-warning); color: #000;">
                Phổ biến nhất
            </span>
        }
        <h5 class="fw-bold mb-1">@Model.Name</h5>
        <div class="mb-3">
            <span class="fs-3 fw-bold" style="color: var(--color-primary);">
                @Model.Price.ToString("N0")đ
            </span>
            <span class="text-muted">/@Model.DurationMonths tháng</span>
        </div>
        <ul class="list-unstyled mb-4 flex-grow-1">
            @foreach (var benefit in Model.Benefits)
            {
                <li class="mb-1">
                    <i class="bi bi-check-circle-fill me-2"
                       style="color: var(--color-success);"></i>@benefit
                </li>
            }
        </ul>
        <a asp-controller="Membership" asp-action="Register"
           asp-route-packageId="@Model.Id"
           class="btn btn-primary w-100 @(Model.IsPopular ? "" : "btn-outline-primary")"
           style="min-height: 44px;">
            Đăng ký ngay
        </a>
    </div>
</div>
```

---

## View 2 — `/Membership/Register` (Registration Form)

### Layout

```
┌─────────────────────────────────────────────────────┐
│  [Header — Authenticated]                            │
├─────────────────────────────────────────────────────┤
│                                                      │
│  ← Quay lại gói dịch vụ                             │
│                                                      │
│  Đăng ký hội viên                                   │
│  ─────────────────                                   │
│  Gói đã chọn: [Nâng cao — 1.200.000đ/năm]  [Đổi]  │
│                                                      │
│  Họ và tên *                                         │
│  [________________________]                          │
│                                                      │
│  Ngày sinh *          Số điện thoại *                │
│  [__/__/____]         [______________]               │
│                                                      │
│  Địa chỉ *                                           │
│  [________________________]                          │
│  [________________________]                          │
│                                                      │
│  Ghi chú (tùy chọn)                                  │
│  [________________________]                          │
│                                                      │
│  CMND/CCCD (tùy chọn, JPG/PNG/PDF, tối đa 5MB)      │
│  [  Chọn file  ] hoặc kéo thả vào đây               │
│                                                      │
│  ─────────────────────────────────────               │
│  [      Nộp đơn đăng ký      ]  ← btn-primary full  │
│                                                      │
└─────────────────────────────────────────────────────┘
```

### Razor View Spec

```html
<!-- Views/Membership/Register.cshtml -->
@model MembershipRegisterFormModel
@{
    ViewData["Title"] = "Đăng ký hội viên";
    ViewData["NoIndex"] = true;   // Authenticated page — no SEO index
}

<div class="container py-4" style="max-width: 640px;">
    <a asp-controller="ServicePackage" asp-action="Index"
       class="text-decoration-none text-muted mb-3 d-inline-flex align-items-center gap-1">
        <i class="bi bi-arrow-left"></i> Quay lại gói dịch vụ
    </a>

    <h2 class="fw-bold mb-4">Đăng ký hội viên</h2>

    @* Validation Summary — server errors *@
    <div asp-validation-summary="ModelOnly" class="alert alert-danger" role="alert"></div>

    <form asp-action="Register" method="post" enctype="multipart/form-data">
        @Html.AntiForgeryToken()

        @* Gói đã chọn (read-only display + hidden field) *@
        <div class="alert d-flex align-items-center gap-3 mb-4"
             style="background: var(--color-primary-light); border: 1px solid var(--color-primary);">
            <i class="bi bi-box-seam fs-4" style="color: var(--color-primary);"></i>
            <div class="flex-grow-1">
                <strong>@Model.SelectedPackageName</strong>
                <span class="text-muted ms-2">— @Model.SelectedPackagePrice.ToString("N0")đ/năm</span>
            </div>
            <a asp-controller="ServicePackage" asp-action="Index"
               class="btn btn-sm btn-outline-secondary">Đổi gói</a>
        </div>
        <input asp-for="ServicePackageId" type="hidden" />

        @* Họ và tên *@
        <div class="mb-3">
            <label asp-for="FullName" class="form-label fw-medium"></label>
            <input asp-for="FullName" class="form-control" placeholder="Nguyễn Văn A"
                   autocomplete="name" />
            <span asp-validation-for="FullName" class="text-danger small"></span>
        </div>

        @* Ngày sinh + SĐT — 2 cột *@
        <div class="row g-3 mb-3">
            <div class="col-12 col-sm-6">
                <label asp-for="DateOfBirth" class="form-label fw-medium"></label>
                <input asp-for="DateOfBirth" type="date" class="form-control" />
                <span asp-validation-for="DateOfBirth" class="text-danger small"></span>
            </div>
            <div class="col-12 col-sm-6">
                <label asp-for="PhoneNumber" class="form-label fw-medium"></label>
                <input asp-for="PhoneNumber" class="form-control" placeholder="0901234567"
                       inputmode="tel" autocomplete="tel" />
                <span asp-validation-for="PhoneNumber" class="text-danger small"></span>
            </div>
        </div>

        @* Địa chỉ *@
        <div class="mb-3">
            <label asp-for="Address" class="form-label fw-medium"></label>
            <textarea asp-for="Address" class="form-control" rows="2"
                      placeholder="Số nhà, đường, phường/xã, quận/huyện, tỉnh/thành phố"></textarea>
            <span asp-validation-for="Address" class="text-danger small"></span>
        </div>

        @* Ghi chú *@
        <div class="mb-3">
            <label asp-for="Notes" class="form-label fw-medium">
                Ghi chú <span class="text-muted fw-normal">(tùy chọn)</span>
            </label>
            <textarea asp-for="Notes" class="form-control" rows="2"
                      placeholder="Thông tin thêm bạn muốn ban quản lý biết..."></textarea>
            <span asp-validation-for="Notes" class="text-danger small"></span>
        </div>

        @* File upload *@
        <div class="mb-4">
            <label asp-for="IdDocumentFile" class="form-label fw-medium">
                CMND/CCCD <span class="text-muted fw-normal">(tùy chọn, JPG/PNG/PDF, tối đa 5MB)</span>
            </label>
            <input asp-for="IdDocumentFile" type="file" class="form-control"
                   accept=".jpg,.jpeg,.png,.pdf" />
            <span asp-validation-for="IdDocumentFile" class="text-danger small"></span>
        </div>

        @* Submit *@
        <button type="submit" class="btn btn-primary w-100 py-3 fw-medium"
                id="btn-submit" style="min-height: 52px;">
            <span class="spinner-border spinner-border-sm me-2 d-none" id="spinner-submit"></span>
            Nộp đơn đăng ký
        </button>
    </form>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
    <script>
        // Loading state khi submit
        document.getElementById('btn-submit').closest('form').addEventListener('submit', function () {
            const btn = document.getElementById('btn-submit');
            const spinner = document.getElementById('spinner-submit');
            btn.disabled = true;
            spinner.classList.remove('d-none');
        });
    </script>
}
```

---

## View 3 — `/Membership/Status`

### Các trạng thái cần thiết kế

**State A — Pending:**
```
┌──────────────────────────────────────────┐
│  ⏳ Đơn đăng ký đang chờ xét duyệt      │
│  ─────────────────────────────────────── │
│  Gói: Nâng cao · Nộp ngày: 27/06/2026   │
│  Đang chờ 2 ngày                         │
│                                           │
│  Ban quản lý sẽ phản hồi trong 1–2 ngày. │
│  [  Hủy đơn  ]                           │
└──────────────────────────────────────────┘
```

**State B — Approved:**
```
┌──────────────────────────────────────────┐
│  ✅ Chúc mừng! Bạn đã là Hội viên QMV   │
│  ─────────────────────────────────────── │
│  Mã hội viên: QMV-202606-4521            │
│  Gói: Nâng cao · Hiệu lực từ: 27/06     │
│  Phê duyệt: 28/06/2026                   │
│                                           │
│  [  Xem quyền lợi hội viên  ]            │
└──────────────────────────────────────────┘
```

**State C — Rejected:**
```
┌──────────────────────────────────────────┐
│  ❌ Đơn đăng ký chưa được chấp thuận    │
│  ─────────────────────────────────────── │
│  Lý do: Thông tin CMND không khớp với    │
│  họ tên đã khai báo.                     │
│                                           │
│  [  Nộp đơn mới  ]  [  Liên hệ hỗ trợ  ]│
└──────────────────────────────────────────┘
```

**State D — No application:**
```
┌──────────────────────────────────────────┐
│  📋 Bạn chưa có đơn đăng ký             │
│                                           │
│  [  Đăng ký hội viên ngay  ]             │
└──────────────────────────────────────────┘
```

---

## View 4 — `/Admin/Membership/Pending`

### Layout

```
┌────────────────────────────────────────────────────────┐
│  [Admin Sidebar]  │  Đơn chờ xét duyệt  [Badge: 12]   │
│                   │  ──────────────────────────────    │
│                   │  Filter: [Tất cả gói ▼] [Ngày ▼]  │
│                   │                                     │
│                   │  Họ tên    Gói      Ngày nộp  Chờ  │
│                   │  ─────────────────────────────────  │
│                   │  Nguyễn A  Nâng cao 25/06   2 ngày │
│                   │  Trần B    VIP      22/06   5 ngày⚠│
│                   │  Lê C      TC       27/06   0 ngày │
│                   │                                     │
│                   │  [← Trước] Trang 1/3 [Sau →]       │
└────────────────────────────────────────────────────────┘
```

**UX rules:**
- Highlight đỏ (`text-danger fw-bold`) nếu `DaysPending > 3`.
- Sort mặc định: `DaysPending DESC` (đơn chờ lâu nhất lên trên).
- Click vào dòng → navigate đến Detail page.
- Không có bulk approve/reject ở MVP — từng đơn 1.

---

## View 5 — `/Admin/Membership/Detail/{id}`

### Layout

```
┌──────────────────────────────────────────────────────┐
│  ← Quay lại danh sách                               │
│                                                      │
│  Đơn #FEAT-001-2026-042 · ⏳ Chờ duyệt  5 ngày     │
│  ────────────────────────────────────────────────   │
│  Họ tên:    Nguyễn Văn A                            │
│  Ngày sinh: 15/03/1990 (36 tuổi)                    │
│  SĐT:       0901234567                              │
│  Địa chỉ:   123 Nguyễn Huệ, Q1, TP.HCM             │
│  Gói:       Nâng cao — 1.200.000đ/năm               │
│  Ghi chú:   —                                        │
│  CMND/CCCD: [Xem ảnh →]                             │
│                                                      │
│  ────────────────────────────────────────────────   │
│  [ ✅ Phê duyệt ]      [ ❌ Từ chối ]               │
│                                                      │
│  @* Modal Từ chối *@                                │
│  ┌────────────────────────────────────┐             │
│  │ Lý do từ chối *                    │             │
│  │ [________________________________] │             │
│  │ [________________________________] │             │
│  │               [Hủy] [Xác nhận từ chối] │        │
│  └────────────────────────────────────┘             │
└──────────────────────────────────────────────────────┘
```

### Razor Detail View Spec

```html
<!-- Approve button — form POST trực tiếp -->
<form asp-action="Approve" asp-route-id="@Model.ApplicationId" method="post" class="d-inline">
    @Html.AntiForgeryToken()
    <button type="submit" class="btn btn-success"
            onclick="return confirm('Xác nhận phê duyệt đơn này?')">
        <i class="bi bi-check-circle me-1"></i>Phê duyệt
    </button>
</form>

<!-- Reject button — mở modal -->
<button type="button" class="btn btn-danger ms-2" data-bs-toggle="modal" data-bs-target="#rejectModal">
    <i class="bi bi-x-circle me-1"></i>Từ chối
</button>

<!-- Reject Modal -->
<div class="modal fade" id="rejectModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <form asp-action="Reject" asp-route-id="@Model.ApplicationId" method="post">
                @Html.AntiForgeryToken()
                <div class="modal-header">
                    <h5 class="modal-title">Lý do từ chối</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <label class="form-label fw-medium">Lý do từ chối <span class="text-danger">*</span></label>
                    <textarea name="RejectionReason" class="form-control" rows="4" required
                              maxlength="1000"
                              placeholder="Mô tả rõ lý do để hội viên biết cách nộp lại..."></textarea>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                    <button type="submit" class="btn btn-danger">Xác nhận từ chối</button>
                </div>
            </form>
        </div>
    </div>
</div>
```

---

## Component States Summary

| Component | States |
|-----------|--------|
| ServicePackageCard | Default, Popular (primary border + badge) |
| Register form | Default, Validation error per field, Submitting (button spinner) |
| Status page | No application, Pending, Approved, Rejected |
| Admin pending table | Loading skeleton, Empty, Populated (row highlight nếu > 3 ngày) |
| Approve/Reject buttons | Default, Loading (spinner khi submit), Disabled (sau khi action) |
| File upload | Default, File selected (filename visible), Error (size/type) |
