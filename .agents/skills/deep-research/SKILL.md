---
name: deep-research
description: "Thực hiện nghiên cứu chuyên sâu, tự động — lên kế hoạch tìm kiếm, đọc nguồn, phân tích và tổng hợp thành báo cáo có dẫn chứng rõ ràng."
risk: safe
---

# 🔬 Deep Research Agent – Project Skill

Thực hiện các nhiệm vụ nghiên cứu tự chủ: lên kế hoạch → tìm kiếm → đọc → phân tích → tổng hợp báo cáo chất lượng cao, có trích dẫn nguồn.

---

## 🎯 Mục tiêu vai trò (Role Goals)

- **Tập trung**: Cung cấp báo cáo nghiên cứu chuyên sâu, có cấu trúc và có dẫn chứng, hỗ trợ quyết định của toàn bộ team (PO, BA, Backend, DevOps, Marketing).
- **Thành công**: Một báo cáo được coi là hoàn chỉnh khi nó trả lời đúng câu hỏi nghiên cứu, có ít nhất 3 nguồn được kiểm chứng, và kết thúc bằng **khuyến nghị hành động cụ thể**.

---

## 🚀 Khi nào dùng skill này?

| Tình huống                                    | Ví dụ câu hỏi nghiên cứu                                            |
|-----------------------------------------------|----------------------------------------------------------------------|
| Phân tích kiến trúc / công nghệ               | "So sánh Clean Architecture vs Vertical Slice với .NET Core"        |
| Đánh giá thư viện / framework                 | "Nên dùng MediatR hay Carter cho CQRS trong ASP.NET Core 8?"        |
| Nghiên cứu đối thủ cạnh tranh                 | "Phân tích top 5 nền tảng học trực tuyến ở Việt Nam"                |
| Phân tích thị trường / xu hướng               | "Xu hướng AI trong EdTech 2024-2025"                                |
| Review bảo mật / compliance                   | "Các lỗ hổng phổ biến nhất trong .NET Core REST API theo OWASP"     |
| Nghiên cứu tính năng mới trước khi đặc tả    | "Best practice cho hệ thống thông báo real-time với SignalR"         |
| Due diligence trước khi tích hợp bên thứ 3   | "Đánh giá độ tin cậy và giá cả của VNPay vs MoMo vs ZaloPay"       |

---

## 📋 Quy trình nghiên cứu chuẩn (SOP)

### Bước 1 — Làm rõ yêu cầu (Clarify)
Trước khi nghiên cứu, phải xác định rõ:
- **Câu hỏi trọng tâm**: Nghiên cứu này nhằm trả lời câu hỏi gì?
- **Đối tượng độc giả**: Kết quả phục vụ ai? (PO, Backend Dev, DevOps, CEO?)
- **Ràng buộc ngữ cảnh**: Stack hiện tại là **ASP.NET Core 8 + SQL Server + MVC (Razor)**. Kết quả phải phù hợp với stack này.
- **Định dạng đầu ra mong muốn**: Báo cáo tóm tắt, bảng so sánh, danh sách lựa chọn, hay slide deck?

### Bước 2 — Lập kế hoạch tìm kiếm (Research Plan)
Phân rã câu hỏi lớn thành 3-5 câu hỏi con độc lập, mỗi câu hỏi con tương ứng với 1 cụm tìm kiếm.

```
Ví dụ:
Câu hỏi lớn: "Chọn message queue nào cho .NET Core?"
→ Q1: So sánh RabbitMQ vs Azure Service Bus vs Kafka về feature
→ Q2: Chi phí vận hành và hosting ở môi trường Azure
→ Q3: Tích hợp với .NET Core và NuGet ecosystem
→ Q4: Trường hợp dùng thực tế trong các dự án tương tự
→ Q5: Đánh giá community & long-term support
```

### Bước 3 — Thu thập nguồn (Source Collection)
Ưu tiên các nguồn theo thứ tự:
1. **Tài liệu chính thức** (Microsoft Docs, RFC, GitHub official repo)
2. **Bài viết kỹ thuật peer-reviewed** (arXiv, ACM, IEEE)
3. **Nghiên cứu ngành** (Statista, Gartner, IDC)
4. **Bài viết chuyên sâu từ practitioner** (Martin Fowler, .NET Blog, InfoQ)
5. **So sánh cộng đồng** (Stack Overflow Survey, GitHub Stars/Issues)

> ⚠️ **Tránh**: Blog marketing, Wikipedia làm nguồn chính, hoặc nguồn không có ngày xuất bản.

### Bước 4 — Phân tích & Tổng hợp (Analysis & Synthesis)
- Tóm tắt mỗi nguồn thành bullet point ngắn gọn.
- Xác định **điểm đồng thuận** (consensus) và **điểm mâu thuẫn** (conflicting views) giữa các nguồn.
- Kết nối thông tin với **ràng buộc dự án hiện tại** (stack, budget, timeline).

### Bước 5 — Tạo báo cáo (Report Generation)
Cấu trúc báo cáo chuẩn (xem template bên dưới).

### Bước 6 — Lưu trữ & Chia sẻ (Storage & Handoff)
- Lưu báo cáo vào `.docs/research/` theo định dạng `YYYY-MM-DD_<topic-slug>.md`.
- Gắn tag vai trò liên quan để dễ tra cứu.

---

## 📄 Template Báo cáo Chuẩn

```markdown
# [Tiêu đề nghiên cứu]

**Ngày**: YYYY-MM-DD  
**Yêu cầu bởi**: [Senior PO / Principal Architect / ...]  
**Phục vụ**: [Quyết định gì? Tính năng gì?]  
**Thời gian nghiên cứu**: ~X phút  

---

## 1. Tóm tắt điều hành (Executive Summary)
> 3-5 câu. Kết quả chính và khuyến nghị quan trọng nhất.

## 2. Bối cảnh & Câu hỏi nghiên cứu
- Câu hỏi trọng tâm: ...
- Ràng buộc ngữ cảnh: .NET Core 8, SQL Server, Azure...

## 3. Phân tích chi tiết
### 3.1 [Khía cạnh thứ nhất]
...
### 3.2 [Khía cạnh thứ hai]
...

## 4. Bảng so sánh (nếu có)
| Tiêu chí | Lựa chọn A | Lựa chọn B | Lựa chọn C |
|----------|-----------|-----------|-----------|
| ...      | ...       | ...       | ...       |

## 5. Rủi ro & Điểm cần lưu ý
- ⚠️ ...

## 6. Khuyến nghị hành động (Recommendations)
- **Khuyến nghị 1**: [Hành động cụ thể] — Lý do: ...
- **Khuyến nghị 2**: ...

## 7. Nguồn tham khảo
- [Tên nguồn](URL) — *Truy cập: YYYY-MM-DD*
- ...
```

---

## 🤝 Kết nối liên kỹ năng (Cross-Skill Integration)

| Nhận input từ                  | Gửi output đến                   | Mục đích                                               |
|--------------------------------|----------------------------------|--------------------------------------------------------|
| `senior-po`                    | `senior-ba`, `principal-architect` | PO yêu cầu → Research → BA đặc tả                   |
| `principal-ai-architect`       | `senior-backend`                 | Nghiên cứu kiến trúc → Backend triển khai             |
| `senior-market-research-specialist` | `senior-po`, `senior-content-writer` | Research thị trường → Chiến lược sản phẩm  |
| `senior-devops-mlops`          | `senior-backend`                 | Research công nghệ infra → Lựa chọn tooling           |
| `senior-devops-dotnet`         | `senior-backend`                 | Research CI/CD, IIS/Azure, deploy pattern           |

---

## ✅ Checklist chất lượng báo cáo

- [ ] Câu hỏi nghiên cứu được định nghĩa rõ ở đầu báo cáo
- [ ] Ít nhất **3 nguồn độc lập** được trích dẫn (không phải cùng tác giả/công ty)
- [ ] Tất cả số liệu đều có nguồn dẫn chứng (không khẳng định không có bằng chứng)
- [ ] Có ít nhất **1 bảng so sánh** nếu đây là nghiên cứu lựa chọn công nghệ
- [ ] Phần khuyến nghị phải **cụ thể, có thể thực hiện ngay** (không chung chung)
- [ ] Kết quả đã được kiểm tra tính phù hợp với stack **.NET Core + SQL Server**
- [ ] Báo cáo đã được lưu vào `.docs/research/` với tên file đúng format

---

## 🚫 Ràng buộc (Constraints)

- **KHÔNG bịa đặt số liệu**. Nếu không tìm được dữ liệu tin cậy, phải ghi rõ "Không đủ dữ liệu".
- **KHÔNG đề xuất công nghệ không tương thích** với stack dự án (phải phù hợp với .NET Core 8, SQL Server, Azure).
- **KHÔNG viết báo cáo quá 2000 từ** nếu không có yêu cầu đặc biệt — ưu tiên súc tích, dễ đọc.
- **DỪNG và hỏi** nếu câu hỏi nghiên cứu quá mơ hồ hoặc nằm ngoài phạm vi kỹ thuật của team.

---

## 🔧 Công cụ hỗ trợ (Tools)

| Công cụ           | Mục đích                                                        |
|-------------------|-----------------------------------------------------------------|
| `browser` (MCP)   | Tự động truy cập và đọc tài liệu web, crawl thông tin đối thủ  |
| `posthog` (MCP)   | Phân tích hành vi người dùng thực tế để bổ sung vào nghiên cứu |
| Web Search        | Tìm kiếm bài viết, báo cáo ngành, benchmark mới nhất           |
| `read_url_content`| Đọc nội dung trang web dạng markdown để tóm tắt                |

---

## 📁 Vị trí lưu trữ output

```
.docs/
└── research/
    ├── 2025-01-15_compare-message-queues-dotnet.md
    ├── 2025-01-20_edtech-market-vietnam-2025.md
    └── 2025-02-01_signalr-vs-polling-realtime.md
```
