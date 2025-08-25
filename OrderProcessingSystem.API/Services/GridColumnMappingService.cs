using AutoMapper;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.API.Models;

namespace OrderProcessingSystem.API.Services;

/// <summary>
/// Service for handling DTO transformations using AutoMapper
/// Encapsulates mapping logic for better separation of concerns
/// </summary>
public interface IGridColumnMappingService
{
    List<UIGridColumnDto> MapToUIColumns(List<GridColumnDto> contractDtos);
    UIGridColumnDto MapToUIColumn(GridColumnDto contractDto);
}

public class GridColumnMappingService : IGridColumnMappingService
{
    private readonly IMapper _mapper;

    public GridColumnMappingService(IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public List<UIGridColumnDto> MapToUIColumns(List<GridColumnDto> contractDtos)
    {
        if (contractDtos == null || contractDtos.Count == 0)
            return new List<UIGridColumnDto>();

        return _mapper.Map<List<UIGridColumnDto>>(contractDtos);
    }

    public UIGridColumnDto MapToUIColumn(GridColumnDto contractDto)
    {
        if (contractDto == null)
            throw new ArgumentNullException(nameof(contractDto));

        return _mapper.Map<UIGridColumnDto>(contractDto);
    }
}
