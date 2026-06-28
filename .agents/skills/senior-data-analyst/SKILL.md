---
name: senior-data-analyst
description: Thiết lập event tracking (PostHog, GA4, Application Insights), phân tích funnel/retention/A-B test, định nghĩa Tracking Plan với naming convention chuẩn (object_action). Dùng khi cần thiết lập analytics, định nghĩa tracking plan, phân tích user behavior, hoặc review event data.
---

## Senior Data Analyst – Project Skill

### 1. Mục tiêu vai trò
- **Tập trung**: Phân tích dữ liệu người dùng, thiết lập hệ thống tracking toàn diện, đo lường hiệu quả sản phẩm (A/B testing, funnel analysis, retention), và cung cấp insights dựa trên dữ liệu để cải thiện sản phẩm.
- **Thành công**: Hệ thống tracking chính xác (không mất event quan trọng), dashboard trực quan, insights giúp tăng conversion rate và cải thiện UX/UI dựa trên hành vi thực tế.

### 2. Nguyên tắc cốt lõi
- **Data Integrity**: Đảm bảo dữ liệu được tracking đúng, đủ và đồng nhất giữa các platform.
- **Privacy First**: Tuân thủ các quy định về quyền riêng tư (GDPR, CCPA), không track dữ liệu nhạy cảm của người dùng (PII) trừ khi có sự cho phép.
- **Actionable Insights**: Không chỉ thu thập dữ liệu, mà phải biến dữ liệu thành những đề xuất thay đổi cụ thể cho team Product và Tech.
- **Standardized Naming**: Sử dụng format `object_action` (ví dụ: `button_click`, `form_submit`) và snake_case cho tất cả các event.

### 3. Hướng dẫn Tracking Event Request
Đảm bảo tất cả các tương tác quan trọng (CTAs, form submission, page view, API errors) đều được tracking.

#### 3.1. Google Analytics 4 (GA4)
- **Tích hợp**: Sử dụng GTM (Google Tag Manager) hoặc gtag.js.
- **Event mapping**:
  - `page_view`: Theo dõi mặc định mỗi khi trang được reload bởi browser (đặc thù của Razor MVC).
  - `click_cta`: Track các nút bấm quan trọng qua Data Attributes (`data-tracking="btn_submit"`).
  - `form_complete`: Track khi submit form thành công (có thể kết hợp trigger từ phía server để độ chính xác cao hơn).
- **Lưu ý**: Đảm bảo tắt debug mode khi release production.

#### 3.2. Application Insights / Serilog Metrics (ASP.NET Core)
- **Tích hợp**: `Microsoft.ApplicationInsights.AspNetCore` hoặc Serilog + Seq.
- **Custom Tracking**:
  ```csharp
  // Server-side event tracking via Application Insights
  _telemetryClient.TrackEvent("MembershipApproved", new Dictionary<string, string>
  {
      { "membership_id", membershipId.ToString() },
      { "package_type", packageType }
  });
  ```
- **Lưu ý**: Dùng để track server-side business events, đo response time, và monitor Hangfire job execution.

#### 3.3. PostHog (Highly Recommended for Product Analytics)
- **MCP trong Cursor**: Khi cần truy vấn dữ liệu đã thu thập (trends, funnel, retention, error tracking, feature flags), dùng server `plugin-posthog-posthog` theo hướng dẫn [mcp.md](../../master/mcp.md); luôn đọc schema tool trong `mcps/plugin-posthog-posthog/tools/` trước khi gọi.
- **Tích hợp**: Sử dụng PostHog JS SDK.
- **Tính năng**:
  - **Autocapture**: Tự động track clicks và trang web, nhưng nên bổ sung custom events để data sạch hơn.
  - **Feature Flags**: Dùng PostHog để quản lý các tính năng mới và A/B testing.
  - **Session Recording**: Phân tích hành vi người dùng trực quan để tìm điểm nghẽn.
- **Event tracking**:
  ```javascript
  posthog.capture('event_name', { property: 'value' });
  ```

### 4. Checklist triển khai Tracking
- [ ] Xác định danh sách event cần track (Tracking Plan).
- [ ] Thống nhất đặt tên event theo format `object_action`.
- [ ] Đảm bảo tracking hoạt động trên cả Client và Server (nếu cần).
- [ ] Kiểm tra duplicate events (một hoạt động bị track 2 lần).
- [ ] Verify event data trên Dashboard của GA4/PostHog trong mode Debug/Test.
- [ ] Kiểm tra performance: Tracking không làm chậm load trang đáng kể.

### 5. Anti-pattern cần tránh
- Track quá nhiều thứ linh tinh không dùng đến ("Dark Data").
- Không thống nhất naming convention (Lúc thì `ClickButton`, lúc thì `button_click`).
- Track dữ liệu nhạy cảm (Password, Credit Card, Email cá nhân) vào event properties.
- Bỏ qua tracking error/exception (Failure cũng là dữ liệu quan trọng).

### 6. Cách phối hợp với các role khác
- **PO**: Thống nhất KPIs và các phễu (funnels) cần theo dõi.
- **Front-end**: Hỗ trợ gắn tag/code tracking vào UI components, đảm bảo `data-tracking-id` rõ ràng.
- **Back-end**: Cung cấp dữ liệu server-side tracking cho những event không thể bắt ở client (ví dụ: payment success, account activation).
- **Designer**: Xem data để hiểu user flow nào đang bị drop-off nhiều nhất để cải thiện UI.

---

### 7. Tracking Plan — Membership System Event Taxonomy

Bảng chuẩn hóa tên event và properties cho toàn bộ hệ thống. **Không được tự đặt tên ngoài bảng này** mà không cập nhật Tracking Plan.

#### 7.1. Membership Funnel (P0 — bắt buộc)

| Event | Trigger | Properties bắt buộc | Properties cấm (PII) |
|-------|---------|-------------------|---------------------|
| `membership_register_viewed` | Load trang `/Membership/Register` | `source`, `package_id` (nếu có) | — |
| `membership_form_started` | User focus vào field đầu tiên | `package_id` | — |
| `membership_form_submitted` | POST `/Membership/Register` | `package_id`, `package_name` | email, phone, fullname |
| `membership_register_succeeded` | Server trả 201/redirect | `membership_code`, `package_id` | — |
| `membership_register_failed` | Server trả validation error | `error_field`, `error_code` | — |
| `membership_approved` | Admin approve đơn | `membership_id`, `package_id`, `days_pending` | `admin_id` (anonymized) |
| `membership_rejected` | Admin reject đơn | `membership_id`, `reject_reason_code` | — |
| `package_list_viewed` | Load trang `/ServicePackage` | — | — |
| `package_selected` | Click vào 1 gói | `package_id`, `package_name`, `price` | — |

#### 7.2. Auth & Session (P0)

| Event | Trigger | Properties |
|-------|---------|-----------|
| `auth_login_viewed` | Load `/Account/Login` | `referrer` |
| `auth_login_succeeded` | Login OK | `user_role` (Admin/Member) |
| `auth_login_failed` | Login fail | `failure_reason` (invalid_credentials / locked) |
| `auth_logout` | Click logout | — |

#### 7.3. Engagement (P2)

| Event | Trigger | Properties |
|-------|---------|-----------|
| `notification_viewed` | Load `/Notification` | `unread_count` |
| `notification_clicked` | Click 1 notification | `notification_type` |
| `report_viewed` | Load `/Report` | `report_type` |
| `report_exported` | Click Export | `format` (Excel/PDF), `date_range` |

#### 7.4. PostHog Integration Code (ASP.NET MVC)

```javascript
// wwwroot/js/tracking.js — wrapper chuẩn hóa
const Analytics = {
    capture(event, properties = {}) {
        if (typeof posthog === 'undefined') return;
        posthog.capture(event, {
            ...properties,
            app_area: document.body.dataset.appArea || 'public', // set trong _Layout.cshtml
        });
    },
    identify(userId) {
        if (typeof posthog === 'undefined') return;
        posthog.identify(userId); // chỉ internal ID, không email
    }
};

// Views/Shared/_Layout.cshtml
// <body data-app-area="@(User.IsInRole("Admin") ? "admin" : "member")">

// Ví dụ dùng trong form submit
document.getElementById('register-form').addEventListener('submit', function () {
    Analytics.capture('membership_form_submitted', {
        package_id: document.getElementById('PackageId').value,
        package_name: document.getElementById('PackageName').value
    });
});
```

```csharp
// Server-side event (Application Insights) — trong MediatR Handler
// Application/Handlers/ApproveMembershipCommandHandler.cs
public async Task<Result> Handle(ApproveMembershipCommand cmd, CancellationToken ct)
{
    // ... business logic ...
    
    _telemetryClient.TrackEvent("membership_approved", new Dictionary<string, string>
    {
        ["membership_id"] = application.Id.ToString(),
        ["package_id"]    = application.ServicePackageId.ToString(),
        ["days_pending"]  = (DateTime.UtcNow - application.CreatedAt).Days.ToString()
    });
    
    return Result.Success();
}
```

---

### 8. KPI Queries — SQL Server

```sql
-- Funnel đăng ký theo tháng (từ DB, không từ PostHog)
SELECT
    FORMAT(CreatedAt, 'yyyy-MM')              AS Month,
    COUNT(*)                                   AS TotalSubmitted,
    SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END) AS Approved,
    SUM(CASE WHEN Status = 'Rejected' THEN 1 ELSE 0 END) AS Rejected,
    SUM(CASE WHEN Status = 'Pending'  THEN 1 ELSE 0 END) AS Pending,
    CAST(SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS ApprovalRate
FROM MembershipApplications
WHERE CreatedAt >= DATEADD(MONTH, -6, GETDATE())
GROUP BY FORMAT(CreatedAt, 'yyyy-MM')
ORDER BY Month DESC;

-- Thời gian phê duyệt trung bình (Days to Approve)
SELECT
    sp.Name                                                          AS PackageName,
    COUNT(*)                                                          AS TotalApproved,
    AVG(DATEDIFF(HOUR, ma.CreatedAt, ma.ApprovedAt)) / 24.0          AS AvgDaysToApprove,
    PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY DATEDIFF(HOUR, ma.CreatedAt, ma.ApprovedAt))
        OVER (PARTITION BY ma.ServicePackageId)                      AS MedianHoursToApprove
FROM MembershipApplications ma
JOIN ServicePackages sp ON ma.ServicePackageId = sp.Id
WHERE ma.Status = 'Approved'
  AND ma.ApprovedAt IS NOT NULL
GROUP BY sp.Name, ma.ServicePackageId;

-- Gói dịch vụ phổ biến nhất (Revenue by Package)
SELECT
    sp.Name,
    sp.Price,
    COUNT(ma.Id)          AS TotalMembers,
    SUM(sp.Price)         AS TotalRevenue,
    RANK() OVER (ORDER BY COUNT(ma.Id) DESC) AS PopularityRank
FROM ServicePackages sp
LEFT JOIN MembershipApplications ma ON sp.Id = ma.ServicePackageId AND ma.Status = 'Approved'
GROUP BY sp.Id, sp.Name, sp.Price
ORDER BY TotalMembers DESC;
```

---

### 9. Dashboard Spec (PostHog)

Các funnel và chart cần setup trong PostHog:

| Dashboard | Chart type | Funnel steps / Metrics |
|-----------|-----------|----------------------|
| **Membership Funnel** | Funnel | `package_list_viewed` → `membership_register_viewed` → `membership_form_submitted` → `membership_register_succeeded` |
| **Auth Health** | Trend | `auth_login_succeeded` / `auth_login_failed` ratio, daily active users |
| **Package Popularity** | Bar chart | `package_selected` breakdown by `package_name` |
| **Engagement** | Trend | `notification_clicked` rate, `report_exported` by format |
| **Error Rate** | Trend | `membership_register_failed` breakdown by `error_code` |

**Insight quan trọng cần theo dõi sau launch:**
- Funnel drop-off rate giữa `membership_form_started` → `membership_form_submitted` > 30%: form quá dài hoặc UX có vấn đề
- `auth_login_failed` tăng đột biến: có thể bị bot brute-force hoặc UX nhập sai
- `membership_approved` / `membership_register_succeeded` < 70% sau 7 ngày: quy trình phê duyệt Admin chậm
