---
name: develop-feature
description: Quy trình phát triển tính năng end-to-end theo mô hình Agile 6 phase — từ khám phá thị trường đến deploy & quan sát. Stack: ASP.NET Core API + MVC + SQL Server. Dùng khi bắt đầu phát triển một tính năng mới từ đầu.
---

# Quy trình Phát triển Tính năng (Feature Development Workflow)

Workflow này hướng dẫn phát triển một tính năng sản phẩm từ giai đoạn khám phá đến khi phát hành, sử dụng đội ngũ các Agent chuyên biệt phối hợp theo mô hình cross-functional.

**Stack kỹ thuật**: ASP.NET Core 8 API · MVC Razor Views · SQL Server · EF Core · Hangfire · Bootstrap 5

---

## Phase 1: Discovery & Strategy (Khám phá & Chiến lược)

**Mục tiêu**: Hiểu thị trường, định nghĩa vấn đề và thống nhất giá trị kinh doanh.

1. **Nghiên cứu thị trường**: [Senior Market Research Specialist](../skills/senior-market-research-specialist/SKILL.md) thực hiện phân tích đối thủ và xác định khoảng trống thị trường.
2. **Chiến lược SEO**: [Senior SEO Specialist](../skills/senior-seo-specialist/SKILL.md) nghiên cứu từ khóa và xác định search intent.
3. **Định nghĩa sản phẩm**: [Senior Product Owner](../skills/senior-po/SKILL.md) viết PRD, định nghĩa User Stories và xác định success metrics.
4. **Đặc tả chức năng**: [Senior Business Analyst](../skills/senior-ba/SKILL.md) chi tiết hóa logic nghiệp vụ và tạo SRS theo [SRS Template](../SRS_Template.md).
5. **Kế hoạch dữ liệu**: [Senior Data Analyst](../skills/senior-data-analyst/SKILL.md) định nghĩa tracking plans và yêu cầu dashboard.

---

## Phase 2: Architecture & Design (Kiến trúc & Thiết kế)

**Mục tiêu**: Thiết kế kiến trúc kỹ thuật, database schema và UX trước khi code.

1. **Thiết kế kiến trúc hệ thống**: [Senior Architect .NET](../skills/senior-architect-dotnet/SKILL.md) quyết định Clean Architecture layers, CQRS Commands/Queries, MediatR handlers và API contracts cần tạo mới.
2. **Thiết kế Database Schema**: [Senior EF Core](../skills/senior-efcore/SKILL.md) phối hợp cùng [Senior Backend](../skills/senior-backend/SKILL.md) thiết kế entity models, relationships, migration strategy; xuất sơ đồ ERD (Mermaid).
3. **Thiết kế UX**: [Senior UI/UX Designer](../skills/senior-uiux/SKILL.md) tạo user flows (Mermaid) và mockup Razor Views chi tiết theo [Design System](design-system.md).
4. **Thiết kế nội dung**: [Senior Content Writer](../skills/senior-content-writer/SKILL.md) soạn copy phù hợp brand voice và yêu cầu SEO.

> **Checkpoint**: Senior Architect .NET phải duyệt ERD và API contract trước khi Phase 3 bắt đầu.

---

## Phase 3: Implementation (Triển khai)

**Mục tiêu**: Xây dựng tính năng theo đúng kiến trúc đã duyệt. Thứ tự thực thi quan trọng.

### 3.1 Infrastructure Layer (Song song)
- **Database Migration**: [Senior EF Core](../skills/senior-efcore/SKILL.md) tạo EF Core Entities, DbContext configuration, tạo và review migration script trước khi apply.
- **Integration Services**: [Senior Integration](../skills/senior-integration/SKILL.md) chuẩn bị các service tích hợp nếu tính năng cần: Email template mới, Zalo notification template, Hangfire job mới.

### 3.2 Application & API Layer
- **Backend Development**: [Senior Backend](../skills/senior-backend/SKILL.md) xây dựng MediatR Commands/Queries, Validators (FluentValidation), API Controllers và business logic theo Clean Architecture.

### 3.3 Presentation Layer
- **Frontend Development**: [Senior Frontend](../skills/senior-frontend/SKILL.md) xây dựng Razor Views, Partial Views, Tag Helpers (ASP.NET MVC + Bootstrap) và tích hợp với Backend API. Tuân thủ PRG pattern, CSRF protection.

### 3.4 Observability
- **Logging & Health**: [Senior Logging](../skills/senior-logging/SKILL.md) thêm structured log points (Serilog) tại các bước nghiệp vụ quan trọng; cập nhật Health Check nếu tính năng phụ thuộc service ngoài.

---

## Phase 4: Validation & Quality Control (Kiểm định & Đảm bảo chất lượng)

**Mục tiêu**: Đảm bảo tính năng không có lỗi, hiệu năng tốt và đáp ứng tất cả acceptance criteria.

1. **Kiểm thử**: [Senior QC](../skills/senior-qc/SKILL.md) thực hiện chiến lược kiểm thử:
   - Unit tests + Integration tests (WebApplicationFactory + xUnit).
   - API contract tests (Postman/Newman).
   - MVC-specific: CSRF protection, PRG pattern, Authorization.
   - Performance test với k6 nếu tính năng có traffic cao.
2. **Security review**: [Senior Backend](../skills/senior-backend/SKILL.md) chạy `dotnet list package --vulnerable`; kiểm tra OWASP Top 10 theo security checklist.
3. **UAT & Launch**: [Senior Product Owner](../skills/senior-po/SKILL.md) thực hiện User Acceptance Testing và phê duyệt go-live.

---

## Phase 5: Deploy (Triển khai lên môi trường)

**Mục tiêu**: Deploy an toàn, có kiểm soát, không downtime.

1. **CI/CD Pipeline**: [Senior DevOps .NET](../skills/senior-devops-dotnet/SKILL.md) đảm bảo GitHub Actions pipeline pass (`dotnet build`, `dotnet test`, migration check).
2. **Database Migration**: [Senior EF Core](../skills/senior-efcore/SKILL.md) review và apply migration lên Staging trước, backup Production DB.
3. **Deploy**: [Senior DevOps .NET](../skills/senior-devops-dotnet/SKILL.md) deploy lên IIS / Azure App Service theo [deploy-workflow.md](../workflow/deploy-workflow.md).
4. **Smoke Test post-deploy**: [Senior QC](../skills/senior-qc/SKILL.md) xác nhận critical paths hoạt động trên môi trường Production.

---

## Phase 6: Observe & Iterate (Quan sát & Cải thiện)

**Mục tiêu**: Theo dõi hệ thống và phản ứng nhanh nếu có sự cố.

1. **Theo dõi Logging**: [Senior Logging](../skills/senior-logging/SKILL.md) giám sát Serilog/Application Insights — không có ERROR mới, Hangfire jobs không thất bại bất thường.
2. **Theo dõi metrics**: [Senior Data Analyst](../skills/senior-data-analyst/SKILL.md) giám sát PostHog (page views, funnel, error rate) để xác nhận tính năng đạt mục tiêu.
3. **Rollback nếu cần**: [Senior DevOps .NET](../skills/senior-devops-dotnet/SKILL.md) thực hiện theo rollback plan trong deploy-workflow.

---

## Bảng tóm tắt Agent theo Phase

| Phase | Agent tham gia |
|-------|---------------|
| 1. Discovery | Market Research, SEO, PO, BA, Data Analyst |
| 2. Architecture & Design | **Architect .NET**, **EF Core**, Backend, UX, Content |
| 3. Implementation | **EF Core**, **Integration**, Backend, Frontend, **Logging** |
| 4. Validation | QC, Backend, PO |
| 5. Deploy | **DevOps .NET**, **EF Core**, QC |
| 6. Observe | **Logging**, Data Analyst, **DevOps .NET** |
