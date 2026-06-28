using SkillNet.Domain.Enums;

namespace SkillNet.Web.ViewModels;

public class ApplicationStatusViewModel
{
    public bool HasApplication { get; set; }
    public Guid? ApplicationId { get; set; }
    public ApplicationStatus? Status { get; set; }
    public string? ServicePackageName { get; set; }
    public decimal? ServicePackagePrice { get; set; }
    public string? MembershipCode { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? RejectionReason { get; set; }
    public int DaysPending { get; set; }
}
