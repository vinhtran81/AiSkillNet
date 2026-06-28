namespace SkillNet.Web.ViewModels;

public class ServicePackageListViewModel
{
    public List<ServicePackageCardViewModel> Packages { get; set; } = [];
}

public class ServicePackageCardViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMonths { get; set; }
    public List<string> Benefits { get; set; } = [];
    public bool IsPopular { get; set; }
}
