using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkillNet.Web.ViewModels;

public class MembershipRegisterFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [MaxLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
    [Display(Name = "Ngày sinh")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số, bắt đầu bằng 0.")]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [MaxLength(300)]
    [Display(Name = "Địa chỉ")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn gói dịch vụ.")]
    [Display(Name = "Gói dịch vụ")]
    public Guid ServicePackageId { get; set; }

    [MaxLength(500)]
    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }

    [Display(Name = "CMND/CCCD (tùy chọn)")]
    public IFormFile? IdDocumentFile { get; set; }

    public List<SelectListItem> AvailablePackages { get; set; } = [];
}
