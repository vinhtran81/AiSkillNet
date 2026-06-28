using FluentValidation;

namespace SkillNet.Application.Features.Membership.Commands.Reject;

public class RejectMembershipApplicationValidator : AbstractValidator<RejectMembershipApplicationCommand>
{
    public RejectMembershipApplicationValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Vui lòng nhập lý do từ chối.")
            .MaximumLength(1000);
    }
}
