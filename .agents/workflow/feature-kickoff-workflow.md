---
name: feature-kickoff
description: Feature Kickoff Master Workflow — Điều phối toàn diện từ Ý tưởng thô → Đặc tả SRS → Kiến trúc .NET → DB Schema → API Specs → Sequence Diagram → Test Prep → sẵn sàng coding. Dùng khi cần kickoff một tính năng với đầy đủ artifacts trước khi bắt đầu implement.
---

# 🚀 Quy trình Khởi tạo Tính năng Toàn diện (Feature Kickoff Pipeline)

Bạn là **Tác nhân Điều phối trưởng (Chief Orchestrator)**. Nhiệm vụ: nhận một **[Ý tưởng tính năng thô]** từ người dùng, tự động kích hoạt tuần tự và song song các workflows con để biến ý tưởng đó thành bộ tài liệu kỹ thuật hoàn chỉnh, sẵn sàng cho coding.

**Stack**: ASP.NET Core 8 API · MVC Razor Views · SQL Server · EF Core · Clean Architecture · MediatR

**Quy tắc BẮT BUỘC:**
1. Sử dụng **Output Artifact của bước trước** làm Input Context cho bước sau.
2. TUÂN THỦ 100% quy tắc trong `.agents/master/design-system.md` cho mọi thiết kế giao diện.
3. DỪNG tại mỗi **Checkpoint** — đợi người dùng gõ `Approve` hoặc comment chỉnh sửa trước khi đi tiếp.

---

### Bước 1: Khởi tạo Đặc tả & Nghiệp vụ (Single Source of Truth)

**Agents tham gia:** Senior PO, Senior BA, Senior UX, Senior AI Engineer (nếu có AI)

- **Hành động:** Kích hoạt workflow `/generate-feature-spec`.
- **Mục tiêu:** Bộ ba PM + BA + UX phối hợp sinh ra file Đặc tả Yêu cầu (SRS) chuẩn mực theo [SRS_Template.md](../SRS_Template.md), bao gồm:
  - User Stories + Acceptance Criteria (BDD format).
  - Business rules và edge cases.
  - Non-functional requirements (performance, security, accessibility).
  - Nếu tính năng có AI: [Senior AI Engineer](../skills/senior-ai-engineer/SKILL.md) bổ sung RAG Context, AI Persona, Guardrails.

- **📋 Checkpoint 1:** Hiển thị file `[feature_name]_srs.md`. Đợi `Approve`.

---

### Bước 2: Thiết kế Kiến trúc .NET (Architecture Blueprint)

**Agents tham gia:** Senior Architect .NET *(bắt buộc)*, Principal AI Architect *(nếu cần review)*

- **Đầu vào:** SRS đã duyệt từ Bước 1.
- **Hành động:** [Senior Architect .NET](../skills/senior-architect-dotnet/SKILL.md) phân tích SRS và xuất bản:
  1. **Layer map**: Domain / Application / Infrastructure / Web — xác định class/interface nào cần tạo mới.
  2. **CQRS breakdown**: Danh sách Commands và Queries (MediatR) cho tính năng.
  3. **API contract draft**: Endpoints, HTTP methods, request/response schema (JSON).
  4. **Dependency diagram**: Tính năng này phụ thuộc service/module nào đã có.
  5. **Integration needs**: Có cần Email / Zalo notification / Hangfire job không? → Flag cho Bước 3.

- **📋 Checkpoint 2:** Hiển thị Architecture Blueprint. Đợi `Approve`.

---

### Bước 3: Thiết kế Database & Integration (Data Foundation)

**Agents tham gia:** Senior EF Core *(bắt buộc)*, Senior Integration *(nếu có notification/job)*

- **Đầu vào:** SRS (Bước 1) + Architecture Blueprint (Bước 2).
- **Hành động — Song song:**

  **Luồng 3A — Database Schema:** [Senior EF Core](../skills/senior-efcore/SKILL.md)
  - Thiết kế EF Core Entities, relationships, indexes.
  - Viết Fluent API configuration.
  - Tạo migration script (`dotnet ef migrations add [FeatureName]`).
  - Xuất sơ đồ ERD dạng Mermaid.

  **Luồng 3B — Integration Services:** [Senior Integration](../skills/senior-integration/SKILL.md) *(chỉ nếu Bước 2 có flag)*
  - Email: thiết kế template HTML + plain text fallback.
  - Zalo: xác định message template + OA API endpoint.
  - Hangfire: định nghĩa Job class, queue, retry policy, schedule (nếu recurring).

- **📋 Checkpoint 3:** Hiển thị ERD Mermaid + Integration design. Đợi `Approve`.

---

### Bước 4: Thiết kế Giao tiếp & Giao diện (Interface Design)

**Agents tham gia:** Senior Backend, Senior Frontend, Senior UX — chạy song song

- **Đầu vào:** Architecture Blueprint (Bước 2) + DB Schema (Bước 3).
- **Hành động — Chạy đồng thời 3 luồng:**

  **Luồng 4A — API Specification:** [Senior Backend](../skills/senior-backend/SKILL.md)
  - Chi tiết hóa API contract: validation rules, error codes, response schema.
  - Kích hoạt `/generate-api-spec` → xuất file `[feature]_api_spec.md`.

  **Luồng 4B — Sequence Diagram:** [Senior Backend](../skills/senior-backend/SKILL.md) + [Senior Architect .NET](../skills/senior-architect-dotnet/SKILL.md)
  - Kích hoạt `/generate-sequence-diagram` → vẽ luồng tương tác:
    `Browser → MVC Controller → MediatR Handler → EF Core → SQL Server → Hangfire → Email/Zalo`.

  **Luồng 4C — UX & Razor View Design:** [Senior UI/UX Designer](../skills/senior-uiux/SKILL.md)
  - Tạo mockup Razor Views theo [Design System](design-system.md) (Bootstrap + CSS Variables).
  - Xác định components cần Partial View, form fields, validation messages, TempData alerts.

- **📋 Checkpoint 4:** Tổng hợp API Spec + Sequence Diagram + UI mockup. Đợi `Approve`.

---

### Bước 5: Chuẩn bị Kiểm thử & Observability (QA & Logging Prep)

**Agents tham gia:** Senior QC *(bắt buộc)*, Senior Logging *(bắt buộc)*

- **Đầu vào:** Toàn bộ artifacts từ Bước 1–4.
- **Hành động:**

  **Senior QC** — [senior-qc/SKILL.md](../skills/senior-qc/SKILL.md):
  - Viết Test Plan: Happy Path, Edge Cases, Error Cases, Security Cases (CSRF, Authorization).
  - Tạo Postman Collection cho API endpoints mới.
  - Chuẩn bị Seed Data / Test Fixtures cho WebApplicationFactory.
  - Xác định ngưỡng hiệu năng: response time ≤ 500ms, error rate < 1%.

  **Senior Logging** — [senior-logging/SKILL.md](../skills/senior-logging/SKILL.md):
  - Xác định các log point quan trọng cần thêm: `LogInformation` khi tạo/duyệt/từ chối, `LogWarning` khi retry, `LogError` khi thất bại.
  - Cập nhật Health Check nếu tính năng phụ thuộc service ngoài (Zalo API, Email SMTP).
  - Xác định alert threshold cho Hangfire jobs mới.

- **📋 Checkpoint 5:** Hiển thị Test Plan + Logging Plan. Đợi `Approve`.

---

### Bước 6: Tối ưu hóa Hiệu năng (Optimization Review)

**Agents tham gia:** Senior Architect .NET, Senior Backend, Senior EF Core, Senior BA

- **Đầu vào:** API Spec + DB Schema + Sequence Diagram.
- **Mục tiêu:** Đảm bảo tính năng chạy đúng, nhanh, và ổn định:
  - [Senior EF Core](../skills/senior-efcore/SKILL.md): kiểm tra N+1 query risk, missing index, query plan.
  - [Senior Backend](../skills/senior-backend/SKILL.md): caching strategy (IMemoryCache / OutputCache) nếu data ít thay đổi.
  - [Senior Architect .NET](../skills/senior-architect-dotnet/SKILL.md): xác nhận không có circular dependency, không vi phạm Clean Architecture boundaries.
  - [Senior BA](../skills/senior-ba/SKILL.md): xác nhận logic nghiệp vụ không bị over-engineer hay thiếu sót.

---

### Bước 7: Hợp nhất & Đóng gói (Gather & Wrap-up)

- **Hành động:**
  1. Kiểm tra sự tồn tại của toàn bộ artifacts được sinh ra ở 6 bước trên:
     - `[feature]_srs.md` ✓
     - `[feature]_architecture.md` ✓ *(Layer map, CQRS list)*
     - `[feature]_erd.md` ✓ *(Mermaid ERD)*
     - `[feature]_api_spec.md` ✓
     - `[feature]_sequence.md` ✓
     - `[feature]_test_plan.md` ✓
     - `[feature]_logging_plan.md` ✓
  2. Cập nhật `IMPLEMENTATION_MANIFEST.md` tại thư mục gốc với link toàn bộ artifacts mới.

- **Đầu ra cuối cùng:**
  > 🎉 **Planning & Design hoàn tất 100%.**  
  > Stack: ASP.NET Core API + MVC · Clean Architecture · EF Core · Hangfire  
  > Mọi artifacts đã sẵn sàng — team Dev có thể bắt đầu coding theo [develop-feature.md](../master/develop-feature.md) Phase 3.

---

This command will be available in chat with `/feature-kickoff`
