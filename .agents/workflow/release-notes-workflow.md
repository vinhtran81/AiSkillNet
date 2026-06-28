---
name: release-notes
description: Professional release notes workflow for users, developers, and stakeholders. Dùng khi viết release notes cho một version mới — thu thập thay đổi, phân loại, viết nội dung user-facing và technical, review rồi publish.
---

# Release Notes Workflow

> **Lệnh**: `/release-notes`  
> **Mục đích**: Ghi nhận mọi thay đổi và truyền tải rõ tới người dùng, dev và stakeholder.

## Phase 1: Change Collection

**Goal**: Thu thập thay đổi từ Git, Linear, DB.

1. **Git History**: Review commits và merged PRs kể từ tag/release trước.
2. **Ticket Review**: [Senior Product Owner](../skills/senior-po/SKILL.md) — đối chiếu Linear/backlog.
3. **DB Migration Review**: [Senior Backend](../skills/senior-backend/SKILL.md) — migration breaking hoặc schema changes.

## Phase 2: Categorization

**Goal**: Phân loại thay đổi.

1. **Categories**: Features, Bugfixes, Improvements, Technical, Breaking, Deprecations.
2. **Prioritize by Impact**: [Senior Product Owner](../skills/senior-po/SKILL.md) — thứ tự hiển thị theo giá trị người dùng.

## Phase 3: Content Writing

**Goal**: Viết nội dung user-facing và technical notes.

1. **User-Facing Notes**: [Senior Content Writer](../skills/senior-content-writer/SKILL.md) — ngôn ngữ rõ, benefit-led.
2. **Technical Notes**: [Senior Backend](../skills/senior-backend/SKILL.md) — API/DB changes cho dev.
3. **Visual Assets**: [Senior UI/UX Designer](../skills/senior-uiux/SKILL.md) — screenshot/GIF nếu cần.
4. **SEO**: [Senior SEO Specialist](../skills/senior-seo-specialist/SKILL.md) — blog/changelog meta nếu publish web.

## Phase 4: Review & Publish

**Goal**: Kiểm tra chính xác, phê duyệt, phân phối.

1. **Accuracy Review**: [Senior QC](../skills/senior-qc/SKILL.md) — đối chiếu với build thực tế.
2. **Stakeholder Approval**: [Senior Product Owner](../skills/senior-po/SKILL.md).
3. **Publish Channels**: In-app, blog, App Store, email — theo kênh dự án.
4. **Track Engagement**: [Senior Data Analyst](../skills/senior-data-analyst/SKILL.md) — opens, clicks nếu có tracking.
