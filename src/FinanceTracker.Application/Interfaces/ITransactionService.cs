using FinanceTracker.Application.Dtos;

namespace FinanceTracker.Application.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionReadDto>> GetAllAsync(CancellationToken ct = default);
    Task<TransactionReadDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TransactionReadDto> CreateAsync(TransactionCreateDto dto, CancellationToken ct = default);
    Task<TransactionReadDto> UpdateAsync(int id, TransactionCreateDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
