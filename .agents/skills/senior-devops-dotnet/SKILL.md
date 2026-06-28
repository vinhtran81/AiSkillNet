---
name: senior-devops-dotnet
description: Xây dựng CI/CD pipeline với GitHub Actions và deploy ASP.NET Core lên IIS hoặc Azure App Service — tích hợp EF Core migration tự động, secret management, health check, rollback plan. Dùng khi thiết lập GitHub Actions workflow, cấu hình deploy pipeline, hoặc xử lý deployment issue.
---

## Senior DevOps .NET – Project Skill (CI/CD + Deploy)

### 1. Mục tiêu vai trò
- **Tập trung**: Xây dựng và vận hành CI/CD pipeline tự động, an toàn cho ứng dụng **ASP.NET Core** — từ build, test, migration đến deploy lên **IIS** hoặc **Azure App Service**.
- **Thành công**: Mọi PR được kiểm tra tự động (build + test + migrate), deploy production không downtime, rollback nhanh dưới 5 phút.

### 2. Tech Stack CI/CD chuẩn

| Công cụ | Vai trò |
|---------|---------|
| GitHub Actions / Azure DevOps | Pipeline orchestration |
| `dotnet` CLI | Build, test, publish |
| `dotnet ef` CLI | EF Core migrations |
| Docker | Container hóa (optional) |
| IIS / Web Deploy | On-premise deployment |
| Azure App Service | Cloud deployment |
| Azure Key Vault / GitHub Secrets | Secret management |

### 3. Pipeline Chuẩn — GitHub Actions

#### 3.1. CI Pipeline (`.github/workflows/ci.yml`)

```yaml
name: CI — Build & Test

on:
  push:
    branches: [develop, main]
  pull_request:
    branches: [develop, main]

env:
  DOTNET_VERSION: '8.0.x'
  PROJECT_PATH: 'src/AppName.Web/AppName.Web.csproj'
  TEST_PROJECT: 'tests/AppName.Application.Tests/AppName.Application.Tests.csproj'

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore --configuration Release
      
      - name: Run Tests
        run: dotnet test ${{ env.TEST_PROJECT }} --no-build --configuration Release \
          --logger trx --results-directory ./TestResults
      
      - name: Upload Test Results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: test-results
          path: ./TestResults/*.trx

  check-migrations:
    runs-on: ubuntu-latest
    needs: build-and-test
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      - name: Install EF Core tools
        run: dotnet tool install --global dotnet-ef
      - name: Verify migrations up to date
        run: |
          dotnet ef migrations has-pending-model-changes \
            --project src/AppName.Infrastructure \
            --startup-project src/AppName.Web
```

#### 3.2. CD Pipeline — Deploy to Azure App Service

```yaml
name: CD — Deploy Production

on:
  push:
    branches: [main]
  workflow_dispatch:

jobs:
  deploy:
    runs-on: ubuntu-latest
    environment: production
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore & Publish
        run: |
          dotnet restore
          dotnet publish src/AppName.Web/AppName.Web.csproj \
            --configuration Release \
            --output ./publish \
            --no-restore
      
      - name: Run EF Core Migrations
        env:
          ConnectionStrings__DefaultConnection: ${{ secrets.CONNECTION_STRING_PROD }}
        run: |
          dotnet tool install --global dotnet-ef
          dotnet ef database update \
            --project src/AppName.Infrastructure \
            --startup-project src/AppName.Web \
            --no-build \
            --configuration Release
      
      - name: Deploy to Azure App Service
        uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ secrets.AZURE_APP_SERVICE_NAME }}
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ./publish
      
      - name: Health Check sau deploy
        run: |
          sleep 30
          curl -f https://${{ secrets.APP_URL }}/health || exit 1
```

#### 3.3. CD Pipeline — Deploy to IIS (On-premise)

```yaml
name: CD — Deploy IIS

on:
  workflow_dispatch:
    inputs:
      environment:
        description: 'Target environment'
        required: true
        default: 'staging'
        type: choice
        options: [staging, production]

jobs:
  deploy-iis:
    runs-on: [self-hosted, windows, iis]
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Publish
        run: |
          dotnet publish src/AppName.Web/AppName.Web.csproj `
            --configuration Release `
            --output D:\Deploy\AppName-new
      
      - name: Run EF Core Migrations
        env:
          ConnectionStrings__DefaultConnection: ${{ secrets.CONNECTION_STRING }}
        run: |
          dotnet ef database update `
            --project src/AppName.Infrastructure `
            --startup-project src/AppName.Web
      
      - name: Stop IIS App Pool
        run: |
          Import-Module WebAdministration
          Stop-WebAppPool -Name "AppName"
          Start-Sleep -Seconds 5
      
      - name: Swap deployment
        run: |
          if (Test-Path D:\Deploy\AppName-backup) { 
            Remove-Item -Recurse D:\Deploy\AppName-backup 
          }
          if (Test-Path D:\wwwroot\AppName) { 
            Move-Item D:\wwwroot\AppName D:\Deploy\AppName-backup 
          }
          Move-Item D:\Deploy\AppName-new D:\wwwroot\AppName
      
      - name: Start IIS App Pool
        run: |
          Import-Module WebAdministration
          Start-WebAppPool -Name "AppName"
          Start-Sleep -Seconds 10
      
      - name: Health Check
        run: |
          $response = Invoke-WebRequest -Uri "http://localhost/health" -UseBasicParsing
          if ($response.StatusCode -ne 200) { exit 1 }
      
      - name: Rollback nếu thất bại
        if: failure()
        run: |
          Import-Module WebAdministration
          Stop-WebAppPool -Name "AppName"
          if (Test-Path D:\wwwroot\AppName) { Remove-Item -Recurse D:\wwwroot\AppName }
          Move-Item D:\Deploy\AppName-backup D:\wwwroot\AppName
          Start-WebAppPool -Name "AppName"
```

### 4. EF Core Migration Strategy

#### Quy trình chuẩn
```bash
# 1. Tạo migration mới (local)
dotnet ef migrations add AddMembershipApprovalDate \
  --project src/AppName.Infrastructure \
  --startup-project src/AppName.Web

# 2. Review script SQL trước khi apply
dotnet ef migrations script \
  --project src/AppName.Infrastructure \
  --startup-project src/AppName.Web \
  --output migrations_review.sql \
  --idempotent

# 3. Apply trên staging trước
dotnet ef database update \
  --project src/AppName.Infrastructure \
  --startup-project src/AppName.Web \
  --connection "Server=staging-db;..."

# 4. Apply production (trong CI/CD pipeline)
dotnet ef database update --no-build ...
```

#### Rules Migration (bắt buộc)
- **Luôn review script SQL** (`--idempotent` flag) trước khi apply lên môi trường dùng chung.
- **Không drop column/table ngay** nếu cột/bảng còn đang được dùng — dùng staging period.
- **Backup DB** trước khi apply migration có destructive change (DROP, ALTER).
- Migration file phải được commit vào Git, **không được sửa file migration đã apply** lên shared env.

### 5. Secret Management

#### GitHub Secrets (bắt buộc)
```
CONNECTION_STRING_PROD          # SQL Server connection string production
CONNECTION_STRING_STAGING       # SQL Server connection string staging
AZURE_PUBLISH_PROFILE           # Azure App Service publish profile XML
AZURE_APP_SERVICE_NAME          # Tên App Service trên Azure
APP_URL                         # URL public của ứng dụng
EMAIL_SMTP_PASSWORD             # SMTP password (nếu dùng SMTP)
ZALO_OA_ACCESS_TOKEN            # Zalo OA API token
```

#### appsettings theo môi trường
```
appsettings.json                # Base config (commit lên Git, không chứa secret)
appsettings.Development.json    # Local dev (gitignore nếu chứa secret)
appsettings.Staging.json        # Staging (không commit secret)
appsettings.Production.json     # Production (không commit secret)
```

**Cấu hình lấy secret từ môi trường:**
```csharp
// Program.cs
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()  // Override từ env vars/CI secrets
    .AddUserSecrets<Program>(optional: true);  // Local dev only
```

### 6. Health Check Setup

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "database", tags: ["db", "sql"])
    .AddUrlGroup(new Uri("https://api.zalo.me"), name: "zalo-api", tags: ["external"])
    .AddCheck<CustomBusinessCheck>("business-rules", tags: ["app"]);

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false  // Luôn trả 200 nếu process còn sống
});
```

### 7. Docker (Optional)

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AppName.Web/AppName.Web.csproj", "src/AppName.Web/"]
COPY ["src/AppName.Application/AppName.Application.csproj", "src/AppName.Application/"]
COPY ["src/AppName.Infrastructure/AppName.Infrastructure.csproj", "src/AppName.Infrastructure/"]
COPY ["src/AppName.Domain/AppName.Domain.csproj", "src/AppName.Domain/"]
RUN dotnet restore "src/AppName.Web/AppName.Web.csproj"
COPY . .
RUN dotnet publish "src/AppName.Web/AppName.Web.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AppName.Web.dll"]
```

### 8. Deployment Checklist

#### Pre-deployment
- [ ] Tất cả tests pass trên CI.
- [ ] Migration script đã được review và test trên Staging.
- [ ] Backup DB production đã được thực hiện.
- [ ] `appsettings.Production.json` / environment variables đã được verify.
- [ ] Health check endpoint `/health` đang trả 200 trên Staging.

#### Post-deployment (15 phút đầu)
- [ ] Health check `/health` trả 200.
- [ ] Kiểm tra application logs: không có ERROR level mới.
- [ ] Smoke test các flow nghiệp vụ chính (login, submit form, admin approve).
- [ ] Kiểm tra email/Zalo notification gửi được.
- [ ] Monitor Application Insights / Seq dashboard cho anomalies.

#### Rollback Trigger (bất kỳ điều kiện nào)
- Error rate > 5% trong 5 phút đầu sau deploy.
- Health check `/health` trả lỗi.
- Có Exception mới không có trong pre-deploy baseline.

### 9. Anti-pattern cần tránh

| Anti-pattern | Giải pháp |
|---|---|
| Deploy trực tiếp lên Production không qua Staging | Luôn deploy Staging → smoke test → Production |
| Migration trực tiếp trên Production DB không backup | Backup trước, test trên Staging trước |
| Secret hardcode trong CI yaml | Dùng GitHub Secrets / Key Vault |
| Không có health check | Implement `/health` endpoint |
| Deploy bằng cách copy thủ công | Dùng automated CI/CD pipeline |
| Không có rollback plan | Chuẩn bị script rollback trước mỗi deploy |
