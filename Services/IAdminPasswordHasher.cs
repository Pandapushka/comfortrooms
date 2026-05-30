namespace ComfortRooms.Services;

public interface IAdminPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}
