using FluentValidation;
using SkillNet.Domain.Interfaces;

namespace SkillNet.Application.Features.Membership.Commands.Submit;

public class SubmitMembershipApplicationValidator : AbstractValidator<SubmitMembershipApplicationCommand>
{
    public SubmitMembershipApplicationValidator(IServicePackageRepository packageRepo)
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Vui lòng nhập họ và tên.")
            .MinimumLength(2).WithMessage("Họ và tên phải có ít nhất 2 ký tự.")
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(dob => DateTime.UtcNow.Year - dob.Year >= 16)
            .WithMessage("Bạn phải đủ 16 tuổi để đăng ký.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^0\d{9}$").WithMessage("Số điện thoại gồm 10 chữ số, bắt đầu bằng 0.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Vui lòng nhập địa chỉ.")
            .MinimumLength(5).MaximumLength(300);

        RuleFor(x => x.ServicePackageId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await packageRepo.IsActiveAsync(id, ct))
            .WithMessage("Gói dịch vụ không hợp lệ hoặc không còn hoạt động.");

        When(x => x.Notes != null, () =>
            RuleFor(x => x.Notes).MaximumLength(500));

        When(x => x.IdDocumentFile != null, () =>
        {
            RuleFor(x => x.IdDocumentFile!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("File không được vượt quá 5MB.");
            RuleFor(x => x.IdDocumentFile!.ContentType)
                .Must(ct => new[] { "image/jpeg", "image/png", "application/pdf" }.Contains(ct))
                .WithMessage("Chỉ chấp nhận file jpg, png hoặc pdf.");
        });
    }
}
