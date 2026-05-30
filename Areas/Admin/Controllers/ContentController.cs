using ComfortRooms.Data;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/pages")]
public sealed class ContentController(ComfortRoomsDbContext dbContext) : Controller
{
    [HttpGet("{slug}/content")]
    public async Task<IActionResult> Index(string slug)
    {
        var page = await dbContext.SitePages
            .AsNoTracking()
            .Include(sitePage => sitePage.ContentBlocks.OrderBy(block => block.SortOrder))
            .SingleOrDefaultAsync(sitePage => sitePage.Slug == slug);

        if (page is null)
        {
            return NotFound();
        }

        var model = new AdminPageContentViewModel
        {
            PageSlug = page.Slug,
            PageTitle = page.Title,
            Blocks = page.ContentBlocks
                .OrderBy(block => block.SortOrder)
                .ThenBy(block => block.Id)
                .Select(block => new AdminPageContentBlockViewModel
                {
                    Id = block.Id,
                    Key = block.Key,
                    Label = block.Label,
                    Value = block.Value,
                    SortOrder = block.SortOrder
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost("{slug}/content/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string slug, int id, string value)
    {
        var block = await dbContext.PageContentBlocks
            .Include(item => item.SitePage)
            .SingleOrDefaultAsync(item => item.Id == id && item.SitePage != null && item.SitePage.Slug == slug);

        if (block is null)
        {
            return NotFound();
        }

        block.Value = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        await dbContext.SaveChangesAsync();

        TempData["AdminMessage"] = "Текстовый блок обновлен.";
        return RedirectToAction(nameof(Index), new { slug });
    }
}
