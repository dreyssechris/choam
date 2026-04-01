using Choam.Application.Dtos;
using Choam.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Choam.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/transactions")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TransactionReadDto>>> GetAll(CancellationToken ct)
        => Ok(await transactionService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionReadDto>> GetById(int id, CancellationToken ct)
        => Ok(await transactionService.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<TransactionReadDto>> Create(TransactionCreateDto dto, CancellationToken ct)
    {
        var created = await transactionService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransactionReadDto>> Update(int id, TransactionCreateDto dto, CancellationToken ct)
        => Ok(await transactionService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await transactionService.DeleteAsync(id, ct);
        return NoContent();
    }
}
