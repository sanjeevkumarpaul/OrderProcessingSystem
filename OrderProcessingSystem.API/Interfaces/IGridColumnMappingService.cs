using OrderProcessingSystem.Contracts.Dto;


namespace OrderProcessingSystem.API.Interfaces;
public interface IGridColumnMappingService
{
    List<UIGridColumnDto> MapToUIColumns(List<GridColumnDto> contractDtos);
    UIGridColumnDto MapToUIColumn(GridColumnDto contractDto);
}
