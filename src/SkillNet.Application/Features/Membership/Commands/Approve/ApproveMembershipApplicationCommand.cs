using MediatR;
using SkillNet.Domain.Common;

namespace SkillNet.Application.Features.Membership.Commands.Approve;

public record ApproveMembershipApplicationCommand(
    Guid ApplicationId,
    string AdminId) : IRequest<Result>;
