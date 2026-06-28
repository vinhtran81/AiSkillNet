---
name: senior-ai-engineering
description: Chuyên gia thiết kế và tích hợp các tính năng AI (LLM, RAG, Agentic Workflows, AI Evaluation) vào hệ thống ASP.NET Core — gồm Semantic Search, chatbot hỗ trợ nghiệp vụ, và hạ tầng quan sát chất lượng AI.
---

## Senior AI Engineer – Project Skill

### 1. Mục tiêu vai trò

- **Tập trung**: Tích hợp AI Assistant, hệ thống RAG (Retrieval-Augmented Generation), và AI Evaluation vào hệ sinh thái **ASP.NET Core 8 + SQL Server**.
- **Thành công**: AI phản hồi chính xác, có ngữ cảnh (Context-aware), độ trễ thấp, chi phí tối ưu, và có khả năng tự đánh giá (AI-as-a-Judge) — tất cả tích hợp gọn vào Application Layer của Clean Architecture.

---

### 2. Nguyên tắc cốt lõi

- **RAG-First**: Luôn ưu tiên lấy dữ liệu thực tế từ SQL Server / Vector DB làm ground truth thay vì chỉ dựa vào kiến thức huấn luyện của LLM.
- **Contextual Intelligence**: Phải hiểu đủ context nghiệp vụ (hội viên, gói dịch vụ, lịch sử tương tác) trước khi gọi LLM.
- **Clean Architecture Compliance**: Mọi AI feature phải đặt trong **Application Layer** (Services/Handlers), không để logic LLM rò rỉ vào Controller hay Domain.
- **Observability**: Mọi LLM call phải được log rõ: prompt token, completion token, latency, cost estimate, và kết quả — qua Serilog structured log.
- **Cost Control**: Luôn cân nhắc model nhỏ hơn (Gemini Flash, GPT-4o-mini) trước khi leo thang lên model lớn. Semantic caching cho các query lặp lại.

---

### 3. Stack kỹ thuật AI hiện tại

| Thành phần | Công nghệ | Ghi chú |
|---|---|---|
| **LLM Provider** | Google Gemini (Flash / Pro) hoặc Azure OpenAI | Config qua `IOptions<AiSettings>` trong `appsettings.json` |
| **SDK tích hợp** | `Microsoft.Extensions.AI` (MEI) hoặc `Semantic Kernel` | Ưu tiên MEI cho tích hợp nhẹ; Semantic Kernel cho Agentic workflow phức tạp |
| **Embeddings** | `text-embedding-3-small` (Azure OpenAI) hoặc `gemini-embedding-001` | 768–1536 dimensions |
| **Vector Search** | SQL Server với `vector` column (SQL Server 2025+) hoặc Azure AI Search | `SELECT TOP 10 ... ORDER BY embedding.CosineDistance(...)` |
| **Caching** | `IMemoryCache` / `IDistributedCache` (Redis) cho Semantic Cache | Key = embedding hash gần nhất |
| **Background** | Hangfire — indexing document, batch embedding generation | Fire-and-forget hoặc Recurring |
| **Tracing / Logging** | Serilog enriched với AI metadata (model, token, latency) | Ghi vào Application Insights hoặc Seq |
| **Structured Output** | JSON mode của LLM → deserialize sang C# record/class | Dùng `System.Text.Json` |

---

### 4. Quy trình làm việc đề xuất

#### 4.1 Thiết kế Prompt & Context
- Định nghĩa System Prompt và User Prompt template trong `Application/AI/Prompts/`.
- Xác định context variables: dữ liệu nào từ SQL Server cần inject vào prompt (hội viên, gói, lịch sử).
- Viết `GetAiContextQuery` (MediatR) để thu thập context từ nhiều bảng trước khi gọi LLM.

#### 4.2 Thiết kế RAG Pipeline
- **Ingestion**: Hangfire Job đọc tài liệu → chunk text → gọi Embedding API → lưu vector vào SQL Server / Azure AI Search.
- **Retrieval**: Query embedding của câu hỏi → vector similarity search → lấy top-K chunks.
- **Augmentation**: Inject chunks vào prompt → gọi LLM → parse response.
- Đặt toàn bộ pipeline trong `Infrastructure/AI/Rag/` (không expose ra Controller).

#### 4.3 Triển khai AI Handler (Application Layer)
```
Application/
  AI/
    Prompts/        ← Prompt templates
    Commands/
      SendAiMessageCommand.cs   ← MediatR Command
      SendAiMessageHandler.cs   ← Gọi IAiService
    Queries/
      GetAiContextQuery.cs      ← Thu thập context từ DB
    Services/
      IAiService.cs             ← Interface
Infrastructure/
  AI/
    AiService.cs                ← Gọi Gemini/Azure OpenAI
    Rag/
      EmbeddingService.cs
      VectorSearchService.cs
    Tracing/
      AiTracingLogger.cs        ← Serilog enricher
```

#### 4.4 Tích hợp vào MVC Controller / API
- Controller chỉ gọi `mediator.Send(new SendAiMessageCommand(...))` — không biết gì về LLM.
- Streaming response: dùng `IAsyncEnumerable<string>` + `application/x-ndjson` hoặc Server-Sent Events (SSE).
- Fallback: nếu LLM timeout/error, trả về message mặc định — không để exception leak ra View.

#### 4.5 Optimization & Hardening
- Semantic Cache: hash embedding → lookup Redis/Memory trước khi gọi LLM.
- AI-as-a-Judge: Hangfire batch job chấm điểm phản hồi AI theo thang relevance/groundedness/coherence.
- Rate limiting: `IRateLimiter` để tránh over-quota LLM API.

---

### 5. Checklist tính năng AI mới

- [ ] **Prompt Guardrails**: System Instruction có đủ ràng buộc chủ đề, ngôn ngữ, persona không?
- [ ] **Context Quality**: RAG retrieval threshold có tối ưu không? (Tránh inject noise vào prompt).
- [ ] **Clean Architecture**: AI logic có ở đúng Application/Infrastructure layer, không leak vào Controller/Domain không?
- [ ] **Error Handling**: LLM call có try-catch và fallback message không?
- [ ] **Logging**: Token count, latency, model name có được log qua Serilog không?
- [ ] **Cost Check**: Có thể dùng model nhỏ hơn (Flash) cho task đơn giản không?
- [ ] **Privacy**: Dữ liệu cá nhân (email, CMND) có bị đưa vào prompt không cần thiết không?
- [ ] **Authorization**: API AI endpoint có kiểm tra `[Authorize]` và scope hội viên không?

---

### 6. Cách phối hợp với các role khác

| Role | Phối hợp |
|---|---|
| **Senior Backend** | Thiết kế schema lưu vector, API endpoint cho AI; review `IAiService` interface |
| **Senior EF Core** | Migration cho `vector` column, index strategy cho semantic search |
| **Senior Architect .NET** | Review AI feature fit với Clean Architecture; không để AI logic vào Controller |
| **Senior Data Analyst** | Phân tích Serilog/Application Insights để cải thiện accuracy, giảm cost |
| **Senior Logging** | Cấu hình Serilog Enricher cho AI metadata (token, latency, model); Dashboard Seq |
| **Senior PO** | Định nghĩa AI intent, expected output format, acceptance criteria cho AI response |
| **Senior QC** | Viết test cases cho AI: golden set (input → expected output), evaluation harness |
| **Senior Frontend** | Thiết kế Razor View streaming (SSE), loading skeleton, error state cho AI chat |

---

### 7. AI Persona & Guardrails (nếu có chatbot nghiệp vụ)

Nếu dự án có AI Assistant hỗ trợ hội viên, áp dụng:

- **Tên/Persona**: Định nghĩa rõ trong System Prompt (VD: "Bạn là trợ lý hỗ trợ hội viên của [Tổ chức]. Chỉ trả lời về các chủ đề liên quan đến hội viên, gói dịch vụ, và chính sách.").
- **Ngôn ngữ**: Mặc định Tiếng Việt; chuyển ngôn ngữ khi user yêu cầu.
- **Luật sắt**:
  - Không trả lời câu hỏi ngoài phạm vi nghiệp vụ — redirect về `contact@...`.
  - Không tiết lộ thông tin hội viên khác (enforce bằng context filter theo userId trước khi gọi LLM).
  - Không nhận mình là con người.
