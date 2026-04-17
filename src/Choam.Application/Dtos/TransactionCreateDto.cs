using System.ComponentModel.DataAnnotations;
using Choam.Domain.Entities;

namespace Choam.Application.Dtos;

public record TransactionCreateDto(
    [Required, StringLength(100, MinimumLength = 1)] string Title,
    [Range(0.0, 10_000_000.0)] decimal Amount,
    [StringLength(500)] string? Description,
    DateTime Date,
    [EnumDataType(typeof(TransactionType))] TransactionType Type,
    [Range(1, int.MaxValue)] int CategoryId
);
