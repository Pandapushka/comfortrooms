namespace ComfortRooms.Models;

public sealed class AdminUser
{
    public int Id { get; set; }

    public required string Login { get; set; }

    public required string PasswordHash { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
