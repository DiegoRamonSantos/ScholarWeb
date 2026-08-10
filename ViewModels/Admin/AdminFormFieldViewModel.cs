namespace ScholarWeb.ViewModels.Admin;

public class AdminFormFieldViewModel
{
    public AdminFormFieldViewModel(string name, string label, AdminFieldType fieldType)
    {
        Name = name;
        Label = label;
        FieldType = fieldType;
    }

    public string Name { get; }
    public string Label { get; }
    public AdminFieldType FieldType { get; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public int Rows { get; set; } = 3;
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? Step { get; set; }
    public string? Min { get; set; }
    public string? Max { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public string? PatternMessage { get; set; }
    public string? InputMode { get; set; }
    public IReadOnlyList<AdminSelectOptionViewModel> Options { get; set; } = [];
}
