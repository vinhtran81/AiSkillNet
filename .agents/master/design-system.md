---
name: design-system
description: Design tokens, UI components và quy tắc trạng thái của dự án ASP.NET Core MVC — màu sắc (#FF5000), typography, spacing (4px scale), Bootstrap 5 conventions, responsive rules. Tài liệu tham chiếu cho senior-uiux và senior-frontend.
---

# Design System

Tài liệu này lưu trữ các nguyên tắc thiết kế, Design Tokens, UI Components và quy tắc trạng thái của dự án, nhằm đảm bảo tính nhất quán trên toàn bộ ứng dụng **ASP.NET Core MVC**.

> **Tech Stack Frontend**: Bootstrap 5 + CSS Custom Properties + Vanilla JavaScript / jQuery

---

## 1. Nguyên tắc thiết kế (Design Principles)

- **Mobile-first**: Ưu tiên giao diện trên thiết bị di động trước, mở rộng lên desktop.
- **Accessibility**: Đảm bảo mọi người dùng đều có thể truy cập (WCAG 2.1 AA tối thiểu). Touch target tối thiểu 44px.
- **Consistency**: Nhất quán về màu sắc, typography, khoảng cách và trạng thái trên toàn app.
- **No Gradients**: Không sử dụng gradient phức tạp. Ưu tiên màu phẳng, đường viền rõ ràng.
- **Reusable Components**: Mọi UI element phải được tách thành component tái sử dụng, không hard-code style.

---

## 2. Design Tokens — Màu sắc (Colors)

> **Triển khai**: CSS Custom Properties trong `wwwroot/css/site.css`. Ưu tiên CSS Variables thay vì Tailwind.

| Token | CSS Variable | Giá trị | Mô tả |
|-------|-------------|---------|-------|
| Primary | `--color-primary` | `#FF5000` | Màu chính — CTA buttons, links, highlights |
| Primary Hover | `--color-primary-hover` | `#E04500` | Màu primary khi hover |
| Primary Light | `--color-primary-light` | `#FFF0EB` | Background nhẹ cho primary context |
| Secondary | `--color-secondary` | `#6c757d` | Màu phụ — secondary buttons, labels |
| Background | `--color-background` | `#F8F9FA` | Nền trang chính (xám nhạt) |
| Surface | `--color-surface` | `#FFFFFF` | Nền card, modal, panel |
| Text Primary | `--color-text-primary` | `#1A1A1A` | Văn bản chính |
| Text Secondary | `--color-text-secondary` | `#6B7280` | Văn bản phụ, placeholder |
| Border | `--color-border` | `#E5E7EB` | Đường viền mặc định |
| Error | `--color-error` | `#DC2626` | Lỗi, trạng thái nguy hiểm |
| Success | `--color-success` | `#16A34A` | Thành công, xác nhận |
| Warning | `--color-warning` | `#D97706` | Cảnh báo |

**site.css — khai báo CSS Variables:**
```css
:root {
    --color-primary: #FF5000;
    --color-primary-hover: #E04500;
    --color-primary-light: #FFF0EB;
    --color-secondary: #6c757d;
    --color-background: #F8F9FA;
    --color-surface: #FFFFFF;
    --color-text-primary: #1A1A1A;
    --color-text-secondary: #6B7280;
    --color-border: #E5E7EB;
    --color-error: #DC2626;
    --color-success: #16A34A;
    --color-warning: #D97706;
}

---

## 3. Design Tokens — Typography

| Token | Giá trị | Mô tả |
|-------|---------|-------|
| `--font-family` | `'Inter', sans-serif` | Font chính toàn app |
| `--font-size-xs` | `12px` | Caption, label nhỏ |
| `--font-size-sm` | `14px` | Body nhỏ, meta info |
| `--font-size-base` | `16px` | Body text mặc định |
| `--font-size-lg` | `18px` | Subheading |
| `--font-size-xl` | `20px` | Section heading |
| `--font-size-2xl` | `24px` | Page heading |
| `--font-weight-normal` | `400` | Văn bản thường |
| `--font-weight-medium` | `500` | Label, button |
| `--font-weight-semibold` | `600` | Heading, emphasis |
| `--font-weight-bold` | `700` | Hero text, CTA |
| `--line-height-base` | `1.5` | Body text |
| `--line-height-tight` | `1.25` | Heading |

---

## 4. Design Tokens — Spacing & Layout

| Token | Giá trị | Mô tả |
|-------|---------|-------|
| `--spacing-1` | `4px` | Khoảng cách nhỏ nhất |
| `--spacing-2` | `8px` | |
| `--spacing-3` | `12px` | |
| `--spacing-4` | `16px` | Khoảng cách cơ bản |
| `--spacing-5` | `20px` | |
| `--spacing-6` | `24px` | Card padding |
| `--spacing-8` | `32px` | Section gap |
| `--spacing-10` | `40px` | |
| `--spacing-12` | `48px` | Large section |
| `--border-radius-sm` | `8px` | Input, badge |
| `--border-radius-base` | `12px` | Card, modal |
| `--border-radius-lg` | `16px` | Large card |
| `--border-radius-full` | `9999px` | Pill, avatar |

---

## 5. Breakpoints (Responsive)

| Tên | Giá trị | Mô tả |
|-----|---------|-------|
| `sm` | `640px` | Mobile large |
| `md` | `768px` | Tablet |
| `lg` | `1024px` | **Breakpoint chính** — mobile vs desktop |
| `xl` | `1280px` | Desktop |
| `2xl` | `1536px` | Wide screen |

> **Quy tắc**: Breakpoint `lg` là điểm chính phân chia mobile và desktop layout.

---

## 6. UI Components — Trạng thái bắt buộc

Mọi component hiển thị dữ liệu bất đồng bộ **PHẢI** có đủ 3 trạng thái:

### Loading State
- **Ưu tiên Skeleton** cho layout ổn định (card grid, profile, article list).
- Spinner chỉ dùng cho button, inline action nhỏ.
- Skeleton phải phản ánh đúng cấu trúc nội dung thực để tránh Layout Shift.

### Empty State
- Hiển thị thông điệp rõ ràng tại sao trống.
- Luôn có CTA gợi ý hành động tiếp theo.
- Không hiển thị lỗi khi dữ liệu trống là trạng thái hợp lệ.

### Error State
- Thông báo lỗi thân thiện — không lộ stack trace hoặc thông tin kỹ thuật.
- Có nút "Thử lại" hoặc hướng dẫn xử lý.
- Log lỗi đúng cách phía backend (không silent fail).

---

## 7. UI Components — Danh sách

| Component | Trạng thái | Ghi chú |
|-----------|-----------|---------|
| Button | Primary, Secondary, Danger, Disabled, Loading | Loading state dùng spinner nhỏ trong button |
| Input | Default, Focus, Error, Disabled | Error message hiển thị bên dưới input |
| Card | Default, Hover | Border-radius 12px, shadow nhẹ |
| Modal | — | Backdrop blur, border-radius 16px |
| Skeleton | — | Màu `#E5E7EB`, animated shimmer |
| Badge | Success, Warning, Error, Neutral | Pill shape |
| Toast/Notification | Success, Error, Warning, Info | Timeout 4s mặc định |

---

## 8. Quy tắc phối hợp

- **Senior Frontend** phải đọc và tuân thủ file này trước khi implement bất kỳ component nào.
- **Senior UI/UX Designer** phải cập nhật file này khi có thay đổi về màu sắc, spacing hoặc component mới.
- Khi có mâu thuẫn giữa file này và mockup, ưu tiên **file này** và yêu cầu Designer cập nhật.

---

## 9. Cấu trúc `wwwroot/` (ASP.NET MVC)

```
wwwroot/
├── css/
│   ├── site.css            # CSS chính — CSS Variables + override Bootstrap
│   └── {module}.css        # CSS riêng theo module (membership.css, admin.css)
├── js/
│   ├── site.js             # JS chung toàn app
│   └── {module}.js         # JS riêng theo page/module
└── lib/
    ├── bootstrap/          # Bootstrap 5 (cài qua LibMan)
    ├── jquery/             # jQuery
    └── jquery-validation/  # jQuery Validation + Unobtrusive
```

**LibMan config (`libman.json`):**
```json
{
  "version": "1.0",
  "defaultProvider": "cdnjs",
  "libraries": [
    {
      "library": "bootstrap@5.3.3",
      "destination": "wwwroot/lib/bootstrap/"
    },
    {
      "library": "jquery@3.7.1",
      "destination": "wwwroot/lib/jquery/"
    },
    {
      "library": "jquery-validation@1.21.0",
      "destination": "wwwroot/lib/jquery-validation/"
    },
    {
      "library": "jquery-validation-unobtrusive@4.0.0",
      "destination": "wwwroot/lib/jquery-validation-unobtrusive/"
    }
  ]
}
```

## 10. Bootstrap Customization — Override với CSS Variables

```css
/* wwwroot/css/site.css */

/* 1. Khai báo CSS Variables (Design Tokens) */
:root {
    --color-primary: #FF5000;
    --bs-primary: #FF5000;       /* Override Bootstrap primary */
    --bs-primary-rgb: 255, 80, 0;
    --color-surface: #FFFFFF;
    --color-background: #F8F9FA;
    --color-border: #E5E7EB;
    --spacing-base: 16px;
}

/* 2. Override Bootstrap component — dùng CSS Variables */
.btn-primary {
    background-color: var(--color-primary);
    border-color: var(--color-primary);
}
.btn-primary:hover {
    background-color: var(--color-primary-hover);
    border-color: var(--color-primary-hover);
}

/* 3. Custom component */
.card-membership {
    background: var(--color-surface);
    border: 1px solid var(--color-border);
    border-radius: 12px;
    padding: var(--spacing-base);
    transition: box-shadow 0.2s ease;
}
.card-membership:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}
```
