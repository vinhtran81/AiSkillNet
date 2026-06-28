using Microsoft.AspNetCore.Http;
using SkillNet.Domain.Common;

namespace SkillNet.Application.Features.Membership.Commands.Submit;

public record SubmitMembershipApplicationCommand(
    string UserId,
    string UserEmail,
    string UserName,
    Guid ServicePackageId,
    string FullName,
    DateOnly DateOfBirth,
    string PhoneNumber,
    string Address,
    string? Notes,
    IFormFile? IdDocumentFile,
    string PackageName) : MediatR.IRequest<Result<Guid>>;
