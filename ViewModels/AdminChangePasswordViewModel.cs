using System.ComponentModel.DataAnnotations;

namespace ComfortRooms.ViewModels;

public sealed class AdminChangePasswordViewModel
{
    [Required(ErrorMessage = "Введите текущий пароль.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите новый пароль.")]
    [MinLength(8, ErrorMessage = "Пароль должен быть не короче 8 символов.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Повторите новый пароль.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
