using System.Collections.Generic;

namespace OrderProcessingServer.Shared.UIViewModels;
    
public class GridColumnVM
{
    public string Header { get; set; } = string.Empty;
    // Field supports dotted path like Customer.Name
    public string Field { get; set; } = string.Empty;
    public bool Sortable { get; set; } = false;
    public bool Filterable { get; set; } = false;
    // When true the grid will right-align and format values as numeric
    public bool IsNumeric { get; set; } = false;
    public bool IsEnum { get; set; } = false;
    public IEnumerable<string>? EnumValues { get; set; }
}
