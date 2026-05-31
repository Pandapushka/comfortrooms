using System.Security.Claims;
using ComfortRooms.Data;
using ComfortRooms.Services;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin")]
public sealed class AccountController(ComfortRoomsDbContext dbContext, IAdminPasswordHasher passwordHasher) : Controller
{
    [HttpGet("login")]
    public async Task<IActionResult> Login()
    {
        return View(new AdminLoginViewModel
        {
            HasAdminUser = await dbContext.AdminUsers.AnyAsync()
        });
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.HasAdminUser = await dbContext.AdminUsers.AnyAsync();
            return View(model);
        }

        var admin = await dbContext.AdminUsers.SingleOrDefaultAsync(user => user.Login == model.Login);
        if (admin is null || !passwordHasher.VerifyPassword(model.Password, admin.PasswordHash))
        {
            model.HasAdminUser = await dbContext.AdminUsers.AnyAsync();
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Name, admin.Login),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return View(new AdminChangePasswordViewModel());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(AdminChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var adminIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(adminIdValue, out var adminId))
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        var admin = await dbContext.AdminUsers.SingleOrDefaultAsync(user => user.Id == adminId);
        if (admin is null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        if (!passwordHasher.VerifyPassword(model.CurrentPassword, admin.PasswordHash))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Текущий пароль указан неверно.");
            return View(model);
        }

        admin.PasswordHash = passwordHasher.HashPassword(model.NewPassword);
        await dbContext.SaveChangesAsync();

        TempData["AdminMessage"] = "Пароль администратора обновлен.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
