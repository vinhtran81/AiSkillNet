---
name: senior-po
description: Quản lý product backlog, viết PRD và User Story với Acceptance Criteria, UAT sign-off, định nghĩa tracking plan (PostHog/GA4). Dùng khi viết user story, ưu tiên backlog, định nghĩa success metric, chuẩn bị UAT, hoặc thiết lập analytics tracking plan.
---

## Senior Product Owner – Project Skill

### 1. Mục tiêu vai trò

- **Tập trung**: Nối business với team dev/design, đảm bảo làm đúng vấn đề quan trọng nhất, đúng thứ tự, đo được kết quả.
- **Thành công**: Roadmap rõ ràng, backlog được ưu tiên tốt, team hiểu “tại sao”, release đều đặn và cải thiện được KPI.

### 2. Nguyên tắc cốt lõi

- **Outcome over output**: Ưu tiên theo impact (KPI, OKR) chứ không chỉ số lượng task/release.
- **Single source of truth**: Requirement, use case, definition of done phải được ghi rõ, không để trôi nổi trong chat.
- **Discovery song song delivery**: Liên tục validate với user / data trong khi team đang build.
- **Stakeholder management**: Cân bằng nhu cầu nhiều bên, không để ai “lái” sản phẩm một chiều.
- **Empowered team**: Team hiểu context để tự ra quyết định nhỏ, PO không trở thành bottleneck.

### 3. Quy trình làm việc đề xuất

1. **Discovery & Alignment**
  - Xác định mục tiêu business (OKR, KPI), persona, problem statement rõ ràng.
  - Thống nhất scope & success metrics với stakeholder chính.
2. **Problem shaping**
  - Tạo file PRD chi tiết trong folder `.prd/` (mô tả problem, user story, AC).
  - **User Flow**: Sử dụng Mermaid để vẽ flow chi tiết cho các usecase chính.
  - Tạo Linear ticket cho feature và gán label/milestone phù hợp.
  - Làm việc với UX/UI để chuyển problem thành flow, user story, scenario.
3. **Backlog management**
  - Duy trì backlog luôn được **sắp xếp theo ưu tiên** và luôn có đủ item “ready” cho ít nhất 1–2 sprint.
  - Sử dụng tiêu chí ưu tiên (VD: RICE, MoSCoW, Impact/Effort) minh bạch.
4. **Delivery với team**
  - Tham gia/vận hành các ceremony: planning, daily, review, retro.
  - Đảm bảo dev hiểu rõ context, không chỉ đọc ticket.
  - Làm việc chặt với UX/UI & Tech Lead để refine giải pháp (không áp đặt thiết kế sẵn).
5. **Measure & Learn**
  - Định nghĩa metrics: adoption, activation, retention, conversion, NPS… tùy tính năng.
  - Sau mỗi release, review dữ liệu + feedback để điều chỉnh roadmap/backlog.

### 4. Các bước phát triển một tính năng (Feature Development Steps)

Quy trình từ ý tưởng đến khi người dùng sử dụng thực tế:

1. **Xác định & Nghiên cứu (Ideation & Discovery)**
  - Tìm hiểu nỗi đau (pain point) của người dùng thông qua feedback, interview hoặc data.
    - Xác định mục tiêu rõ ràng (Tại sao cần làm tính năng này? Nó giúp ích gì cho KPI/OKR?).
2. **Định hình tính năng (Feature Shaping & PRD)**
  - Viết PRD chi tiết: User Story, Acceptance Criteria (AC).
    - Vẽ **User Flow (Mermaid)** để hình dung hành trình người dùng.
    - Xác định phạm vi (Scope) cho phiên bản đầu tiên (MVP).
3. **Thiết kế (UI/UX Design)**
  - Phối hợp với Designer để tạo Wireframes/Mockups.
    - Team review thiết kế để đảm bảo đúng logic business và UX mượt mà.
4. **Hội quân & Ước lượng (Technical Refinement)**
  - Họp với team Dev (Front-end, Back-end) để đánh giá khả thi kỹ thuật.
    - Chia nhỏ tính năng thành các task kỹ thuật và ước lượng thời gian (Estimation).
5. **Lập kế hoạch & Ưu tiên (Planning & Prioritization)**
  - Đưa các task vào Sprint Planning.
    - Sắp xếp thứ tự ưu tiên trong Backlog dựa trên Impact vs Effort.
6. **Theo dõi Phát triển (Development Tracking)**
  - PO theo sát tiến độ trong Sprint, giải đáp các thắc mắc phát sinh của Dev/Designer.
    - Review bản demo sớm (Early Preview) để điều chỉnh kịp thời nếu có sai lệch.
7. **Kiểm soát chất lượng (QA & UAT)**
  - Phối hợp với QC để kiểm tra lỗi.
    - PO thực hiện **UAT (User Acceptance Testing)** để đảm bảo tính năng chạy đúng theo AC đã đề ra.
8. **Phát hành & Theo dõi (Release & Monitoring)**
  - Release tính năng (có thể rollout dần dần).
    - Theo dõi các chỉ số tracking (GA4, PostHog) và thu thập feedback người dùng để lên kế hoạch cải tiến (Next Version).

### 5. Checklist cho một feature “sẵn sàng”

- **Problem rõ ràng**
  - Đã được mô tả bằng 1–3 câu, gắn với KPI cụ thể.
  - Đã confirm với stakeholder chính.
- **User story & scope**
  - Có user story và acceptance criteria rõ ràng.
  - Scope được phân chia thành các increment nhỏ có thể ship độc lập.
- **Design & tech**
  - Đã có thiết kế/wireframe tối thiểu hoặc guideline UI/UX từ designer.
  - Tech lead/front-end/back-end đã review feasibility và chỉ ra dependency chính.
- **Tracking Ready**
  - Đã định nghĩa các event cần tracking (Google Analytics, PostHog); với PostHog xem **mục 9** (phạm vi trang & ưu tiên).
- **Risk & dependency**
  - Đã liệt kê risk chính + plan giảm thiểu.
  - Đã ghi rõ dependency giữa team/feature khác (nếu có).

### 6. Anti-pattern cần tránh

- “Yêu cầu” chi tiết layout, solution mà không cho UX/UI & dev space để đề xuất.
- Thay đổi scope liên tục giữa sprint mà không có quy tắc.
- Không đo lường kết quả, chỉ ship xong là coi như hoàn thành.
- Backlog chứa hàng trăm task nhưng không được ưu tiên, không ai hiểu vì sao làm.

### 7. Cách làm việc với các role khác

- **Senior UI/UX**
  - Cùng define problem, persona, journey, ưu tiên flow quan trọng nhất.
  - Chốt scope dựa trên bối cảnh dev (deadline, resource).
- **Senior Front-end**
  - Làm rõ behavior UI, edge cases, loading, error, empty state.
  - Thống nhất compromise khi có constraint kỹ thuật.
- **Senior Back-end**
  - Rõ ràng về dữ liệu, performance, security requirement.
  - Đảm bảo API contract phù hợp use case (pagination, filter, sort, error code).
- **Senior QC**
  - Hỗ trợ định nghĩa test strategy theo risk & critical path.
  - Đảm bảo acceptance criteria đủ chi tiết để viết test case tự tin.
- **Senior Data Analyst**
  - Thống nhất Tracking Plan và Dashboards để đo lường thành công của tính năng.

### 8. UAT & Deployment Sign-off Checklist

Trước khi ký duyệt release lên production:

1. **Acceptance Criteria Check**: Mọi AC trong PRD đều đã được pass.
2. **UI/UX Fidelity**: Tính năng thực tế khớp ít nhất 95% với mockup thiết kế.

2b. **Loading / perceived performance**: Các route chính (case study, profile, submissions, career playground) hiển thị skeleton hoặc tiến trình rõ ràng khi chờ dữ liệu — tham chiếu `.docs/specs/frontend_loading_skeleton_srs.md` và audit `.docs/specs/frontend_loading_states_audit.md`.
3. **Critical Path Verification**: Các luồng quan trọng nhất (Happy Path) hoạt động hoàn hảo.
4. **Stakeholder Approval**: Đã nhận được cái "gật đầu" từ các bên liên quan nếu tính năng có ảnh hưởng lớn.
5. **Release Note Ready**: Nội dung mô tả bản cập nhật đã sẵn sàng để gửi tới người dùng.

### 9. PostHog — phạm vi tracking & PO đề xuất

**PostHog** là analytics chính để phân tích hành vi người dùng trong ứng dụng MVC. PO phối hợp với DA để định nghĩa Tracking Plan và đảm bảo các event quan trọng được đo đúng.

PO chịu trách nhiệm **phạm vi đo** và **ưu tiên**, phối hợp DA/Frontend để ra **Tracking Plan** (tên event thống nhất `object_action`, không nhét PII vào properties).

#### 9.1. Nguyên tắc ưu tiên

| Tier | Mục tiêu | Ghi chú PO |
| --- | --- | --- |
| **P0** | Nền tảng | Page views toàn app, identify sau login (chỉ user ID nội bộ, không email plaintext). |
| **P1** | Luồng giá trị cốt lõi | Activation & retention: đăng ký hội viên, phê duyệt, gia hạn gói. |
| **P2** | Engagement | Xem thông báo, tra cứu gói, báo cáo. |
| **P3** | Admin / nội bộ | Tách biệt hoặc không track — tránh làm lệch funnel người dùng cuối. |

#### 9.2. Bảng trang (controller/action) đề xuất tích hợp

| Controller/Action (Razor MVC) | Ưu tiên | Lý do nghiệp vụ | Event / dimension gợi ý |
| --- | --- | --- | --- |
| Toàn app (`_Layout.cshtml`) | **P0** | Baseline traffic, path, device | `$pageview`; super property `app_area` |
| `Account/Login` | **P0** | Conversion đăng nhập | `auth_login_viewed`, `auth_login_submitted`, `auth_login_succeeded/failed` |
| `Membership/Register` | **P1** | Funnel đăng ký hội viên | `membership_register_started`, `membership_form_submitted` |
| `Membership/Approve` | **P1** | Phê duyệt đơn | `membership_approved`, `membership_rejected` + `admin_id` |
| `ServicePackage/Index` | **P1** | Xem gói dịch vụ | `package_list_viewed`, `package_selected` |
| `Notification/Index` | **P2** | Engagement thông báo | `notification_viewed`, `notification_clicked` |
| `Report/Index` | **P2** | Sử dụng báo cáo | `report_viewed`, `report_exported` với `format` (Excel/PDF) |
| `Admin/*` | **P3** | Vận hành nội bộ | `admin_area_viewed` + `route`; hoặc tắt capture |

#### 9.3. Việc PO cần làm trước khi dev bắt tay

1. Chốt **P0–P1** cho MVP tracking (đủ để đo funnel đăng ký → phê duyệt).
2. Tạo file Tracking Plan trong `.docs/specs/tracking_plan.md` — event dictionary + properties cho phép / cấm.
3. Thống nhất với **QC**: smoke test có bước verify event fire trên Staging (PostHog Live events).
4. Sau release: dùng `posthog` MCP để review funnel 2 tuần đầu và quyết định mở **P2** hay không.

---

### 10. Membership System — KPI Dashboard

Bảng KPI cốt lõi cần theo dõi sau mỗi release. Phối hợp với **Senior Data Analyst** để setup queries SQL Server và PostHog.

#### 10.1. North Star Metrics

| Metric | Đo bằng | Mục tiêu MVP | Mục tiêu 6 tháng |
|--------|---------|-------------|-----------------|
| **Approval Rate** | `Approved / Submitted` (SQL Server) | ≥ 70% | ≥ 85% |
| **Time to Approve** | Avg days từ submit → approve (SQL Server) | ≤ 5 ngày | ≤ 2 ngày |
| **Registration Funnel** | `register_started → submitted` (PostHog) | ≥ 60% | ≥ 75% |
| **Monthly Active Members** | Unique logins / tháng (PostHog) | — | Tăng 20% QoQ |
| **Notification Open Rate** | Zalo OA + Email open (MailKit log) | ≥ 30% | ≥ 50% |

#### 10.2. Leading Indicators (cảnh báo sớm)

| Signal | Ngưỡng cảnh báo | Hành động |
|--------|----------------|-----------|
| `membership_register_failed` tăng > 15% WoW | Form UX có vấn đề | Review error_code breakdown, contact UX |
| `auth_login_failed` tăng > 20% WoW | Brute-force hoặc UX nhập liệu khó | Alert Backend, review lockout policy |
| Time to Approve > 7 ngày | Quy trình admin bottleneck | Reminder Hangfire job, thông báo admin |
| Funnel drop `form_started → submitted` > 40% | Form quá dài hoặc validation lỗi | A/B test, UX audit |

#### 10.3. OKR Template — Membership Quarter

```
Objective: Tăng tỷ lệ đăng ký hội viên thành công
  KR1: Approval Rate đạt ≥ 80% trong Q2 (baseline: 65%)
  KR2: Time to Approve trung bình ≤ 3 ngày (baseline: 7 ngày)
  KR3: Funnel conversion register_started → succeeded đạt ≥ 70%

Objective: Cải thiện trải nghiệm Admin
  KR1: Admin phê duyệt 1-click từ dashboard — 0 clicks để vào trang chi tiết
  KR2: Notification phê duyệt Zalo OA có CTR ≥ 40%
```

---

### 11. Sprint Planning Checklist

Trước mỗi Sprint Planning, PO phải chuẩn bị:

**Backlog ready:**
- [ ] Top 10 items trong backlog đã được refined (User Story + AC đầy đủ).
- [ ] Mỗi story đã estimate (Story Points hoặc T-shirt size) — team không estimate "trắng".
- [ ] Dependencies giữa stories đã được chỉ rõ (story X phải xong trước story Y).
- [ ] Story không vượt quá 1/2 sprint capacity — nếu lớn hơn phải chia nhỏ.

**Context cho team:**
- [ ] "Why are we doing this sprint?" — 1 câu mục tiêu sprint, gắn OKR.
- [ ] Stakeholder feedback từ sprint trước đã được tích hợp vào backlog.
- [ ] Constraint mới (deadline, budget, dependency ngoài) đã được share.

**Tracking ready (trước khi code bắt đầu):**
- [ ] Tracking Plan cho tính năng mới đã confirm với DA.
- [ ] PostHog event names đã đặt tên chuẩn `object_action`, không PII.
- [ ] Feature flag (PostHog) đã tạo nếu cần rollout dần.

**After Sprint Planning:**
- [ ] Sprint Goal rõ ràng (1 câu) — gắn lên Linear/board.
- [ ] Burndown / capacity đã tính, không overcommit (để lại 20% buffer cho hotfix).
- [ ] First task của mỗi dev đã được assign rõ ràng, không ai chờ lúc đầu sprint.

---

### 12. Release Communication Template

Sau mỗi release gửi thông báo cho stakeholders (theo kênh phù hợp — Slack, email, Zalo):

```
🚀 SkillNet v[VERSION] — Release [DATE]

✅ Tính năng mới:
  • [Feature 1]: [1 câu mô tả lợi ích cho người dùng]
  • [Feature 2]: [...]

🔧 Cải tiến:
  • [Improvement]: [...]

🐛 Sửa lỗi:
  • [Bug fix]: [...]

📊 Metrics tuần đầu sẽ được share sau [DATE + 7 ngày].

👉 Xem chi tiết: [Link Release Notes / CHANGELOG]
```
