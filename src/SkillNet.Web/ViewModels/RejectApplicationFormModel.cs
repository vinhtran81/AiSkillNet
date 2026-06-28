using System.ComponentModel.DataAnnotations;

namespace SkillNet.Web.ViewModels;

public class RejectApplicationFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập lý do từ chối.")]
    [MaxLength(1000)]
    [Display(Name = "Lý do từ chối")]
    public string RejectionReason { get; set; } = string.Empty;
}
