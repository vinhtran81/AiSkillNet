using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillNet.Application.Features.Membership.Commands.Approve;
using SkillNet.Application.Features.Membership.Commands.Reject;
using SkillNet.Application.Features.Membership.Queries.GetApplicationDetail;
using SkillNet.Application.Features.Membership.Queries.GetPendingApplications;
using SkillNet.Web.ViewModels;
using System.Security.Claims;

namespace SkillNet.Web.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Membership/[action]/{id?}")]
public class AdminMembershipController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Pending(int page = 1)
    {
        var result = await mediator.Send(new GetPendingApplicationsQuery(page));
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var dto = await mediator.Send(new GetApplicationDetailQuery(id));
        if (dto is null) return NotFound();

        ViewBag.RejectForm = new RejectApplicationFormModel();
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await mediator.Send(new ApproveMembershipApplicationCommand(id, adminId));

        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Hội viên đã được phê duyệt thành công."
            : result.Error;

        return RedirectToAction(nameof(Pending));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, RejectApplicationFormModel form)
    {
        if (!ModelState.IsValid)
        {
            var dto = await mediator.Send(new GetApplicationDetailQuery(id));
            if (dto is null) return NotFound();
            ViewBag.RejectForm = form;
            return View("Detail", dto);
        }

        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await mediator.Send(new RejectMembershipApplicationCommand(id, adminId, form.RejectionReason));

        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Đơn đăng ký đã bị từ chối."
            : result.Error;

        return RedirectToAction(nameof(Pending));
    }
}
