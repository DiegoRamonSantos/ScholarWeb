namespace ScholarWeb.ViewModels.Admin;

public class AdminDeleteViewModel
{
    public string Title { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public object Entity { get; set; } = new();
    public IReadOnlyList<AdminDisplayFieldViewModel> Fields { get; set; } = [];
    public bool CanDelete { get; set; }
    public bool CanInactivate { get; set; }
    public string WarningMessage { get; set; } = string.Empty;
    public int Id { get; set; }
}
