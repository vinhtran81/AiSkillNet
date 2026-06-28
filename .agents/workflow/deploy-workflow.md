---
name: deploy
description: Controlled production deployment workflow for ASP.NET Core + SQL Server. Dùng khi triển khai lên production — pre-deployment check, EF migration, build, smoke test, health check, post-deployment monitoring.
---

# Deploy Workflow

> **Lệnh**: `/deploy`  
> **Mục đích**: Triển khai ứng dụng ASP.NET Core lên production an toàn, có kiểm soát, với đầy đủ migration, health check và rollback plan.

## Phase 1: Pre-deployment

**Goal**: Kiểm tra DB migration, env vars và UAT sign-off.

1. **Database & Migrations**: [Senior Backend](../skills/senior-backend/SKILL.md) + [Senior DevOps .NET](../skills/senior-devops-dotnet/SKILL.md)
   - Xác nhận migration đã apply thành công trên **Staging** (không lỗi).
   - Review SQL script idempotent (`dotnet ef migrations script --idempotent`).
   - Backup Production DB trước khi deploy nếu có migration thay đổi schema.
   - Chuẩn bị rollback script nếu migration thất bại.

2. **Environment Variables**: Xác nhận tất cả secrets đã cấu hình đúng trên Production:
   - Connection String SQL Server
   - Email SMTP credentials
   - Zalo OA Access Token
   - Hangfire connection string
   - Xem [senior-devops-dotnet/SKILL.md](../skills/senior-devops-dotnet/SKILL.md#5-secret-management)

3. **Health Check trên Staging**: Gọi `GET /health` trả về 200 trước khi tiến hành.

4. **UAT Sign-off**: [Senior Product Owner](../skills/senior-po/SKILL.md) phê duyệt go-live.

## Phase 2: Build & Verification

**Goal**: Build production và smoke test.

1. **Production Build**: CI/CD pipeline pass toàn bộ:
   - `dotnet build` — không có error/warning blocker.
   - `dotnet test` — 100% test pass.
   - Check migration: `dotnet ef migrations has-pending-model-changes` = false.

2. **Smoke Test**: [Senior QC](../skills/senior-qc/SKILL.md) — kiểm tra critical paths trên Staging:
   - Đăng nhập / phân quyền.
   - Form đăng ký hội viên submit thành công.
   - Admin phê duyệt / từ chối đơn.
   - Email và Zalo notification gửi được.
   - Hangfire dashboard hiển thị đúng.

## Phase 3: Deployment

**Goal**: Push to production với zero-downtime.

1. **EF Core Migration**: [Senior DevOps .NET](../skills/senior-devops-dotnet/SKILL.md)
   ```bash
   dotnet ef database update \
     --project src/AppName.Infrastructure \
     --startup-project src/AppName.Web
   ```

2. **Deploy Application**:
   - **IIS**: Stop App Pool → swap folder → Start App Pool → Health Check
   - **Azure App Service**: `azure/webapps-deploy` action → Health Check

3. **Version Tag**: Git tag theo Semantic Versioning (`v1.2.0`), push lên remote.

## Phase 4: Post-deployment

**Goal**: Theo dõi lỗi và hiệu năng 15 phút đầu sau launch.

1. **Health Check liên tục**: `GET /health` trả 200 trong suốt 15 phút đầu.

2. **Monitoring**: [Senior Logging](../skills/senior-logging/SKILL.md) + [Senior DevOps .NET](../skills/senior-devops-dotnet/SKILL.md)
   - Kiểm tra Serilog / Application Insights: không có ERROR level mới.
   - Kiểm tra Hangfire: không có failed jobs bất thường.
   - Kiểm tra SQL Server: không có query timeout hoặc deadlock.

3. **Rollback Plan**: Nếu bất kỳ điều kiện nào trigger:
   - Error rate > 5% trong 5 phút.
   - Health check `/health` trả lỗi.
   - Exception level cao hơn baseline pre-deploy.
   
   → Thực hiện: rollback swap folder (IIS) hoặc revert deployment slot (Azure) + chạy `dotnet ef database update [PreviousMigration]`.

---

**Tham khảo**: [senior-devops-dotnet](../skills/senior-devops-dotnet/SKILL.md) · [senior-logging](../skills/senior-logging/SKILL.md) · [senior-backend Deployment Checklist](../skills/senior-backend/SKILL.md#8-deployment-readiness-checklist)
