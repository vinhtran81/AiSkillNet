# Implementation Manifest — Skill.Net

Tài liệu tracking toàn bộ feature artifacts và trạng thái planning/implementation.

---

## FEAT-001 — Đăng ký Hội viên mới

**Status:** ✅ Planning complete — Ready for implementation  
**Kickoff date:** 2026-06-27  
**Stack:** ASP.NET Core 8 · MVC Razor · SQL Server · EF Core · MediatR · Hangfire · MailKit · Zalo OA

### Artifacts

| Artifact | File | Status |
|----------|------|--------|
| SRS (Requirements) | [membership-registration_srs.md](specs/membership-registration_srs.md) | ✅ Approved |
| Architecture Blueprint | [membership-registration_architecture.md](specs/membership-registration_architecture.md) | ✅ Approved |
| Database Schema + ERD | [membership-registration_erd.md](specs/membership-registration_erd.md) | ✅ Approved |
| API Spec + Sequence Diagrams | [membership-registration_api_spec.md](specs/membership-registration_api_spec.md) | ✅ Approved |
| UX & Razor View Spec | [membership-registration_ux_spec.md](specs/membership-registration_ux_spec.md) | ✅ Approved |
| Test Plan | [membership-registration_test_plan.md](specs/membership-registration_test_plan.md) | ✅ Approved |
| Logging & Observability Plan | [membership-registration_logging_plan.md](specs/membership-registration_logging_plan.md) | ✅ Approved |

### Implementation Checklist

**Domain Layer**
- [ ] `MembershipApplication` entity với factory method + domain methods (Approve/Reject/Cancel)
- [ ] `ServicePackage` entity
- [ ] `ApplicationStatus` enum
- [ ] 3 Domain Events (Created/Approved/Rejected)
- [ ] `IMembershipApplicationRepository` interface
- [ ] `IServicePackageRepository` interface

**Application Layer**
- [ ] `SubmitMembershipApplicationCommand` + Handler + FluentValidation
- [ ] `ApproveMembershipApplicationCommand` + Handler
- [ ] `RejectMembershipApplicationCommand` + Handler + FluentValidation
- [ ] `CancelMembershipApplicationCommand` + Handler
- [ ] `GetActivePackagesQuery` + Handler + DTO
- [ ] `GetMyApplicationStatusQuery` + Handler + DTO
- [ ] `GetPendingApplicationsQuery` + Handler + DTO (phân trang)
- [ ] `GetApplicationDetailQuery` + Handler + DTO
- [ ] `GetPendingCountQuery` + Handler
- [ ] `INotificationService` + `IFileStorageService` interfaces

**Infrastructure Layer**
- [ ] `MembershipApplicationConfiguration` (EF Fluent API + soft delete filter)
- [ ] `ServicePackageConfiguration`
- [ ] Migration: `AddMembershipRegistrationFeature`
- [ ] Seed: 3 gói dịch vụ (Tiêu chuẩn / Nâng cao / VIP)
- [ ] `MembershipApplicationRepository` impl
- [ ] `ServicePackageRepository` impl
- [ ] `SendApplicationConfirmationEmailJob` Hangfire job
- [ ] `SendApplicationResultNotificationJob` Hangfire job (Email + Zalo)
- [ ] `RemindAdminPendingApplicationsJob` (Recurring daily 8h)
- [ ] Email templates (Confirmation / Approved / Rejected / AdminReminder)
- [ ] Zalo ZNS templates (Approved / Rejected)

**Web Layer**
- [ ] `ServicePackageController` + `Index` view
- [ ] `MembershipController` (Register GET/POST, Status, Cancel)
- [ ] `AdminMembershipController` (Pending, Detail, Approve, Reject)
- [ ] 6 ViewModels
- [ ] 5 Razor Views (ServicePackage/Index, Membership/Register, Membership/Status, Admin/Membership/Pending, Admin/Membership/Detail)
- [ ] `PendingApprovalsBadgeViewComponent`
- [ ] `ServicePackageCardViewComponent`

**Cross-cutting**
- [ ] Health checks: SQL Server + Hangfire + Zalo + SMTP
- [ ] Serilog log points (Handler + Jobs)
- [ ] Application Insights custom events (membership_approved)
- [ ] PostHog tracking events (10 events per taxonomy)
- [ ] robots.txt: `/Admin/` và `/Membership/` → Disallow
- [ ] `noindex` meta tag trên tất cả authenticated views

**Tests**
- [ ] Unit tests: Entity (8 cases), Validator (10 cases), Handler (4 cases)
- [ ] Integration tests: 12 test cases (happy path + edge + security)
- [ ] k6 load test script
- [ ] UAT checklist sign-off (PO)

### Performance Notes (từ Bước 6 Review)

1. **`GetActivePackagesQuery`**: Thêm `IMemoryCache` cache 5 phút — gói ít thay đổi.
2. **`GET /ServicePackage`**: Thêm `[OutputCache(Duration = 300)]`.
3. **Race condition — double Pending**: Thêm unique filtered index:
   ```sql
   CREATE UNIQUE INDEX UX_MembershipApplications_OnePendingPerUser
   ON MembershipApplications (UserId)
   WHERE Status = 'Pending' AND IsDeleted = 0;
   ```
   → Thêm vào `MembershipApplicationConfiguration` và migration.
4. **`GetPendingApplicationsQuery`**: Đảm bảo `.Include(a => a.ServicePackage)` để tránh N+1.

---

*Để bắt đầu implement, chạy `/project:workflow/develop-feature` và tham chiếu các artifacts trên.*
