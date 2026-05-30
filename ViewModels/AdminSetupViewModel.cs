using System.ComponentModel.DataAnnotations;

namespace ComfortRooms.ViewModels;

public sealed class AdminSetupViewModel
{
    [Required(ErrorMessage = "Введите логин.")]
    [MaxLength(80, ErrorMessage = "Логин не должен быть длиннее 80 символов.")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль.")]
    [MinLength(10, ErrorMessage = "Пароль должен быть не короче 10 символов.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Повторите пароль.")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
