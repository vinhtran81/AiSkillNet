---
name: commit-push
description: Standardized git commit, push, and pull request preparation workflow. Dùng khi cần chuẩn hóa quy trình đẩy code — lint, format, Conventional Commit message, rebase và chuẩn bị PR description.
---

# Commit & Push Workflow

> **Lệnh**: `/commit-push`  
> **Mục đích**: Chuẩn hóa đẩy code lên repository — lịch sử git sạch, chất lượng code đảm bảo.

## Phase 1: Pre-flight

**Goal**: Lint, format và test nội bộ.

1. **Lint & Format**: [Senior Backend](../skills/senior-backend/SKILL.md) / [Senior Frontend](../skills/senior-frontend/SKILL.md) — chạy linter/formatter dự án.
2. **Local Tests**: Unit/integration tests pass trước khi commit.
3. **No Secrets**: Không commit `.env`, credentials, API keys.

## Phase 2: Standardization

**Goal**: Viết commit message theo chuẩn Conventional Commits.

1. **Message Format**: `type(scope): description` — feat, fix, refactor, docs, test, chore...
2. **Atomic Commits**: Mỗi commit một mục đích rõ; tránh “WIP” trên main/develop.

## Phase 3: Remote Push

**Goal**: Rebase và đẩy code lên remote.

1. **Sync with Remote**: `git fetch`; rebase hoặc merge theo policy team.
2. **Push Branch**: `git push -u origin <branch>` khi branch mới.

## Phase 4: PR Prep

**Goal**: Chuẩn bị Pull Request và link ticket.

1. **PR Description**: Summary, test plan, screenshots nếu UI.
2. **Link Ticket**: Linear/Jira/issue ID trong PR body.
3. **Reviewers**: Gán reviewer theo CODEOWNERS hoặc quy ước team.
