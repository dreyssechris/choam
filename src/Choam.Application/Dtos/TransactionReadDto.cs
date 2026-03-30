using Choam.Domain.Entities;

namespace Choam.Application.Dtos;

public record TransactionReadDto(
    int Id,
    string Title,
    decimal Amount,
    string Description,
    DateTime Date,
    TransactionType Type,
    int CategoryId);
