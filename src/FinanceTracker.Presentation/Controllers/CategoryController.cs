using FinanceTracker.Application.Dtos;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Presentation.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryReadDto>>> GetAll(CancellationToken ct)
        => Ok(await categoryService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryReadDto>> GetById(int id, CancellationToken ct)
        => Ok(await categoryService.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<CategoryReadDto>> Create(CategoryCreateDto dto, CancellationToken ct)
    {
        var created = await categoryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryReadDto>> Update(int id, CategoryCreateDto dto, CancellationToken ct)
        => Ok(await categoryService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await categoryService.DeleteAsync(id, ct);
        return NoContent();
    }
}
