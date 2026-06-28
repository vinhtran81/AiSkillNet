using MediatR;
using SkillNet.Domain.Common;

namespace SkillNet.Application.Features.Membership.Commands.Reject;

public record RejectMembershipApplicationCommand(
    Guid ApplicationId,
    string AdminId,
    string RejectionReason) : IRequest<Result>;
