using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillNet.Domain.Entities;
using SkillNet.Infrastructure.Identity;
using SkillNet.Infrastructure.Persistence.Configurations;

namespace SkillNet.Infrastructure.Persistence;

public class SkillNetDbContext(DbContextOptions<SkillNetDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<MembershipApplication> MembershipApplications => Set<MembershipApplication>();
    public DbSet<ServicePackage> ServicePackages => Set<ServicePackage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new MembershipApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new ServicePackageConfiguration());
    }
}
