---
name: refactor
description: Code refactor workflow to reduce technical debt without changing external behavior. Dùng khi cần cải tiến cấu trúc code — cleanup, restructure layer, xử lý technical debt mà không đổi behavior bên ngoài.
---

# Refactor Workflow

> **Lệnh**: `/refactor`  
> **Mục đích**: Cải tiến cấu trúc code mà không thay đổi hành vi bên ngoài; giảm nợ kỹ thuật, tăng khả năng bảo trì.

## Phase 1: Analysis & Scoping

**Goal**: Xác định hotspot, nợ kỹ thuật và rủi ro.

1. **Hotspot Identification**: [Senior Backend](../skills/senior-backend/SKILL.md) và [Senior Frontend](../skills/senior-frontend/SKILL.md) liệt kê module/file cần refactor.
2. **Risk Assessment**: [Senior Product Owner](../skills/senior-po/SKILL.md) xác nhận phạm vi, ưu tiên và acceptance criteria (hành vi không đổi).
3. **Scope Document**: Ghi rõ in-scope / out-of-scope và tiêu chí hoàn thành.

## Phase 2: Implementation

**Goal**: Thực hiện cleanup, cấu trúc lại code.

1. **Backend Refactor**: [Senior Backend](../skills/senior-backend/SKILL.md) — tách module, đổi tên, chuẩn hóa API nội bộ (không breaking contract công khai).
2. **Frontend Refactor**: [Senior Frontend](../skills/senior-frontend/SKILL.md) — component structure, hooks, state management.
3. **Incremental Commits**: Commit nhỏ, dễ review; mỗi bước vẫn build/test pass.

## Phase 3: Validation & Performance

**Goal**: Kiểm thử hồi quy, đo lường hiệu năng.

1. **Regression Testing**: [Senior QC](../skills/senior-qc/SKILL.md) chạy test suite và smoke critical paths.
2. **Performance Check**: [Senior Data Analyst](../skills/senior-data-analyst/SKILL.md) so sánh metrics trước/sau (nếu có).
3. **UX Sanity**: [Senior UI/UX Designer](../skills/senior-uiux/SKILL.md) xác nhận UI/flow không lệch so với spec.

## Phase 4: Sign-off & Merge

**Goal**: Review chéo và tích hợp vào main branch.

1. **Cross Review**: Backend review FE (và ngược lại) nếu chạm boundary.
2. **Merge**: Theo [commit-push-workflow.md](commit-push-workflow.md) — PR, conventional commits, CI pass.
