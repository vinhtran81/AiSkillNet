---
name: hotfix
description: Production hotfix workflow for critical bugs with minimal downtime. Dùng khi cần xử lý bug nghiêm trọng trên production — triage, root cause analysis, fix, validate và post-mortem theo 5 phase.
---

# Hotfix Workflow

> **Lệnh**: `/hotfix`  
> **Mục đích**: Xử lý bug nghiêm trọng trên production nhanh chóng, có kiểm soát, downtime tối thiểu.

## Phase 1: Triage & Assessment

**Goal**: Tái hiện bug, đánh giá mức độ ảnh hưởng.

1. **Reproduce Bug & Logs**: [Senior QC](../skills/senior-qc/SKILL.md) tái hiện lỗi và thu thập logs.
2. **Impact Assessment**: [Senior Product Owner](../skills/senior-po/SKILL.md) đánh giá mức độ nghiêm trọng và phạm vi ảnh hưởng.
3. **Error Rate Check**: [Senior Data Analyst](../skills/senior-data-analyst/SKILL.md) kiểm tra tỷ lệ lỗi và metrics liên quan.

## Phase 2: Root Cause Analysis

**Goal**: Xác định nguyên nhân gốc.

1. **Backend Investigation**: [Senior Backend](../skills/senior-backend/SKILL.md) điều tra layer server/DB/API.
2. **Frontend Investigation**: [Senior Frontend](../skills/senior-frontend/SKILL.md) điều tra layer client/UI.
3. **Document Root Cause**: Ghi nhận nguyên nhân và phạm vi sửa tối thiểu.

## Phase 3: Fix Implementation

**Goal**: Tạo branch hotfix, sửa lỗi, viết test.

1. **Create Hotfix Branch**: Tách branch từ production/main theo quy ước dự án.
2. **Apply Minimal Fix**: [Senior Backend](../skills/senior-backend/SKILL.md) hoặc [Senior Frontend](../skills/senior-frontend/SKILL.md) — chỉ sửa phạm vi cần thiết.
3. **Write Regression Test**: [Senior QC](../skills/senior-qc/SKILL.md) bổ sung test chống tái phát.

## Phase 4: Validation & Deploy

**Goal**: Smoke test, code review, deploy production.

1. **Smoke Testing**: [Senior QC](../skills/senior-qc/SKILL.md) xác nhận fix không phá luồng chính.
2. **Code Review**: Review tối thiểu trước merge/deploy.
3. **Deploy to Production**: Triển khai theo [deploy-workflow.md](deploy-workflow.md) (rút gọn nếu khẩn).
4. **Post-Deploy Monitoring**: [Senior Data Analyst](../skills/senior-data-analyst/SKILL.md) theo dõi ~30 phút sau deploy.
5. **Merge back to develop**: Đồng bộ hotfix về nhánh phát triển.

## Phase 5: Post-Mortem

**Goal**: Tổng kết sự cố, cải tiến quy trình.

1. **Incident Report**: [Senior Product Owner](../skills/senior-po/SKILL.md) viết báo cáo sự cố.
2. **Process Improvement**: Cập nhật checklist, monitoring, hoặc test để tránh tái diễn.
