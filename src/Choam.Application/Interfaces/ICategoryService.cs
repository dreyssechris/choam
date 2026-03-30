using Choam.Application.Dtos;

namespace Choam.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryReadDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryReadDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default);
    Task<CategoryReadDto> UpdateAsync(int id, CategoryCreateDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
