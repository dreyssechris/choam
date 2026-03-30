using Choam.Application.Dtos;
using Choam.Domain.Entities;

namespace Choam.Application.Mapping;

public static class MappingExtensions
{
    // Category mappings
    public static CategoryReadDto ToDto(this Category entity)
        => new(entity.Id, entity.Name);

    public static List<CategoryReadDto> ToDto(this IEnumerable<Category> entities)
        => entities.Select(e => e.ToDto()).ToList();

    public static Category ToEntity(this CategoryCreateDto dto)
        => new() { Name = dto.Name };

    // Transaction mappings
    public static TransactionReadDto ToDto(this Transaction entity)
        => new(
            entity.Id,
            entity.Title,
            entity.Amount,
            entity.Description,
            entity.Date,
            entity.Type,
            entity.CategoryId);

    public static List<TransactionReadDto> ToDto(this IEnumerable<Transaction> entities)
        => entities.Select(e => e.ToDto()).ToList();

    public static Transaction ToEntity(this TransactionCreateDto dto)
        => new()
        {
            Title = dto.Title,
            Amount = dto.Amount,
            Description = dto.Description ?? string.Empty,
            Date = EnsureUtc(dto.Date),
            Type = dto.Type,
            CategoryId = dto.CategoryId
        };

    public static void ApplyUpdate(this Transaction entity, TransactionCreateDto dto)
    {
        entity.Title = dto.Title;
        entity.Amount = dto.Amount;
        entity.Description = dto.Description ?? string.Empty;
        entity.Date = EnsureUtc(dto.Date);
        entity.Type = dto.Type;
        entity.CategoryId = dto.CategoryId;
    }

    private static DateTime EnsureUtc(DateTime dt)
        => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}
