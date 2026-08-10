namespace ScholarWeb.ViewModels.Admin;

public class AdminColumnViewModel
{
    public AdminColumnViewModel(string header, string propertyPath, AdminDisplayFormat format = AdminDisplayFormat.Text)
    {
        Header = header;
        PropertyPath = propertyPath;
        Format = format;
    }

    public string Header { get; }
    public string PropertyPath { get; }
    public AdminDisplayFormat Format { get; }
}
