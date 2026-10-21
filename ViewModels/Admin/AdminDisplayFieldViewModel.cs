namespace ScholarWeb.ViewModels.Admin;

public class AdminDisplayFieldViewModel
{
    public AdminDisplayFieldViewModel(string label, string propertyPath, AdminDisplayFormat format = AdminDisplayFormat.Text)
    {
        Label = label;
        PropertyPath = propertyPath;
        Format = format;
    }

    public string Label { get; }
    public string PropertyPath { get; }
    public AdminDisplayFormat Format { get; }
}
