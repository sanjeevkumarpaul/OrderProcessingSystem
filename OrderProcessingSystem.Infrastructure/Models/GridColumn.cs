namespace OrderProcessingSystem.Infrastructure.Models;

/// <summary>
/// Grid column model for UI components
/// </summary>
public class GridColumn
{
    public string Header { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public bool Sortable { get; set; }
    public bool Filterable { get; set; }
    public bool IsNumeric { get; set; }
    public bool IsEnum { get; set; }
    public List<string>? EnumValues { get; set; }
}
