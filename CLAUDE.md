# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

---

## Project Overview

**Skill.Net** là hệ thống quản lý hội viên (membership management) xây dựng trên **ASP.NET Core 8 + SQL Server**.

Repository hiện tại chứa định nghĩa agent skills và tài liệu dự án. Source code ứng dụng sẽ được tạo trong các folder `src/` và `tests/` theo kiến trúc Clean Architecture.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | ASP.NET Core 8 (C#), Controller + Minimal API |
| Frontend | ASP.NET Core MVC — Razor Views, Tag Helpers, Bootstrap 5 |
| Database | SQL Server + Entity Framework Core + Dapper |
| Architecture | Clean Architecture + CQRS (MediatR) + FluentValidation |
| Background Jobs | Hangfire (SQL Server storage) |
| Email | MailKit (SMTP) |
| Notifications | Zalo OA API |
| Logging | Serilog → Seq / Application Insights |
| Analytics | PostHog (client-side JS) |
| Auth | ASP.NET Core Identity, JWT, Cookie Auth |
| CI/CD | GitHub Actions → IIS / Azure App Service |

---

## Solution Structure (khi tạo source code)

```
Skill.Net/
├── src/
│   ├── SkillNet.Domain/         # Entities, Value Objects, Domain Events, Interfaces
│   ├── SkillNet.Application/    # Commands, Queries (MediatR), Handlers, DTOs, FluentValidation
│   ├── SkillNet.Infrastructure/ # EF Core, Repositories, Email, Zalo, Hangfire
│   └── SkillNet.Web/            # ASP.NET Core MVC Controllers, Razor Views, API Controllers
├── tests/
│   ├── SkillNet.Domain.Tests/
│   ├── SkillNet.Application.Tests/
│   └── SkillNet.Integration.Tests/
├── .agents/                     # Claude Code agent skill definitions
├── .docs/specs/                 # Feature SRS documents (.md)
└── .db/seeds/                   # DB seed / migration scripts
```

### Dependency Rule (bắt buộc)
```
Web → Application → Domain      ✅
Infrastructure → Application    ✅  (implements interfaces)
Domain → Infrastructure         ❌  KHÔNG BAO GIỜ
Application → Infrastructure    ❌  Dùng interface, không dùng concrete class
```

---

## Build & Run Commands

```bash
# Build toàn bộ solution
dotnet build

# Run ứng dụng (từ src/SkillNet.Web)
dotnet run --project src/SkillNet.Web

# Run tất cả tests
dotnet test

# Run test cụ thể (theo class hoặc method)
dotnet test --filter "FullyQualifiedName~MembershipApiTests"
dotnet test --filter "FullyQualifiedName~MembershipApiTests.POST_SubmitApplication_Returns201"

# EF Core Migrations
dotnet ef migrations add <MigrationName> --project src/SkillNet.Infrastructure --startup-project src/SkillNet.Web
dotnet ef migrations script --idempotent --project src/SkillNet.Infrastructure --startup-project src/SkillNet.Web
dotnet ef database update --project src/SkillNet.Infrastructure --startup-project src/SkillNet.Web
```

---

## Agent Skill System

Skills được định nghĩa trong `.agents/skills/*/SKILL.md` và có thể gọi qua Claude Code slash commands.

**Command files nằm tại:** `.claude/commands/` — Claude Code tự động nhận diện.  
**Cú pháp gọi:** `/project:<subpath>/<command-name>` (ví dụ: `/project:skills/senior-backend`)

### Workflow Commands

| Lệnh Claude Code | Mô tả |
|-----------------|-------|
| `/project:workflow/develop-feature` | End-to-end feature workflow: SRS → Architecture → Code → Test → Deploy |
| `/project:workflow/feature-kickoff` | Khởi động feature: SRS → DB Schema → API Specs → Sequence Diagram |
| `/project:workflow/hotfix` | Hotfix pipeline: Triage → Root Cause → Fix → Validation → Post-Mortem |
| `/project:workflow/refactor` | Refactor: Analysis → Implementation → Validation → Sign-off |
| `/project:workflow/security-audit` | Security audit 4 phase |
| `/project:workflow/commit-push` | Commit chuẩn: Lint → Conventional Commit → Rebase → PR prep |
| `/project:workflow/deploy` | Deploy production: Pre-check → Build → Migration → Smoke → Health check |
| `/project:workflow/release-notes` | Viết release notes: Change Collection → Categorization → Writing → Review |

### Skills kỹ thuật .NET

| Lệnh Claude Code | Dùng khi |
|-----------------|---------|
| `/project:skills/senior-backend` | Viết API endpoint, business logic, DB schema |
| `/project:skills/senior-frontend` | Viết Razor View, Partial View, ViewComponent, AJAX |
| `/project:skills/senior-architect-dotnet` | Thiết kế/review Clean Architecture, CQRS |
| `/project:skills/senior-efcore` | EF Core migration, query optimization |
| `/project:skills/senior-integration` | Email (MailKit), Zalo OA, Hangfire jobs |
| `/project:skills/senior-devops-dotnet` | GitHub Actions CI/CD, IIS/Azure deploy |
| `/project:skills/senior-devops-mlops` | AI Quality Gates, LLM Evals pipeline |
| `/project:skills/senior-logging` | Serilog, Health Checks, Exception Handler |
| `/project:skills/senior-qc` | Integration test, k6 load test, Postman |
| `/project:skills/senior-seo` | SEO kỹ thuật ASP.NET MVC — robots.txt, sitemap, Core Web Vitals |

### Skills sản phẩm

| Lệnh Claude Code | Dùng khi |
|-----------------|---------|
| `/project:skills/principal-ai-architect` | Thiết kế AI feature mới (RAG, LLM) |
| `/project:skills/senior-ai-engineer` | Implement AI/RAG với Semantic Kernel |
| `/project:skills/senior-po` | PRD, user story, backlog, UAT, KPI tracking |
| `/project:skills/senior-ba` | SRS theo `.agents/SRS_Template.md` |
| `/project:skills/senior-uiux` | Thiết kế UI, user flow (Mermaid), component states |
| `/project:skills/senior-data-analyst` | PostHog tracking plan, SQL KPI queries, funnel analysis |
| `/project:skills/senior-content-writer` | UX copy, email templates, release notes |
| `/project:skills/senior-market-research` | Competitor analysis, user research, SWOT |
| `/project:skills/deep-research` | Nghiên cứu chuyên sâu — lưu vào `.docs/research/` |

---

## Key Conventions

### API & MVC Pattern
- **API Controllers** (`[ApiController]`) → trả JSON cho AJAX/mobile
- **MVC Controllers** (`: Controller`) → trả View cho server-side render
- Sau mọi form POST thành công → `RedirectToAction` (PRG Pattern)
- Mọi form POST phải có `[ValidateAntiForgeryToken]`

### Code Style
- `async`/`await` xuyên suốt — không dùng `.Result` hoặc `.Wait()`
- Controller không chứa business logic — chỉ gọi `_mediator.Send(command)`
- Entity không truyền thẳng ra View — luôn mapping sang ViewModel/DTO
- EF Core read-only query phải dùng `AsNoTracking()`

### Notification
- Email và Zalo **không được gọi trực tiếp** trong HTTP request handler
- Luôn enqueue qua **Hangfire** → background worker gửi async

### Security
- Không hardcode secret trong code hoặc `appsettings.json`
- Sử dụng **User Secrets** (local) và **Environment Variables / Key Vault** (production)
- Mọi endpoint cần `[Authorize]` trừ khi cố ý public (`[AllowAnonymous]`)

---

## Design System Tokens

CSS Variables định nghĩa trong `wwwroot/css/site.css`:

| Token | Giá trị |
|-------|---------|
| `--color-primary` | `#FF5000` — CTA buttons, links |
| `--color-background` | `#F8F9FA` |
| `--color-surface` | `#FFFFFF` |
| `--color-error` | `#DC2626` |
| `--color-success` | `#16A34A` |
| `--font-family` | `'Inter', sans-serif` |

Spacing theo scale **4px** (Bootstrap gap/padding). Mobile-first, touch target tối thiểu **44px**.

---

## Key Reference Files

| File | Mục đích |
|------|---------|
| `.agents/README.md` | Tổng quan workflows và danh sách agents |
| `.agents/skills/README.md` | Danh sách skill và khi nào dùng |
| `.agents/master/design-system.md` | Design tokens, components, responsive rules |
| `.agents/master/mcp.md` | MCP servers đang dùng (browser, PostHog) |
| `.agents/SRS_Template.md` | Template viết đặc tả yêu cầu |
| `.docs/specs/` | SRS documents cho từng tính năng |
