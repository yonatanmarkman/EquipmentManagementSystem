using API.DTOs;
using API.Helpers;

namespace API.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<PagedResultDto<EquipmentDto>> GetEquipmentItemsAsync(int pageNumber, int pageSize);
        Task<EquipmentDto?> GetEquipmentByIdAsync(int id);
        Task<PagedResultDto<EquipmentDto>> SearchEquipmentAsync(EquipmentSearchDto searchDto);
        Task<Result<EquipmentDto>> CreateEquipmentAsync(CreateEquipmentDto createDto);
        Task<Result<EquipmentDto>> UpdateEquipmentAsync(int id, UpdateEquipmentDto updateDto);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<PagedResultDto<EquipmentDto>> GetEquipmentByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    }
}