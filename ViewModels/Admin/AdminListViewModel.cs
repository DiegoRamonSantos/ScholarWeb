namespace ScholarWeb.ViewModels.Admin;

public class AdminListViewModel
{
    public string Title { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string? SearchTerm { get; set; }
    public string SearchPlaceholder { get; set; } = "Buscar...";
    public IReadOnlyList<AdminColumnViewModel> Columns { get; set; } = [];
    public IReadOnlyList<object> Items { get; set; } = [];
}
