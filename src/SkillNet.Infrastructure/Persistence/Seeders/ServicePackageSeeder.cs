using Microsoft.EntityFrameworkCore;
using SkillNet.Domain.Entities;

namespace SkillNet.Infrastructure.Persistence.Seeders;

public static class ServicePackageSeeder
{
    public static async Task SeedAsync(SkillNetDbContext db)
    {
        if (await db.ServicePackages.AnyAsync()) return;

        db.ServicePackages.AddRange(
            new ServicePackage
            {
                Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                Name = "Tiêu chuẩn",
                Description = "Phù hợp cho người mới bắt đầu",
                Price = 500_000m,
                DurationMonths = 12,
                Benefits = """["Truy cập thư viện cơ bản","Tham gia 2 lớp học/tháng","Hỗ trợ qua email"]""",
                IsActive = true,
                DisplayOrder = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ServicePackage
            {
                Id = Guid.Parse("22222222-0000-0000-0000-000000000002"),
                Name = "Nâng cao",
                Description = "Dành cho học viên tích cực",
                Price = 1_200_000m,
                DurationMonths = 12,
                Benefits = """["Toàn bộ quyền lợi Tiêu chuẩn","Tham gia không giới hạn lớp học","Mentoring 1-1 mỗi quý","Chứng chỉ hoàn thành"]""",
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ServicePackage
            {
                Id = Guid.Parse("33333333-0000-0000-0000-000000000003"),
                Name = "VIP",
                Description = "Trải nghiệm cao cấp nhất",
                Price = 3_000_000m,
                DurationMonths = 12,
                Benefits = """["Toàn bộ quyền lợi Nâng cao","Mentoring 1-1 hàng tháng","Ưu tiên đăng ký khoá học mới","Sự kiện networking độc quyền","Hỗ trợ ưu tiên 24/7"]""",
                IsActive = true,
                DisplayOrder = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        await db.SaveChangesAsync();
    }
}
