using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SkillNet.Application.Features.Membership.Queries.GetActivePackages;
using SkillNet.Web.ViewModels;

namespace SkillNet.Web.Controllers;

public class ServicePackageController(IMediator mediator) : Controller
{
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> Index()
    {
        var packages = await mediator.Send(new GetActivePackagesQuery());
        var vm = new ServicePackageListViewModel
        {
            Packages = packages.Select(p => new ServicePackageCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DurationMonths = p.DurationMonths,
                Benefits = p.Benefits,
                IsPopular = p.IsPopular
            }).ToList()
        };
        return View(vm);
    }
}
