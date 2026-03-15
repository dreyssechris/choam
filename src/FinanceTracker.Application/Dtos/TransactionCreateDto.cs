using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Dtos;

public record TransactionCreateDto(
    string Title,
    decimal Amount,
    string? Description,
    DateTime Date,
    TransactionType Type,
    int CategoryId);
