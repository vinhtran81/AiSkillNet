using MediatR;
using SkillNet.Domain.Common;

namespace SkillNet.Application.Features.Membership.Commands.Cancel;

public record CancelMembershipApplicationCommand(
    Guid ApplicationId,
    string UserId) : IRequest<Result>;
