using System.ComponentModel.DataAnnotations;

namespace ComfortRooms.ViewModels;

public sealed class LeadRequestFormViewModel
{
    [Required(ErrorMessage = "Введите имя.")]
    [MaxLength(160, ErrorMessage = "Имя не должно быть длиннее 160 символов.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите телефон.")]
    [MaxLength(80, ErrorMessage = "Телефон не должен быть длиннее 80 символов.")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Комментарий не должен быть длиннее 2000 символов.")]
    public string? Message { get; set; }
}
