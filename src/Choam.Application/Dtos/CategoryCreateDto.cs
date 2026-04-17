using System.ComponentModel.DataAnnotations;

namespace Choam.Application.Dtos;

public record CategoryCreateDto(
    [Required, StringLength(50, MinimumLength = 1)] string Name);
