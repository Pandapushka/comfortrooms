using ComfortRooms.Data;
using ComfortRooms.Models;
using ComfortRooms.Services;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/setup")]
public sealed class SetupController(ComfortRoomsDbContext dbContext, IAdminPasswordHasher passwordHasher) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (await dbContext.AdminUsers.AnyAsync())
        {
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

        return View(new AdminSetupViewModel());
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AdminSetupViewModel model)
    {
        if (await dbContext.AdminUsers.AnyAsync())
        {
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        dbContext.AdminUsers.Add(new AdminUser
        {
            Login = model.Login.Trim(),
            PasswordHash = passwordHasher.HashPassword(model.Password)
        });
        await dbContext.SaveChangesAsync();

        TempData["AdminMessage"] = "Администратор создан. Теперь можно войти.";
        return RedirectToAction("Login", "Account", new { area = "Admin" });
    }
}
