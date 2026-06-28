---
name: security-audit
description: Security audit workflow focused on ASP.NET Core API security and SQL Server data protection. Dùng khi kiểm tra bảo mật — RLS policies, vulnerability assessment, hardening, và role-based authorization verification.
---

# Security Audit Workflow

> **Lệnh**: `/security-audit`  
> **Mục đích**: Đảm bảo an toàn cho dự án theo chuẩn ASP.NET Core + SQL Server — xác thực, phân quyền, bảo vệ dữ liệu và kiểm tra lỗ hổng bảo mật.

## Phase 1: Policy Review

**Goal**: Kiểm tra Authorization policies và phân quyền theo vai trò.

1. **Authorization Inventory**: [Senior Backend](../skills/senior-backend/SKILL.md) liệt kê tất cả Endpoint, attribute `[Authorize]` / `[AllowAnonymous]`, và Policy mapping theo vai trò (Role/Claim).
2. **Permission Matrix**: Xây dựng bảng CRUD theo vai trò — ai được đọc/viết/xóa tài nguyên nào.
3. **JWT Configuration**: Kiểm tra cấu hình JWT (Issuer, Audience, SigningKey, Expiry, Refresh Token) đúng chuẩn và không hardcode trên Production.

## Phase 2: Vulnerability Assessment

**Goal**: Tìm kiếm lỗ hổng bảo mật ở layer Backend và Frontend.

1. **SQL Injection Prevention**: [Senior Backend](../skills/senior-backend/SKILL.md) — xác nhận toàn bộ truy vấn đều dùng Parameterized Query (EF Core LINQ hoặc Dapper với parameter). Không có raw SQL cộng chuỗi.
2. **API Surface Audit**: Kiểm tra các Endpoint có thể bị tấn công IDOR (Insecure Direct Object Reference), Mass Assignment, hoặc Broken Object Level Authorization.
3. **Frontend Surface**: [Senior Frontend](../skills/senior-frontend/SKILL.md) — XSS prevention (Razor auto-encode, tránh `@Html.Raw()`), CSRF token trên tất cả form POST, không lộ connection string hoặc secret qua View/JS.
4. **Architecture Review**: [Principal AI Architect](../skills/principal-ai-architect/SKILL.md) — rà soát luồng dữ liệu nhạy cảm qua các service, AI/RAG nếu có.
5. **Dependency CVE Scan**: Kiểm tra lỗ hổng trong packages (`dotnet list package --vulnerable`, `npm audit`).

## Phase 3: Hardening

**Goal**: Áp dụng chính sách bảo mật chặt chẽ hơn.

1. **Connection Security**: [Senior Backend](../skills/senior-backend/SKILL.md) — kiểm tra Connection String có `Encrypt=True`, `TrustServerCertificate=False` trên Production.
2. **Least Privilege DB Account**: Tài khoản kết nối DB của ứng dụng chỉ có quyền tối thiểu cần thiết, không dùng `sa` hay `db_owner`.
3. **Security Headers & CORS**: Cấu hình CORS chặt chẽ (chỉ whitelist domain tin cậy), bật HSTS, `X-Content-Type-Options`, `X-Frame-Options`.
4. **Secret Management**: Đảm bảo Connection String, API Keys không lưu trong `appsettings.json` trên Production — sử dụng Azure Key Vault, Environment Variables hoặc User Secrets.
5. **Rate Limiting**: Kích hoạt Rate Limiting middleware (ASP.NET Core 7+) cho các Endpoint đăng nhập, OTP, upload file.

## Phase 4: Verification

**Goal**: Kiểm tra phân quyền và bảo mật theo các kịch bản thực tế.

1. **Role-based Tests**: [Senior QC](../skills/senior-qc/SKILL.md) — test User A không đọc/sửa được data của User B; anonymous user không truy cập được protected endpoint.
2. **SQL Injection Test**: QC thử inject payload vào các field input (form, query string) và xác nhận API trả về lỗi validation, không bị lộ dữ liệu.
3. **Backend Sign-off**: [Senior Backend](../skills/senior-backend/SKILL.md) xác nhận Security Audit Checklist (Section 7 trong [senior-backend/SKILL.md](../skills/senior-backend/SKILL.md)) đã được thực hiện đầy đủ.

---

**Tham khảo thêm**: [Senior Backend Security Checklist](../skills/senior-backend/SKILL.md#7-security-audit-checklist--aspnet-core--sql-server) · [mcp.md](../master/mcp.md)
