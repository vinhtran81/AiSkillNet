---
name: mcp-config
description: Định nghĩa các MCP servers được dùng trong dự án và mapping với vai trò Agent — browser MCP, PostHog MCP, server ID, quy tắc gọi tool. Tài liệu tham chiếu nội bộ cho tất cả agents cần gọi MCP tool.
---

# MCP Configuration

Tài liệu này định nghĩa các Model Context Protocol (MCP) servers được sử dụng trong dự án **ASP.NET Core API + MVC** và ánh xạ chúng với các vai trò của Agents.

> **Stack hiện tại**: ASP.NET Core 8 · SQL Server · MVC Razor Views · Bootstrap 5 · Hangfire · MailKit · Zalo OA API

---

## MCPs đang sử dụng

### 1. Browser MCP
- **Server ID:** `browser`
- **Mục đích:** Tự động hóa trình duyệt để smoke test UI/UX (Razor Views), cào dữ liệu từ web, kiểm tra giao diện trực quan sau deploy.
- **Agent sử dụng:** Senior QC, Senior Frontend, Senior Market Research, Deep Research Agent.
- **Use cases cụ thể:**
  - Kiểm tra form submit MVC hoạt động đúng (POST → Redirect → GET).
  - Smoke test các trang sau deploy lên IIS/Azure.
  - Thu thập screenshot khi bug report UI.
  - Senior Market Research: crawl dữ liệu đối thủ cạnh tranh.

### 2. PostHog MCP
- **Server ID:** `posthog`
- **Mục đích:** Phân tích hành vi người dùng trên ứng dụng MVC, theo dõi phễu chuyển đổi (funnels), session replay, custom events.
- **Agent sử dụng:** Senior Data Analyst, Senior Product Owner.
- **Use cases cụ thể:**
  - Tracking page views, form submission events trên Razor Views.
  - Phân tích funnel: lượt vào trang đăng ký → submit → phê duyệt.
  - Feature flags để A/B test UI components.
  - Session replay để debug UX issues.

---

## MCPs đã loại bỏ (không còn phù hợp với stack)

| MCP | Lý do loại bỏ | Thay thế |
|-----|--------------|----------|
| ~~Vercel MCP~~ | Dự án deploy trên **IIS / Azure App Service**, không dùng Vercel | GitHub Actions CI/CD + Azure Portal / IIS Manager |
| ~~Supabase MCP~~ | Dự án dùng **SQL Server + ASP.NET Core Identity**, không dùng Supabase | EF Core + SQL Server trực tiếp qua `senior-efcore` skill |

---

## Kiến trúc MCP theo tầng (Tech Stack hiện tại)

| Tầng | Công nghệ | MCP / Tool |
|------|-----------|-----------|
| **Backend API** | ASP.NET Core 8 + SQL Server | Không cần MCP — thao tác qua code trực tiếp |
| **Frontend** | Razor Views + Bootstrap 5 | `browser` MCP (smoke test, screenshot) |
| **Auth & Session** | ASP.NET Core Identity + Cookie Auth | Không cần MCP — built-in framework |
| **Background Jobs** | Hangfire + SQL Server | Không cần MCP — Hangfire Dashboard `/hangfire` |
| **Database** | SQL Server + EF Core | Không cần MCP — EF Core migrations + SSMS |
| **Email** | MailKit / SMTP | Không cần MCP — `senior-integration` skill |
| **Zalo Notification** | Zalo OA API (REST) | Không cần MCP — `senior-integration` skill |
| **Analytics** | PostHog (embedded script) | `posthog` MCP |
| **Deploy** | GitHub Actions → IIS / Azure App Service | GitHub Actions workflow trực tiếp |
| **Logging** | Serilog + Seq / Application Insights | Dashboard Seq hoặc Azure Portal |
| **UI Testing** | Browser automation | `browser` MCP |

> **Nguyên tắc:** Mọi Business Logic và Data CRUD đi qua **ASP.NET Core API**. Không có client-side trực tiếp vào DB. MVC Controller xử lý request → gọi Application Layer (MediatR) → Infrastructure (EF Core).

---

## Quy tắc sử dụng MCP

1. Khi một Agent cần gọi tool từ MCP, phải xác định đúng **Server ID** và chỉ dùng cho mục đích đã được phê duyệt.
2. **Đọc schema tool** (`list_tools`, `get_tool_schema`) trước khi gọi bất kỳ tool nào lần đầu.
3. Không truyền PII (họ tên, email, CMND, số điện thoại) vào các tool call nếu không thực sự cần thiết.
4. `browser` MCP chỉ trỏ vào **Staging URL** — không tự động thao tác trên Production.
5. `posthog` MCP chỉ đọc dữ liệu analytics — không dùng để xóa events hoặc thay đổi cấu hình.

---

## Mapping Agent ↔ MCP

| Agent | MCP được phép dùng | Mục đích |
|-------|-------------------|---------|
| Senior QC | `browser` | Smoke test UI sau deploy, screenshot bug |
| Senior Frontend | `browser` | Kiểm tra Razor View render đúng |
| Senior Market Research | `browser` | Crawl dữ liệu đối thủ |
| Deep Research Agent | `browser` | Đọc tài liệu web, benchmark |
| Senior Data Analyst | `posthog` | Phân tích hành vi người dùng, funnel |
| Senior Product Owner | `posthog` | Xem metrics tính năng mới |
| Các agents còn lại | — | Không dùng MCP, tương tác qua code trực tiếp |
