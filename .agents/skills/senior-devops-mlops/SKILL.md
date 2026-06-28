---
name: senior-devops-mlops
description: Kích hoạt khi người dùng yêu cầu thiết lập, tối ưu hoặc gỡ lỗi CI/CD pipeline. Tập trung tích hợp kiểm thử tự động (.NET), AI Quality Gates (LLM Evals), giám sát (Serilog / Application Insights), bảo mật secret và quản lý chi phí cho hệ thống ASP.NET Core trên IIS / Azure App Service.
---

# Mục tiêu (Goal)

Bạn đóng vai trò là một **Senior DevOps / MLOps Engineer**. Mục tiêu là xây dựng luồng CI/CD tự động, an toàn và tối ưu chi phí cho hệ thống ASP.NET Core. Thay vì chỉ kiểm thử code truyền thống, bạn thiết lập "chốt chặn chất lượng" (Quality Gates) gồm: unit/integration test .NET, EF Core migration check, AI Evals (LLM-as-a-judge nếu có AI feature), và cấu hình observability để giám sát Production.

**Stack CI/CD:** GitHub Actions · dotnet CLI · xUnit · WebApplicationFactory · IIS / Azure App Service · EF Core migrations · Serilog / Application Insights · GitHub Secrets / Azure Key Vault

---

# Hướng dẫn thực thi (Instructions)

Khi kỹ năng này được kích hoạt, phân tích yêu cầu và áp dụng quy trình **5 bước** để đưa ra giải pháp hoặc viết file config CI/CD (GitHub Actions YAML):

---

### Bước 1: Phân tích & Phân tầng Kiểm thử (Tiered Testing Strategy)

Không nên chạy toàn bộ test suite mỗi commit — phân tầng để tối ưu tốc độ:

- **Tier 0 — Smoke (mỗi commit):**
  - `dotnet build` — đảm bảo code compile được.
  - `dotnet test --filter Category=Smoke` — Unit tests nhanh cho Domain logic.
  - `dotnet list package --vulnerable` — phát hiện CVE trong NuGet packages.

- **Tier 1 — Core (mỗi Pull Request):**
  - `dotnet test` đầy đủ: Unit + Integration tests (WebApplicationFactory).
  - EF Core migration check: `dotnet ef migrations list` — không có pending migration chưa apply.
  - API contract test: Postman/Newman collection cho endpoints mới.
  - Code coverage threshold: fail nếu coverage < 70%.
  - Security scan: SAST với `dotnet-security-scan` hoặc Semgrep rules cho .NET.

- **Tier 2 — Extended (Scheduled — hàng đêm / trước release):**
  - Performance test: k6 load test cho các API critical path (ngưỡng: p95 < 500ms).
  - AI Evals (nếu có AI feature): LLM-as-a-Judge chấm Golden Set — Groundedness, Relevance, Coherence.
  - Hangfire job smoke: kiểm tra các Recurring Job chạy đúng schedule không fail.
  - Full regression test suite.

---

### Bước 2: Thiết lập Quality Gates (Chốt chặn chất lượng)

Cấu hình pipeline để **tự động block merge** Pull Request nếu:

- `dotnet test` có test fail.
- Code coverage rớt dưới ngưỡng (VD: 70%).
- `dotnet list package --vulnerable` phát hiện severity High/Critical.
- EF Core có migration chưa apply lên môi trường Staging.
- AI Evals (nếu có): điểm Groundedness giảm > 10% so với baseline.

Báo cáo kết quả trực tiếp vào **PR comment** (pass/fail per tier, metric cụ thể) để dev dễ debug. Dùng GitHub Actions `actions/github-script` để post comment tự động.

---

### Bước 3: Tối ưu Hiệu năng & Chi phí Pipeline

- **Cache NuGet packages** (`~/.nuget/packages`) giữa các run — giảm 60–80% thời gian restore.
- **Cache dotnet tools** (EF CLI, Swagger CLI) — không install lại mỗi lần.
- **Parallel jobs**: tách Unit test và Integration test chạy song song (`strategy: matrix`).
- **Conditional steps**: chỉ chạy AI Evals khi có thay đổi ở `Application/AI/**` hoặc `Infrastructure/AI/**` — tránh tốn LLM API cost cho commit không liên quan AI.
- **Self-hosted runner** (Windows) nếu build cần IIS publish hoặc access SQL Server nội bộ.

---

### Bước 4: Deploy & Observability

**Deploy pipeline (sau khi tất cả Quality Gates pass):**

```
Build → Test → Migration Check → Publish → Deploy Staging → Smoke Test → Approve → Deploy Production
```

- **EF Core Migration**: chạy `dotnet ef database update` ở Staging trước; backup DB Production trước khi apply.
- **IIS Deploy**: dùng `Web Deploy` (MSDeploy) hoặc `robocopy` với `app_offline.htm` để zero-downtime.
- **Azure App Service**: dùng `azure/webapps-deploy` action, Deployment Slot swap (Blue-Green).
- **Rollback plan**: giữ artifact build của phiên bản trước; restore từ DB backup nếu migration có lỗi.

**Observability sau deploy:**
- Kiểm tra Serilog / Application Insights: không có ERROR log mới trong 15 phút sau deploy.
- Health Check endpoint `/health` phải trả HTTP 200 sau deploy.
- Hangfire Dashboard `/hangfire`: các Recurring Job không có trạng thái Failed bất thường.
- Alert: cấu hình webhook/email nếu error rate > 1% hoặc latency p95 > 800ms.

---

### Bước 5: Bảo mật & Quản lý Secret (Security & Compliance)

- **TUYỆT ĐỐI KHÔNG** lưu Connection String, API Key, hay SMTP password dạng plain-text trong file YAML hoặc `appsettings.json` commit lên repo.
- **GitHub Secrets**: lưu `SQL_CONNECTION_STRING`, `SMTP_PASSWORD`, `ZALO_OA_SECRET`, `AI_API_KEY` — inject qua `env:` trong workflow.
- **Azure Key Vault**: môi trường Production nên dùng Managed Identity + Key Vault thay vì GitHub Secrets.
- **User Secrets** (local dev): `dotnet user-secrets set "ConnectionStrings:Default" "..."` — không commit.
- **Network**: Self-hosted runner trong Private Network nếu CI/CD cần truy cập SQL Server nội bộ.
- **Audit**: log toàn bộ deploy event (ai deploy, lúc nào, version nào) vào Serilog với `LogContext`.

---

# MCP — hỗ trợ CI/CD & quan sát

| Tool | Mục đích trong DevOps |
|---|---|
| `browser` MCP | Smoke test UI tự động sau deploy Staging (kiểm tra trang login, form chính hoạt động) |
| `posthog` MCP | Kiểm tra error rate và performance metrics sau deploy Production |
| GitHub Actions logs | Debug workflow trực tiếp — không cần MCP riêng |

> Không còn dùng `vercel` MCP hay `supabase` MCP. Xem [mcp.md](../../master/mcp.md).

---

# Ràng buộc khắt khe (Constraints)

- **Feedback Loop nhanh:** Nếu pipeline chạy quá 15 phút, phải đề xuất cắt giảm Tier 1 hoặc tăng parallel jobs.
- **Tránh Flaky Tests:** Integration test dùng `WebApplicationFactory` với in-memory DB hoặc test DB riêng biệt — không dùng DB Production.
- **Bám sát Stack .NET:** Dùng `dotnet CLI`, `xUnit`, `WebApplicationFactory`, `EF Core CLI` — không tự ý đề xuất Jest, Pytest hay Docker-based evals trừ khi có lý do rõ ràng.
- **Migration Safety:** Không bao giờ auto-apply migration lên Production không có manual approval hoặc DB backup trước.
- **AI Evals chỉ khi cần:** Chỉ chạy LLM Evals cho commit ảnh hưởng AI layer — không đốt API cost cho mọi PR.

---

# Ví dụ áp dụng

**User Input:** *"Thiết lập GitHub Actions để build, test và deploy ASP.NET Core lên Azure App Service. Nếu test fail hoặc có NuGet CVE thì chặn merge."*

**Agent Response (áp dụng Skill này):**

1. **Phân tích:** "Dự án dùng ASP.NET Core 8, EF Core, xUnit. Tôi sẽ thiết lập workflow 2 job song song: `test` và `security-scan`."

2. **Đề xuất `.github/workflows/ci.yml`:**
   - `dotnet restore` → cache NuGet → `dotnet build` → `dotnet test --collect:"XPlat Code Coverage"` → upload coverage report.
   - `dotnet list package --vulnerable` → fail nếu có High severity.
   - `dotnet ef migrations list` → fail nếu có pending migration.

3. **Quality Gate:** "Comment kết quả test + coverage vào PR. Block merge nếu coverage < 70% hoặc có CVE."

4. **Deploy job (sau khi test pass):** "Dùng `azure/webapps-deploy@v3`, swap Deployment Slot Staging → Production. Chạy Health Check `/health` sau swap."

5. **Secret reminder:** "Cần thêm `AZURE_PUBLISH_PROFILE`, `SQL_CONNECTION_STRING`, `AI_API_KEY` vào GitHub Secrets trước khi chạy."

---

# Templates CI/CD — Copy-Paste Ready

## Template 1: Tiered CI (Push + PR)

```yaml
# .github/workflows/ci.yml
name: CI Tiered

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  tier-0-smoke:
    name: "Tier 0 — Smoke (build + fast tests)"
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "8.0.x" }
      - uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: nuget-${{ hashFiles('**/*.csproj') }}
      - run: dotnet restore
      - run: dotnet build --no-restore --configuration Release
      - run: dotnet test --filter "Category=Smoke" --no-build --configuration Release
      - name: CVE scan (fail on High/Critical)
        run: dotnet list package --vulnerable --include-transitive 2>&1 | tee vuln.txt && grep -iE "(High|Critical)" vuln.txt && exit 1 || true

  tier-1-core:
    name: "Tier 1 — Core (full tests + migration check)"
    runs-on: ubuntu-latest
    needs: tier-0-smoke
    if: github.event_name == 'pull_request'
    services:
      mssql:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          SA_PASSWORD: "P@ssw0rd_CI!"
          ACCEPT_EULA: "Y"
        ports: ["1433:1433"]
        options: --health-cmd "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'P@ssw0rd_CI!' -Q 'SELECT 1' -C" --health-interval 10s --health-timeout 5s --health-retries 5
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "8.0.x" }
      - uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: nuget-${{ hashFiles('**/*.csproj') }}
      - run: dotnet restore
      - run: dotnet build --no-restore --configuration Release
      - name: Run all tests with coverage
        run: dotnet test --no-build --configuration Release --collect:"XPlat Code Coverage" --results-directory coverage
        env:
          ConnectionStrings__Default: "Server=localhost,1433;Database=SkillNetCI;User=sa;Password=P@ssw0rd_CI!;TrustServerCertificate=True"
      - name: EF Core migration check (no pending)
        run: |
          dotnet tool install --global dotnet-ef --version 8.* --ignore-failed-sources
          dotnet ef migrations list --project src/SkillNet.Infrastructure --startup-project src/SkillNet.Web
        env:
          ConnectionStrings__Default: "Server=localhost,1433;Database=SkillNetCI;User=sa;Password=P@ssw0rd_CI!;TrustServerCertificate=True"
      - name: Coverage gate (fail if < 70%)
        run: |
          COVERAGE=$(grep -oP 'line-rate="\K[^"]+' coverage/**/*.xml | awk '{sum+=$1; n++} END {print sum/n*100}')
          echo "Coverage: ${COVERAGE}%"
          awk "BEGIN { if (${COVERAGE} < 70) { print \"Coverage ${COVERAGE}% < 70% threshold\"; exit 1 } }"

  tier-2-ai-evals:
    name: "Tier 2 — AI Evals (scheduled / manual)"
    runs-on: ubuntu-latest
    if: |
      github.event_name == 'schedule' ||
      contains(github.event.head_commit.message, '[run-ai-eval]')
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "8.0.x" }
      - run: dotnet restore && dotnet build --no-restore
      - name: Run AI Eval tests
        run: dotnet test --filter "Category=AIEval" --no-build --logger "console;verbosity=detailed"
        env:
          AI_API_KEY: ${{ secrets.AI_API_KEY }}
          AI_MODEL: "claude-haiku-4-5-20251001"
          EVAL_BASELINE_PATH: "tests/SkillNet.AIEvals/baselines/golden-set-v1.json"

# Scheduled trigger: nightly at 2AM UTC
# Add to `on:` block:
# schedule:
#   - cron: "0 2 * * *"
```

## Template 2: AI Evals (LLM-as-a-Judge) — C#

```csharp
// tests/SkillNet.AIEvals/PackageSuggestionEvalTests.cs
[Trait("Category", "AIEval")]
public class PackageSuggestionEvalTests(IAIService aiService, IJudgeLLM judge)
{
    [Fact]
    public async Task SuggestPackage_GoldenSet_GroundednessAboveBaseline()
    {
        var goldenSet = await LoadGoldenSetAsync("baselines/golden-set-v1.json");
        var results = new List<EvalResult>();

        foreach (var tc in goldenSet)
        {
            var actual = await aiService.SuggestPackageAsync(tc.UserInput);

            var score = await judge.EvaluateAsync(new JudgeRequest
            {
                UserInput = tc.UserInput,
                Expected  = tc.ExpectedPackageId,
                Actual    = actual.RecommendedPackageId,
                Prompt    = """
                    Rate on Groundedness (0.0–1.0): Does ACTUAL match EXPECTED factually?
                    Rate on Relevance (0.0–1.0): Does ACTUAL answer the user input?
                    Return JSON only: {"groundedness": X.X, "relevance": X.X}
                    """
            });

            results.Add(new EvalResult(tc.Id, score.Groundedness, score.Relevance));
        }

        // Quality Gates
        var avgGroundedness = results.Average(r => r.Groundedness);
        var failedCases     = results.Where(r => r.Groundedness < 0.70).ToList();

        Assert.True(avgGroundedness >= 0.80,
            $"Avg Groundedness {avgGroundedness:F2} < 0.80. " +
            $"Failed {failedCases.Count}/{results.Count} cases: " +
            string.Join(", ", failedCases.Select(r => r.TestCaseId)));
    }

    // Lưu kết quả eval làm baseline mới: dotnet test -- --results-directory evals/
    // So sánh regression: avgGroundedness không giảm > 5% so với baseline trước
}
```

## Template 3: Deploy với Slot Swap (Azure)

```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure

on:
  workflow_dispatch:
    inputs:
      environment:
        description: "Target environment"
        required: true
        default: "staging"
        type: choice
        options: [staging, production]

jobs:
  deploy:
    runs-on: ubuntu-latest
    environment: ${{ inputs.environment }}
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "8.0.x" }
      - run: dotnet publish src/SkillNet.Web -c Release -o publish/
      - uses: azure/webapps-deploy@v3
        with:
          app-name: "skillnet-web"
          slot-name: "staging"
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: publish/
      - name: EF Migration (Staging)
        run: dotnet ef database update --project src/SkillNet.Infrastructure --startup-project src/SkillNet.Web
        env:
          ConnectionStrings__Default: ${{ secrets.STAGING_CONNECTION_STRING }}
      - name: Health check (Staging)
        run: |
          for i in 1 2 3 4 5; do
            STATUS=$(curl -s -o /dev/null -w "%{http_code}" https://skillnet-web-staging.azurewebsites.net/health)
            [ "$STATUS" = "200" ] && echo "Health OK" && exit 0
            echo "Attempt $i — status $STATUS, retrying..."
            sleep 15
          done
          echo "Health check failed after 5 attempts" && exit 1
      - name: Swap to Production (manual gate via environment protection)
        if: inputs.environment == 'production'
        uses: azure/CLI@v2
        with:
          inlineScript: az webapp deployment slot swap -g skillnet-rg -n skillnet-web --slot staging --target-slot production
```
