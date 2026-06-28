using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkillNet.Application.Features.Membership.Commands.Cancel;
using SkillNet.Application.Features.Membership.Commands.Submit;
using SkillNet.Application.Features.Membership.Queries.GetActivePackages;
using SkillNet.Application.Features.Membership.Queries.GetMyApplicationStatus;
using SkillNet.Web.ViewModels;
using System.Security.Claims;

namespace SkillNet.Web.Controllers;

[Authorize]
public class MembershipController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Register(Guid? packageId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existing = await mediator.Send(new GetMyApplicationStatusQuery(userId));
        if (existing is not null)
            return RedirectToAction(nameof(Status));

        var form = new MembershipRegisterFormModel
        {
            ServicePackageId = packageId ?? Guid.Empty,
            AvailablePackages = await LoadPackageSelectListAsync()
        };
        return View(form);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(MembershipRegisterFormModel form)
    {
        if (!ModelState.IsValid)
        {
            form.AvailablePackages = await LoadPackageSelectListAsync();
            return View(form);
        }

        var cmd = new SubmitMembershipApplicationCommand(
            UserId: User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            UserEmail: User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            UserName: User.FindFirstValue(ClaimTypes.Name) ?? form.FullName,
            ServicePackageId: form.ServicePackageId,
            FullName: form.FullName,
            DateOfBirth: form.DateOfBirth!.Value,
            PhoneNumber: form.PhoneNumber,
            Address: form.Address,
            Notes: form.Notes,
            IdDocumentFile: form.IdDocumentFile,
            PackageName: string.Empty);

        var result = await mediator.Send(cmd);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            form.AvailablePackages = await LoadPackageSelectListAsync();
            return View(form);
        }

        TempData["Success"] = "Đơn đăng ký của bạn đã được gửi. Ban quản lý sẽ xét duyệt trong 1–2 ngày làm việc.";
        return RedirectToAction(nameof(Status));
    }

    [HttpGet]
    public async Task<IActionResult> Status()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var dto = await mediator.Send(new GetMyApplicationStatusQuery(userId));

        var vm = new ApplicationStatusViewModel { HasApplication = dto is not null };
        if (dto is not null)
        {
            vm.ApplicationId = dto.ApplicationId;
            vm.Status = dto.Status;
            vm.ServicePackageName = dto.ServicePackageName;
            vm.ServicePackagePrice = dto.ServicePackagePrice;
            vm.MembershipCode = dto.MembershipCode;
            vm.CreatedAt = dto.CreatedAt;
            vm.ProcessedAt = dto.ProcessedAt;
            vm.RejectionReason = dto.RejectionReason;
            vm.DaysPending = dto.DaysPending;
        }
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await mediator.Send(new CancelMembershipApplicationCommand(id, userId));

        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Đơn đăng ký đã được hủy."
            : result.Error;

        return RedirectToAction(nameof(Status));
    }

    private async Task<List<SelectListItem>> LoadPackageSelectListAsync()
    {
        var packages = await mediator.Send(new GetActivePackagesQuery());
        return packages.Select(p => new SelectListItem(
            $"{p.Name} — {p.Price:N0}đ/{p.DurationMonths} tháng",
            p.Id.ToString())).ToList();
    }
}
