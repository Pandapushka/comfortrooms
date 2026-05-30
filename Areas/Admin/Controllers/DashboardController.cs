using ComfortRooms.Data;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin")]
public sealed class DashboardController(ComfortRoomsDbContext dbContext) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var pages = await dbContext.SitePages
            .AsNoTracking()
            .OrderBy(page => page.SortOrder)
            .Select(page => new AdminPageListItemViewModel
            {
                Slug = page.Slug,
                Title = page.Title,
                ImagesCount = page.Images.Count
            })
            .ToListAsync();

        return View(pages);
    }
}
