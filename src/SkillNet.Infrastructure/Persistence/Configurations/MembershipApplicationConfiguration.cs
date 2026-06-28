using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillNet.Domain.Entities;
using SkillNet.Domain.Enums;

namespace SkillNet.Infrastructure.Persistence.Configurations;

public class MembershipApplicationConfiguration : IEntityTypeConfiguration<MembershipApplication>
{
    public void Configure(EntityTypeBuilder<MembershipApplication> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId).IsRequired().HasMaxLength(450);
        builder.Property(a => a.FullName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(15);
        builder.Property(a => a.Address).IsRequired().HasMaxLength(300);
        builder.Property(a => a.Notes).HasMaxLength(500);
        builder.Property(a => a.IdDocumentPath).HasMaxLength(500);
        builder.Property(a => a.RejectionReason).HasMaxLength(1000);
        builder.Property(a => a.MembershipCode).HasMaxLength(30);
        builder.Property(a => a.ProcessedByAdminId).HasMaxLength(450);

        builder.Property(a => a.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ApplicationStatus>(v))
            .HasMaxLength(20);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => new { a.UserId, a.Status });

        builder.HasIndex(a => a.UserId)
            .HasFilter("Status = 'Pending' AND IsDeleted = 0")
            .IsUnique()
            .HasDatabaseName("UX_MembershipApplications_OnePendingPerUser");

        builder.HasOne(a => a.ServicePackage)
            .WithMany()
            .HasForeignKey(a => a.ServicePackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(a => a.DomainEvents);
    }
}
