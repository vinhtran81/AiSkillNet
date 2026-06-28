using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillNet.Application.Features.Membership.Queries.GetPendingCount;

namespace SkillNet.Web.ViewComponents;

public class PendingApprovalsBadgeViewComponent(IMediator mediator) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var count = await mediator.Send(new GetPendingCountQuery());
        return View(count);
    }
}
