using System.ComponentModel.DataAnnotations;

namespace Choam.Application.Dtos;

public record ChatMessageDto(
    [Required, StringLength(20)] string Role,
    [Required, StringLength(4000)] string Content);

public record ChatRequestDto(
    [Required, StringLength(2000, MinimumLength = 1)] string Message,
    [MaxLength(40)] List<ChatMessageDto>? History);
