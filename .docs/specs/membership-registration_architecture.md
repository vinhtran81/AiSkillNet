---
type: Architecture Blueprint
feature_id: FEAT-001
status: DRAFT
version: 1.0
created_at: 2026-06-27
depends_on: membership-registration_srs.md
---

# Architecture Blueprint — FEAT-001 Đăng ký Hội viên mới

---

## 1. Layer Map — Clean Architecture

```
SkillNet.Domain
└── Entities/
│   ├── MembershipApplication.cs        ← NEW
│   └── ServicePackage.cs               ← NEW
└── Enums/
│   └── ApplicationStatus.cs            ← NEW  (Pending/Approved/Rejected/Cancelled)
└── Events/
│   ├── MembershipApplicationCreatedEvent.cs   ← NEW
│   ├── MembershipApplicationApprovedEvent.cs  ← NEW
│   └── MembershipApplicationRejectedEvent.cs  ← NEW
└── Interfaces/
    ├── IMembershipApplicationRepository.cs    ← NEW
    └── IServicePackageRepository.cs           ← NEW

SkillNet.Application
└── Features/Membership/
│   ├── Commands/
│   │   ├── Submit/
│   │   │   ├── SubmitMembershipApplicationCommand.cs   ← NEW
│   │   │   ├── SubmitMembershipApplicationHandler.cs   ← NEW
│   │   │   └── SubmitMembershipApplicationValidator.cs ← NEW (FluentValidation)
│   │   ├── Approve/
│   │   │   ├── ApproveMembershipApplicationCommand.cs  ← NEW
│   │   │   └── ApproveMembershipApplicationHandler.cs  ← NEW
│   │   ├── Reject/
│   │   │   ├── RejectMembershipApplicationCommand.cs   ← NEW
│   │   │   ├── RejectMembershipApplicationHandler.cs   ← NEW
│   │   │   └── RejectMembershipApplicationValidator.cs ← NEW
│   │   └── Cancel/
│   │       ├── CancelMembershipApplicationCommand.cs   ← NEW
│   │       └── CancelMembershipApplicationHandler.cs   ← NEW
│   └── Queries/
│       ├── GetMyApplicationStatus/
│       │   ├── GetMyApplicationStatusQuery.cs          ← NEW
│       │   ├── GetMyApplicationStatusHandler.cs        ← NEW
│       │   └── ApplicationStatusDto.cs                 ← NEW
│       ├── GetPendingApplications/
│       │   ├── GetPendingApplicationsQuery.cs          ← NEW
│       │   ├── GetPendingApplicationsHandler.cs        ← NEW
│       │   └── PendingApplicationDto.cs                ← NEW
│       ├── GetApplicationDetail/
│       │   ├── GetApplicationDetailQuery.cs            ← NEW
│       │   ├── GetApplicationDetailHandler.cs          ← NEW
│       │   └── ApplicationDetailDto.cs                 ← NEW
│       └── GetPendingCount/
│           ├── GetPendingCountQuery.cs                 ← NEW
│           └── GetPendingCountHandler.cs               ← NEW
└── Features/ServicePackage/
│   └── Queries/
│       └── GetActivePackages/
│           ├── GetActivePackagesQuery.cs               ← NEW
│           ├── GetActivePackagesHandler.cs             ← NEW
│           └── ServicePackageDto.cs                    ← NEW
└── Common/Interfaces/
    ├── INotificationService.cs     ← NEW (abstraction — impl ở Infrastructure)
    └── IFileStorageService.cs      ← NEW (abstraction — impl ở Infrastructure)

SkillNet.Infrastructure
└── Persistence/
│   ├── Configurations/
│   │   ├── MembershipApplicationConfiguration.cs  ← NEW (EF Fluent API)
│   │   └── ServicePackageConfiguration.cs         ← NEW
│   └── Repositories/
│       ├── MembershipApplicationRepository.cs     ← NEW
│       └── ServicePackageRepository.cs            ← NEW
└── Jobs/                                          ← Hangfire
│   ├── SendApplicationConfirmationEmailJob.cs     ← NEW
│   └── SendApplicationResultNotificationJob.cs    ← NEW
└── Services/
    ├── EmailNotificationService.cs    ← NEW (implements INotificationService)
    ├── ZaloNotificationService.cs     ← NEW (implements INotificationService)
    └── LocalFileStorageService.cs     ← NEW (implements IFileStorageService)

SkillNet.Web
└── Controllers/
│   ├── ServicePackageController.cs   ← NEW  [AllowAnonymous]
│   ├── MembershipController.cs       ← NEW  [Authorize]
│   └── Admin/
│       └── AdminMembershipController.cs  ← NEW  [Authorize(Roles = "Admin")]
└── ViewModels/Membership/
│   ├── ServicePackageListViewModel.cs    ← NEW
│   ├── ServicePackageCardViewModel.cs    ← NEW
│   ├── MembershipRegisterFormModel.cs    ← NEW
│   ├── ApplicationStatusViewModel.cs    ← NEW
│   ├── AdminPendingListViewModel.cs      ← NEW
│   └── AdminApplicationDetailViewModel.cs ← NEW
└── Views/
│   ├── ServicePackage/Index.cshtml       ← NEW
│   ├── Membership/Register.cshtml        ← NEW
│   ├── Membership/Status.cshtml          ← NEW
│   └── Admin/Membership/
│       ├── Pending.cshtml               ← NEW
│       └── Detail.cshtml                ← NEW
└── ViewComponents/
    └── PendingApprovalsBadge/
        ├── PendingApprovalsBadgeViewComponent.cs  ← NEW
        └── Default.cshtml                         ← NEW
```

---

## 2. CQRS Breakdown (MediatR)

### Commands

| Command | Handler action | Triggered by |
|---------|---------------|-------------|
| `SubmitMembershipApplicationCommand` | Validate → check no existing Pending → create Application → enqueue confirmation email job | `MembershipController.Register [POST]` |
| `ApproveMembershipApplicationCommand` | Set Status = Approved, ProcessedAt, ProcessedByAdminId → enqueue result notification job | `AdminMembershipController.Approve [POST]` |
| `RejectMembershipApplicationCommand` | Validate RejectionReason → set Status = Rejected → enqueue result notification job | `AdminMembershipController.Reject [POST]` |
| `CancelMembershipApplicationCommand` | Check ownership + status = Pending → set Status = Cancelled | `MembershipController.Cancel [POST]` |

### Queries

| Query | Return | Used by |
|-------|--------|---------|
| `GetActivePackagesQuery` | `List<ServicePackageDto>` | `ServicePackageController.Index [GET]` |
| `GetMyApplicationStatusQuery(userId)` | `ApplicationStatusDto?` | `MembershipController.Status [GET]` |
| `GetPendingApplicationsQuery(filter, page)` | `PagedResult<PendingApplicationDto>` | `AdminMembershipController.Pending [GET]` |
| `GetApplicationDetailQuery(applicationId)` | `ApplicationDetailDto` | `AdminMembershipController.Detail [GET]` |
| `GetPendingCountQuery` | `int` | `PendingApprovalsBadgeViewComponent` |

---

## 3. API / MVC Contract

### Public Routes (AllowAnonymous)

| Method | Route | Controller Action | Description |
|--------|-------|-------------------|-------------|
| `GET` | `/ServicePackage` | `ServicePackage.Index` | Danh sách gói dịch vụ active |

### Member Routes (Authorize)

| Method | Route | Controller Action | Description |
|--------|-------|-------------------|-------------|
| `GET` | `/Membership/Register` | `Membership.Register` | Form đăng ký (optional: `?packageId={guid}`) |
| `POST` | `/Membership/Register` | `Membership.Register [POST]` | Submit đơn → PRG → Status |
| `GET` | `/Membership/Status` | `Membership.Status` | Trạng thái đơn hiện tại |
| `POST` | `/Membership/Cancel/{id}` | `Membership.Cancel [POST]` | Hủy đơn Pending |

### Admin Routes (Authorize Roles="Admin")

| Method | Route | Controller Action | Description |
|--------|-------|-------------------|-------------|
| `GET` | `/Admin/Membership/Pending` | `AdminMembership.Pending` | Danh sách chờ duyệt (phân trang) |
| `GET` | `/Admin/Membership/Detail/{id}` | `AdminMembership.Detail` | Chi tiết 1 đơn |
| `POST` | `/Admin/Membership/Approve/{id}` | `AdminMembership.Approve [POST]` | Phê duyệt → PRG |
| `POST` | `/Admin/Membership/Reject/{id}` | `AdminMembership.Reject [POST]` | Từ chối → PRG |

### Request/Response Schemas

```csharp
// POST /Membership/Register — Form Model (MVC Model Binding)
public class MembershipRegisterFormModel
{
    [Required] [MaxLength(100)] public string FullName { get; set; }
    [Required] public DateOnly DateOfBirth { get; set; }       // validate: age >= 16
    [Required] [RegularExpression(@"^0\d{9}$")] public string PhoneNumber { get; set; }
    [Required] [MaxLength(300)] public string Address { get; set; }
    [Required] public Guid ServicePackageId { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
    public IFormFile? IdDocumentFile { get; set; }             // optional, max 5MB
}

// POST /Admin/Membership/Reject/{id} — Form Model
public class RejectApplicationFormModel
{
    [Required] [MaxLength(1000)] public string RejectionReason { get; set; }
}

// ApplicationStatusDto (View Model source)
public record ApplicationStatusDto(
    Guid ApplicationId,
    string MembershipCode,
    ApplicationStatus Status,
    string ServicePackageName,
    decimal ServicePackagePrice,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    string? RejectionReason
);
```

---

## 4. Dependency Diagram

```
FEAT-001 phụ thuộc vào:
├── ASP.NET Core Identity          ← Auth, User management (đã có)
├── SkillNet.Domain entities       ← MembershipApplication, ServicePackage (MỚI)
├── MediatR                        ← CQRS dispatch (đã có)
├── FluentValidation               ← Input validation (đã có)
├── EF Core                        ← Persistence (đã có)
│   └── Migration: AddMembershipApplications   ← MỚI
├── Hangfire                       ← Async notification jobs (đã có hoặc cần setup)
├── MailKit                        ← Email gửi (đã có hoặc cần setup)
└── Zalo OA API                    ← Zalo notification (đã có hoặc cần setup)

FEAT-001 KHÔNG phụ thuộc vào:
└── FEAT-002 (Gia hạn hội viên)    ← Tính năng tương lai, scope tách biệt
```

---

## 5. Integration Flags (→ Input cho Bước 3B)

| Integration | Khi nào gọi | Job class | Queue |
|-------------|------------|-----------|-------|
| **Email** | Sau khi submit thành công | `SendApplicationConfirmationEmailJob` | `default` |
| **Email** | Sau khi Admin approve/reject | `SendApplicationResultNotificationJob` | `notifications` |
| **Zalo OA** | Sau khi Admin approve/reject | `SendApplicationResultNotificationJob` | `notifications` |
| **Hangfire** | Schedule: nhắc Admin nếu đơn > 3 ngày chưa xử lý | `RemindAdminPendingApplicationsJob` | `scheduled` |

> ⚠️ **Flag cho Senior Integration**: Cần thiết kế 3 job classes + 2 email templates + 1 Zalo message template.

---

## 6. Clean Architecture Compliance Check

| Rule | Status | Ghi chú |
|------|--------|---------|
| Domain → Infrastructure | ✅ Không vi phạm | Domain chỉ chứa Entity, Event, Interface |
| Application → Infrastructure | ✅ Không vi phạm | Application dùng `INotificationService`, không dùng concrete `MailKit` |
| Web → Application | ✅ | Controller chỉ gọi `_mediator.Send()` |
| Entity ra View | ✅ Không vi phạm | Controller map Entity → ViewModel trước khi truyền View |
| Business logic trong Controller | ✅ | Logic nằm trong MediatR Handler |
| `[ValidateAntiForgeryToken]` | ✅ | Tất cả POST actions |
| `AsNoTracking()` | ✅ | Tất cả Query handlers (read-only) |
