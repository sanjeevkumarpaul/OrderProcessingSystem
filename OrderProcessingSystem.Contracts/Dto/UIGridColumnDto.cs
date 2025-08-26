namespace OrderProcessingSystem.Contracts.Dto;

/// <summary>
/// UI-specific DTO for grid column metadata - optimized for client consumption
/// </summary>
public class UIGridColumnDto
{
    public string Header { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public bool Sortable { get; set; }
    public bool Filterable { get; set; }
    public bool IsNumeric { get; set; }
    public bool IsEnum { get; set; }
}
