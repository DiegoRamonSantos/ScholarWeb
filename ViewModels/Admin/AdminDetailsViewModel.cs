namespace ScholarWeb.ViewModels.Admin;

public class AdminDetailsViewModel
{
    public string Title { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public object Entity { get; set; } = new();
    public IReadOnlyList<AdminDisplayFieldViewModel> Fields { get; set; } = [];
}
