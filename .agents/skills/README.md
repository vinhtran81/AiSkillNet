# Agent Skills

Danh sách skill chuyên biệt trong `.agents/skills/`. Mỗi skill định nghĩa vai trò, quy trình và công cụ cho Agent. Stack dự án: **ASP.NET Core API + MVC + SQL Server**.

**Liên quan:** [Workflows tổng quan](../README.md) · [MCP configuration](../master/mcp.md) · [SRS Template](../SRS_Template.md)

---

## 🏗️ Skills Kỹ thuật .NET (Core)

| Agent | Folder | Skill | Khi dùng |
| --- | --- | --- | --- |
| Senior Backend | `senior-backend` | [SKILL.md](senior-backend/SKILL.md) | API Controllers, SQL Server, JWT, Security, Caching |
| Senior Frontend | `senior-frontend` | [SKILL.md](senior-frontend/SKILL.md) | Razor Views, Tag Helpers, PRG Pattern, CSRF, Bootstrap |
| Senior Architect .NET | `senior-architect-dotnet` | [SKILL.md](senior-architect-dotnet/SKILL.md) | Clean Architecture, CQRS, MediatR, FluentValidation |
| Senior EF Core | `senior-efcore` | [SKILL.md](senior-efcore/SKILL.md) | EF Core, Migrations, Repository Pattern, Dapper |
| Senior Integration | `senior-integration` | [SKILL.md](senior-integration/SKILL.md) | Email (MailKit), Zalo OA API, Hangfire Background Jobs |
| Senior DevOps .NET | `senior-devops-dotnet` | [SKILL.md](senior-devops-dotnet/SKILL.md) | GitHub Actions CI/CD, IIS/Azure deploy, EF migration |
| Senior Logging | `senior-logging` | [SKILL.md](senior-logging/SKILL.md) | Serilog, Health Checks, Exception Handler, Observability |
| Senior QC | `senior-qc` | [SKILL.md](senior-qc/SKILL.md) | Testing, WebApplicationFactory, xUnit, Postman/Newman, k6 |

---

## 🎯 Skills Sản phẩm & Chiến lược

| Agent | Folder | Skill | Khi dùng |
| --- | --- | --- | --- |
| Principal AI Architect | `principal-ai-architect` | [SKILL.md](principal-ai-architect/SKILL.md) | Spec mới, đánh giá kiến trúc hệ thống, tối ưu/cắt giảm thành phần |
| Senior AI Engineer | `senior-ai-engineer` | [SKILL.md](senior-ai-engineer/SKILL.md) | RAG, agentic workflows, tracing; tích hợp AI features |
| Senior Product Owner | `senior-po` | [SKILL.md](senior-po/SKILL.md) | PRD, user stories, UAT, backlog, quyết định sản phẩm |
| Senior Business Analyst | `senior-ba` | [SKILL.md](senior-ba/SKILL.md) | SRS, logic nghiệp vụ, BDD acceptance criteria |
| Senior Market Research Specialist | `senior-market-research-specialist` | [SKILL.md](senior-market-research-specialist/SKILL.md) | Phân tích thị trường, đối thủ, SWOT |
| Senior SEO Specialist | `senior-seo-specialist` | [SKILL.md](senior-seo-specialist/SKILL.md) | Keyword, search intent, SEO on-page |
| Senior UI/UX Designer | `senior-uiux` | [SKILL.md](senior-uiux/SKILL.md) | User flows, mockups, design system |
| Senior Content Writer | `senior-content-writer` | [SKILL.md](senior-content-writer/SKILL.md) | Copy, brand voice, nội dung marketing |
| Senior Data Analyst | `senior-data-analyst` | [SKILL.md](senior-data-analyst/SKILL.md) | Analytics, tracking plans, dashboard |
| Senior DevOps/MLOps | `senior-devops-mlops` | [SKILL.md](senior-devops-mlops/SKILL.md) | AI evals pipeline, LLM quality gates (nếu có AI features) |
| Deep Research Agent | `deep-research` | [SKILL.md](deep-research/SKILL.md) | Nghiên cứu chuyên sâu: công nghệ, thị trường, đối thủ |

---

## 📖 Hướng dẫn chọn Skill

```
Implement API endpoint?          → senior-backend
Implement Razor View / UI?       → senior-frontend
Thiết kế Clean Architecture?     → senior-architect-dotnet
EF Core, Migration, Query?       → senior-efcore
Email, Zalo, Background Job?     → senior-integration
CI/CD, Deploy, IIS/Azure?        → senior-devops-dotnet
Logging, Health Check?           → senior-logging
Test API, Integration Test?      → senior-qc
Viết SRS / User Story?           → senior-ba
Thiết kế UI/UX, Mockup?          → senior-uiux
```
