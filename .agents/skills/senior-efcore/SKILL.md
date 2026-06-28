---
name: senior-efcore
description: Thiết kế Data Access Layer với Entity Framework Core + SQL Server — Fluent API configuration, Migrations, Repository Pattern, query optimization (AsNoTracking, projection), Dapper cho query phức tạp. Dùng khi viết migration, tối ưu EF Core query, hoặc thiết kế database schema.
---

## Senior EF Core / Database – Project Skill

### 1. Mục tiêu vai trò
- **Tập trung**: Thiết kế và vận hành Data Access Layer sử dụng **Entity Framework Core** trên **SQL Server**, đảm bảo schema nhất quán qua Migrations, query hiệu quả và data integrity tối đa.
- **Thành công**: Không có N+1 query, Migration rõ ràng và an toàn, Repository Pattern nhất quán, không có raw SQL cộng chuỗi.

### 2. DbContext Setup chuẩn

```csharp
// Infrastructure/Persistence/AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<MembershipApplication> MembershipApplications => Set<MembershipApplication>();
    public DbSet<ServicePackage> ServicePackages => Set<ServicePackage>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Auto-apply tất cả IEntityTypeConfiguration trong assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    
    // Tự động set CreatedAt / UpdatedAt
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### 3. Entity Configuration (Fluent API)

```csharp
// Infrastructure/Persistence/Configurations/MembershipApplicationConfiguration.cs
public class MembershipApplicationConfiguration : IEntityTypeConfiguration<MembershipApplication>
{
    public void Configure(EntityTypeBuilder<MembershipApplication> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(x => x.Status)
            .HasConversion<string>()  // Enum → VARCHAR
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");
        
        // Index cho các cột query thường xuyên
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.Status, x.CreatedAt });  // Composite index
        
        builder.ToTable("MembershipApplications");
    }
}
```

### 4. Migration Strategy

#### Tạo Migration
```bash
# Đặt tên migration rõ nghĩa (động từ + danh từ)
dotnet ef migrations add AddMembershipApplicationTable \
  --project src/AppName.Infrastructure \
  --startup-project src/AppName.Web \
  --output-dir Persistence/Migrations

# Review SQL script trước khi apply
dotnet ef migrations script --idempotent \
  --project src/AppName.Infrastructure \
  --startup-project src/AppName.Web

# Apply lên môi trường
dotnet ef database update \
  --project src/AppName.Infrastructure \
  --startup-project src/AppName.Web
```

#### Quy tắc Migration (bắt buộc)
- **Không sửa** file migration đã được apply lên môi trường dùng chung.
- **Luôn review** SQL script trước khi apply lên Staging/Production.
- **Không DROP** column/table trong cùng migration với việc migrate data — tách thành 2 migration.
- Đặt tên migration **mô tả hành động**: `AddMembershipApprovalDateColumn`, `CreateServicePackagesTable`.
- Với data migration phức tạp: viết idempotent SQL script riêng trong thư mục `.db/seeds/`.

### 5. Repository Pattern

```csharp
// Application/Interfaces/IRepository.cs
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

// Application/Interfaces/IMembershipApplicationRepository.cs  
public interface IMembershipApplicationRepository : IRepository<MembershipApplication>
{
    Task<PagedList<MembershipApplication>> GetPagedAsync(
        ApplicationStatus? status, int page, int pageSize, CancellationToken ct = default);
    Task<MembershipApplication?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsActiveApplicationAsync(string email, CancellationToken ct = default);
}

// Infrastructure/Persistence/Repositories/MembershipApplicationRepository.cs
public class MembershipApplicationRepository : IMembershipApplicationRepository
{
    private readonly AppDbContext _context;
    
    public MembershipApplicationRepository(AppDbContext context)
        => _context = context;
    
    public async Task<MembershipApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.MembershipApplications
            .Include(x => x.ServicePackage)  // Eager load cần thiết
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    
    public async Task<PagedList<MembershipApplication>> GetPagedAsync(
        ApplicationStatus? status, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.MembershipApplications
            .AsNoTracking()  // Read-only query — không cần tracking
            .Where(x => !status.HasValue || x.Status == status.Value)
            .OrderByDescending(x => x.CreatedAt);
        
        return await PagedList<MembershipApplication>.CreateAsync(query, page, pageSize, ct);
    }
    
    public void Add(MembershipApplication entity)
        => _context.MembershipApplications.Add(entity);
    
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
```

### 6. Tránh N+1 Query

```csharp
// ❌ SAI: N+1 query
var applications = await _context.MembershipApplications.ToListAsync();
foreach (var app in applications)
{
    var package = await _context.ServicePackages.FindAsync(app.ServicePackageId); // N queries!
}

// ✅ ĐÚNG: Eager loading
var applications = await _context.MembershipApplications
    .Include(x => x.ServicePackage)
    .ToListAsync();

// ✅ ĐÚNG: Select projection — chỉ lấy field cần thiết
var dtos = await _context.MembershipApplications
    .AsNoTracking()
    .Where(x => x.Status == ApplicationStatus.Pending)
    .Select(x => new ApplicationSummaryDto
    {
        Id = x.Id,
        FullName = x.FullName,
        Email = x.Email,
        PackageName = x.ServicePackage.Name  // Join trong 1 query
    })
    .ToListAsync();
```

### 7. Dapper cho Query Phức Tạp

```csharp
// Khi EF Core LINQ quá phức tạp hoặc cần stored procedure
public class ReportRepository
{
    private readonly IDbConnection _connection;
    
    public async Task<IEnumerable<MembershipReportDto>> GetMonthlyReportAsync(int year, int month)
    {
        const string sql = @"
            SELECT 
                sp.Name AS ServiceName,
                COUNT(*) AS TotalApplications,
                SUM(CASE WHEN ma.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
                SUM(ma.Amount) AS TotalAmount
            FROM MembershipApplications ma
            INNER JOIN ServicePackages sp ON ma.ServicePackageId = sp.Id
            WHERE YEAR(ma.CreatedAt) = @Year AND MONTH(ma.CreatedAt) = @Month
            GROUP BY sp.Id, sp.Name
            ORDER BY TotalAmount DESC";
        
        return await _connection.QueryAsync<MembershipReportDto>(sql, new { Year = year, Month = month });
    }
}
```

### 8. Connection Resilience (Polly)

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));
```

### 9. Performance Checklist EF Core

- [ ] Dùng `AsNoTracking()` cho tất cả read-only query.
- [ ] Không gọi `.ToList()` sớm trước khi áp filter/select.
- [ ] Luôn dùng `Select()` hoặc `Include()` thay vì `SELECT *`.
- [ ] Không có loop gọi DB bên trong: batch với `.Contains()` hoặc `.Join()`.
- [ ] Các cột dùng trong `WHERE`, `ORDER BY`, `JOIN` có Index phù hợp.
- [ ] Kiểm tra SQL sinh ra bằng `EnableSensitiveDataLogging()` trên Development.
- [ ] Sử dụng `ExecuteUpdateAsync` / `ExecuteDeleteAsync` (EF Core 7+) cho bulk operations.

### 10. Seed Data chuẩn

```csharp
// Infrastructure/Persistence/Seed/AppDbContextSeed.cs
public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!context.ServicePackages.Any())
        {
            logger.LogInformation("Seeding service packages...");
            context.ServicePackages.AddRange(GetPreconfiguredServicePackages());
            await context.SaveChangesAsync();
        }
    }
    
    private static IEnumerable<ServicePackage> GetPreconfiguredServicePackages() => new[]
    {
        new ServicePackage { Id = Guid.NewGuid(), Code = "PKG001", Name = "Gói Cơ Bản", Price = 500_000m },
        new ServicePackage { Id = Guid.NewGuid(), Code = "PKG002", Name = "Gói Nâng Cao", Price = 1_200_000m },
    };
}
```
