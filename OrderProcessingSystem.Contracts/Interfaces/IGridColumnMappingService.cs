using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Contracts.Interfaces;

public interface IGridColumnMappingService
{
    List<UIGridColumnDto> MapToUIColumns(List<GridColumnDto> contractDtos);
    UIGridColumnDto MapToUIColumn(GridColumnDto contractDto);
}
