using MediatR;

namespace SkillNet.Application.Features.Membership.Queries.GetPendingCount;

public record GetPendingCountQuery : IRequest<int>;
