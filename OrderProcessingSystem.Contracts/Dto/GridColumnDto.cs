namespace OrderProcessingSystem.Contracts.Dto;

public class GridColumnDto
{
    public string Header { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public bool Sortable { get; set; } = false;
    public bool Filterable { get; set; } = false;
    public bool IsNumeric { get; set; } = false;
    public bool IsEnum { get; set; } = false;
    public List<string>? EnumValues { get; set; }
}
