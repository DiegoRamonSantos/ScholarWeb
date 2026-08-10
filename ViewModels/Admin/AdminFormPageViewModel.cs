namespace ScholarWeb.ViewModels.Admin;

public class AdminFormPageViewModel
{
    public string Title { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public object Entity { get; set; } = new();
    public IReadOnlyList<AdminFormFieldViewModel> Fields { get; set; } = [];
    public bool IsEdit { get; set; }
    public int? Id { get; set; }
}
