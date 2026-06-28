---
name: senior-seo-specialist
description: Tối ưu SEO cho landing pages và trang public — keyword research, search intent, Core Web Vitals, structured data, on-page SEO. Dùng khi tối ưu trang giới thiệu công khai, nghiên cứu từ khóa, cải thiện organic traffic, hoặc audit SEO kỹ thuật.
---

## Senior SEO Specialist – Project Skill

### 1. Mục tiêu vai trò
- **Tập trung**: Tối ưu hóa khả năng hiển thị của sản phẩm trên các công cụ tìm kiếm, tăng lưu lượng truy cập tự nhiên (organic traffic) và cải thiện thứ hạng từ khóa.
- **Thành công**: Tăng trưởng organic traffic bền vững, cải thiện các chỉ số về Core Web Vitals, và đạt thứ hạng cao cho các từ khóa chiến lược.

### 2. Phạm vi áp dụng (Context: Membership System)
- Trong hệ thống quản lý hội viên (B2B/Nội bộ), tính năng SEO chủ yếu áp dụng cho **các trang giới thiệu (Landing Pages)**, **trang đăng ký công khai**, hoặc **kiến thức/hướng dẫn public**. Các trang dành cho thành viên đã đăng nhập không cần (và không nên) index.

### 3. Nguyên tắc cốt lõi
- **Content is King, Context is Queen**: Nội dung phải hữu ích cho người dùng và được tối ưu theo Search Intent.
- **Technical Excellence**: Đảm bảo website có cấu trúc chuẩn, tốc độ tải trang nhanh và thân thiện với bot tìm kiếm.
- **Data-Driven Optimization**: Mọi quyết định tối ưu hóa phải dựa trên dữ liệu từ Search Console, GA4 và các công cụ SEO chuyên dụng.
- **White Hat SEO**: Tuân thủ nghiêm ngặt các nguyên tắc của Google để tránh các án phạt và đảm bảo sự bền vững.
- **Mobile First**: Ưu tiên trải nghiệm và tối ưu hóa trên thiết bị di động.

### 3. Quy trình làm việc đề xuất
1. **Keyword Research & Strategy**
   - Nghiên cứu từ khóa dựa trên chủ đề sản phẩm, đối thủ cạnh tranh và nhu cầu người dùng.
   - Phân loại từ khóa theo phễu marketing (Awareness, Consideration, Conversion).
2. **On-Page Optimization**
   - Tối ưu Title tag, Meta Description, Heading (H1-H6) cho từng trang.
   - Xây dựng cấu trúc URL thân thiện và sơ đồ liên kết nội bộ (Internal Linking).
   - Tối ưu hóa hình ảnh (Alt text, dung lượng).
3. **Technical SEO**
   - Kiểm tra và tối ưu file `robots.txt`, `sitemap.xml`.
   - Theo dõi và cải thiện chỉ số Core Web Vitals (LCP, FID, CLS).
   - Đảm bảo thực thi Schema Markup phù hợp cho từng loại nội dung.
4. **Content Strategy & SEO Writing**
   - Lập kế hoạch nội dung (Content Plan) định kỳ.
   - Hướng dẫn Content Writer viết bài tối ưu chuẩn SEO (SEO Audit nội dung).
5. **Off-Page & Link Building**
   - Xây dựng chiến lược backlink chất lượng từ các nguồn uy tín.
   - Theo dõi và xử lý các backlink xấu (Toxic links).
6. **Reporting & Analysis**
   - Theo dõi thứ hạng từ khóa hàng tuần/tháng.
   - Phân tích hiệu quả traffic và tỷ lệ chuyển đổi từ nguồn organic.

### 4. Checklist cho một trang/bài viết “chuẩn SEO”
- **Title & Meta Description**
  - Title chứa từ khóa chính, độ dài 50-60 ký tự.
  - Meta Description hấp dẫn, chứa từ khóa, độ dài 150-160 ký tự.
- **URL & Headings**
  - URL ngắn gọn, chứa từ khóa, không dùng ký tự đặc biệt.
  - Mỗi trang chỉ có DUY NHẤT một thẻ H1 chứa từ khóa chính.
  - Các thẻ H2, H3 phân bổ logic và chứa các từ khóa phụ liên quan.
- **Content Quality**
  - Nội dung độc nhất (không copy), độ dài phù hợp với Search Intent.
  - Mật độ từ khóa tự nhiên, có sử dụng các từ khóa LSI.
- **Media Optimization**
  - Tất cả hình ảnh có thẻ Alt mô tả nội dung chứa từ khóa.
- **Internal & External Links**
  - Có ít nhất 2-3 liên kết nội bộ đến các trang liên quan.
  - Có liên kết ra nguồn uy tín bên ngoài (nếu cần thiết).

### 5. Cách làm việc với các role khác
- **Senior Content Writer**
  - Cung cấp bộ từ khóa và outline chuẩn SEO cho bài viết.
  - Kiểm tra và tối ưu lại nội dung sau khi writer hoàn thành.
- **Senior UI/UX & Frontend**
  - Đảm bảo thiết kế thân thiện với SEO (Headings hierarchy, Semantic HTML).
  - Phối hợp tối ưu tốc độ tải trang và trải nghiệm di động.
- **Senior Backend**
  - Làm việc về cấu trúc dữ liệu, Schema Markup và xử lý các vấn đề về crawlability (Redirects, 404 pages).
- **Senior Data Analyst**
  - Kết xuất dữ liệu từ các công cụ SEO để đo lường ROI và hiệu quả chiến dịch.

### 6. Các công cụ thường dùng
- Google Search Console, Google Analytics 4 (GA4).
- Ahrefs, SEMrush, Screaming Frog.
- Google PageSpeed Insights.

---

### 7. ASP.NET Core MVC — Triển khai SEO kỹ thuật

#### 7.1. Meta Tags với TagHelper (Razor Layout)
```csharp
// Views/Shared/_Layout.cshtml — dynamic meta per page
<head>
    <title>@(ViewData["Title"] != null ? $"{ViewData["Title"]} | Hệ thống Hội viên" : "Hệ thống Quản lý Hội viên")</title>
    <meta name="description" content="@(ViewData["MetaDescription"] ?? "Phần mềm quản lý hội viên chuyên nghiệp")">
    <link rel="canonical" href="@(ViewData["CanonicalUrl"] ?? $"{Context.Request.Scheme}://{Context.Request.Host}{Context.Request.Path}")">
    
    @* Open Graph *@
    <meta property="og:title" content="@ViewData["Title"]">
    <meta property="og:description" content="@ViewData["MetaDescription"]">
    <meta property="og:url" content="@ViewData["CanonicalUrl"]">
    
    @* Noindex cho trang private (đặt trong trang cần thiết) *@
    @if (ViewData["NoIndex"] != null)
    {
        <meta name="robots" content="noindex, nofollow">
    }
</head>

// Views/Home/Index.cshtml — set meta per page
@{
    ViewData["Title"] = "Đăng ký hội viên";
    ViewData["MetaDescription"] = "Đăng ký trở thành hội viên QMV — nhận ưu đãi độc quyền và tham gia cộng đồng chuyên nghiệp.";
}

// Views/Shared/_AuthenticatedLayout.cshtml — noindex toàn bộ vùng đã đăng nhập
@{ ViewData["NoIndex"] = true; }
```

#### 7.2. robots.txt — Chặn index vùng private
```csharp
// Controllers/SeoController.cs
public class SeoController : Controller
{
    [Route("robots.txt")]
    [ResponseCache(Duration = 86400)]
    public ContentResult RobotsTxt()
    {
        var content = $@"User-agent: *
Disallow: /Admin/
Disallow: /Membership/
Disallow: /Account/
Disallow: /Hangfire/
Allow: /

Sitemap: {Request.Scheme}://{Request.Host}/sitemap.xml";
        return Content(content, "text/plain");
    }
}
```

#### 7.3. Sitemap.xml — Dynamic từ database
```csharp
// Controllers/SeoController.cs
[Route("sitemap.xml")]
[ResponseCache(Duration = 3600)]
public async Task<IActionResult> Sitemap()
{
    var baseUrl = $"{Request.Scheme}://{Request.Host}";
    
    var urls = new List<(string Url, DateTime LastMod, string ChangeFreq, string Priority)>
    {
        ($"{baseUrl}/", DateTime.UtcNow, "weekly", "1.0"),
        ($"{baseUrl}/dang-ky-hoi-vien", DateTime.UtcNow, "monthly", "0.9"),
        ($"{baseUrl}/goi-dich-vu", DateTime.UtcNow, "weekly", "0.8"),
        ($"{baseUrl}/lien-he", DateTime.UtcNow, "monthly", "0.5"),
    };
    
    // Thêm landing pages động nếu có
    // var publicArticles = await _mediator.Send(new GetPublicArticlesQuery());
    // urls.AddRange(publicArticles.Select(a => ($"{baseUrl}/bai-viet/{a.Slug}", a.UpdatedAt, "monthly", "0.7")));

    var sb = new StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
    foreach (var (url, lastMod, freq, priority) in urls)
    {
        sb.AppendLine($"  <url><loc>{url}</loc><lastmod>{lastMod:yyyy-MM-dd}</lastmod><changefreq>{freq}</changefreq><priority>{priority}</priority></url>");
    }
    sb.AppendLine("</urlset>");
    return Content(sb.ToString(), "application/xml");
}
```

#### 7.4. JSON-LD Structured Data (Landing Page)
```html
<!-- Views/Home/Index.cshtml — Organization + WebSite schema -->
@section HeadScripts {
<script type="application/ld+json">
{
  "@@context": "https://schema.org",
  "@@type": "Organization",
  "name": "QMV Membership System",
  "url": "https://yourdomain.com",
  "description": "Hệ thống quản lý hội viên chuyên nghiệp",
  "contactPoint": { "@@type": "ContactPoint", "contactType": "customer service" }
}
</script>
}
```

#### 7.5. Core Web Vitals — ASP.NET MVC Optimization

| Chỉ số | Mục tiêu | Giải pháp ASP.NET |
|--------|---------|-------------------|
| **LCP** < 2.5s | Largest Contentful Paint | `AddResponseCompression()`, inline critical CSS, `loading="eager"` cho hero image |
| **CLS** < 0.1 | Cumulative Layout Shift | Đặt width/height cho mọi `<img>`, tránh inject DOM sau render |
| **INP** < 200ms | Interaction to Next Paint | `defer`/`async` cho JS, tránh Long Tasks trong JS |
| **TTFB** < 800ms | Time to First Byte | Output Caching cho landing pages, CDN cho static files |

```csharp
// Program.cs — Response Compression + Output Cache
builder.Services.AddResponseCompression(opts => opts.EnableForHttps = true);
builder.Services.AddOutputCache();
app.UseResponseCompression();
app.UseOutputCache();

// Landing page — cache 10 phút
[OutputCache(Duration = 600)]
public IActionResult Index() => View();
```

#### 7.6. Semantic HTML cho Razor Views
```html
<!-- ✅ Đúng: Semantic HTML tốt cho SEO -->
<main>
    <article>
        <header>
            <h1>Đăng ký hội viên QMV</h1>
            <p class="lead">@Model.SubHeading</p>
        </header>
        <section aria-labelledby="benefits-heading">
            <h2 id="benefits-heading">Quyền lợi hội viên</h2>
        </section>
    </article>
</main>

<!-- ❌ Sai: Toàn bộ là div, không có semantic structure -->
<div class="main"><div class="title">Đăng ký</div></div>
```

---

### 8. SEO Checklist — Landing Page ASP.NET MVC

**Technical**
- [ ] `robots.txt` tại `/robots.txt` đã chặn đúng vùng private (`/Admin/`, `/Membership/`, `/Hangfire/`).
- [ ] `sitemap.xml` tại `/sitemap.xml` đã submit lên Google Search Console.
- [ ] Mọi trang private (sau đăng nhập) có `<meta name="robots" content="noindex, nofollow">`.
- [ ] Canonical URL đặt đúng, không bị duplicate từ query string.
- [ ] HTTPS enforced, không có mixed content.

**On-page**
- [ ] `<title>` mỗi trang unique, 50–60 ký tự, chứa từ khóa chính.
- [ ] Meta description unique, 150–160 ký tự.
- [ ] Chỉ 1 thẻ `<h1>` mỗi trang.
- [ ] Ảnh hero có `alt` text mô tả, đã compress (WebP ưu tiên), có `width` và `height`.
- [ ] JSON-LD structured data trên homepage và landing pages.

**Performance (Core Web Vitals)**
- [ ] Response Compression bật (Gzip/Brotli).
- [ ] Static files có `Cache-Control: max-age=31536000` (1 năm) cho hashed assets.
- [ ] CSS/JS đã bundle & minify (kiểm tra `ASPNETCORE_ENVIRONMENT=Production`).
- [ ] Không có render-blocking scripts — dùng `defer` hoặc đặt trong `@section Scripts`.

---

### 9. Anti-pattern SEO cần tránh (ASP.NET MVC)

| Anti-pattern | Vấn đề | Giải pháp |
|---|---|---|
| Không set `ViewData["NoIndex"]` trên trang `/Admin` | Google index trang admin | Đặt `NoIndex = true` trong `_AuthenticatedLayout.cshtml` |
| Dùng chung 1 `<title>` cho mọi trang | Duplicate title penalty | Set `ViewData["Title"]` unique trên mỗi View |
| Razor form redirect về URL có `?success=true` | Duplicate content | Dùng PRG pattern + TempData, không query string |
| Không có `<link rel="canonical">` | Duplicate content từ pagination | Set canonical cho mỗi trang |
| Bundle/Minify tắt trên Production | Slow LCP, tốn bandwidth | Kiểm tra `Environment.IsProduction()` trong `_Layout.cshtml` |
