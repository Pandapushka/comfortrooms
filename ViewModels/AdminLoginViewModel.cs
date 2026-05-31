using System.ComponentModel.DataAnnotations;

namespace ComfortRooms.ViewModels;

public sealed class AdminLoginViewModel
{
    public bool HasAdminUser { get; set; } = true;

    [Required(ErrorMessage = "Введите логин.")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль.")]
    public string Password { get; set; } = string.Empty;
}
