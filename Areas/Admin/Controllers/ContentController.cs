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
                    SortOrder = block.SortOrder,
                    Options = GetOptions(block.Key)
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

    [HttpPost("{slug}/content")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAll(string slug, Dictionary<int, string> values)
    {
        var blocks = await dbContext.PageContentBlocks
            .Include(item => item.SitePage)
            .Where(item => item.SitePage != null && item.SitePage.Slug == slug)
            .ToListAsync();

        if (blocks.Count == 0)
        {
            return NotFound();
        }

        foreach (var block in blocks)
        {
            if (values.TryGetValue(block.Id, out var value))
            {
                block.Value = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
            }
        }

        await dbContext.SaveChangesAsync();

        TempData["AdminMessage"] = "Тексты страницы обновлены.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    private static IReadOnlyList<AdminSelectOptionViewModel> GetOptions(string key)
    {
        if (key.EndsWith("-color", StringComparison.OrdinalIgnoreCase))
        {
            return TextColorOptions;
        }

        if (key.EndsWith("-background", StringComparison.OrdinalIgnoreCase))
        {
            return BackgroundOptions;
        }

        if (key.EndsWith("-button-style", StringComparison.OrdinalIgnoreCase))
        {
            return ButtonStyleOptions;
        }

        return [];
    }

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> TextColorOptions =
    [
        new AdminSelectOptionViewModel { Value = "text-accent-gold", Label = "Золотой" },
        new AdminSelectOptionViewModel { Value = "text-accent-charcoal", Label = "Темный графит" },
        new AdminSelectOptionViewModel { Value = "text-accent-terracotta", Label = "Терракотовый" },
        new AdminSelectOptionViewModel { Value = "text-accent-sage", Label = "Шалфейный" },
        new AdminSelectOptionViewModel { Value = "text-accent-cream", Label = "Светлый кремовый" },
        new AdminSelectOptionViewModel { Value = "text-accent-brass", Label = "Латунный" },
        new AdminSelectOptionViewModel { Value = "text-accent-copper", Label = "Медный" },
        new AdminSelectOptionViewModel { Value = "text-accent-olive", Label = "Оливковый" },
        new AdminSelectOptionViewModel { Value = "text-accent-forest", Label = "Темно-зеленый" },
        new AdminSelectOptionViewModel { Value = "text-accent-steel", Label = "Стальной" },
        new AdminSelectOptionViewModel { Value = "text-accent-warm-gray", Label = "Теплый серый" },
        new AdminSelectOptionViewModel { Value = "text-accent-ruby", Label = "Рубиновый" }
    ];

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> BackgroundOptions =
    [
        new AdminSelectOptionViewModel { Value = "surface-cream", Label = "Кремовый" },
        new AdminSelectOptionViewModel { Value = "surface-white", Label = "Белый" },
        new AdminSelectOptionViewModel { Value = "surface-warm", Label = "Теплый светлый" },
        new AdminSelectOptionViewModel { Value = "surface-sand", Label = "Песочный" },
        new AdminSelectOptionViewModel { Value = "surface-gold-soft", Label = "Мягкий золотой" },
        new AdminSelectOptionViewModel { Value = "surface-sage-soft", Label = "Мягкий шалфей" },
        new AdminSelectOptionViewModel { Value = "surface-terracotta-soft", Label = "Мягкая терракота" },
        new AdminSelectOptionViewModel { Value = "surface-stone", Label = "Каменный" },
        new AdminSelectOptionViewModel { Value = "surface-mist", Label = "Светлый туман" },
        new AdminSelectOptionViewModel { Value = "surface-charcoal", Label = "Темный графит" },
        new AdminSelectOptionViewModel { Value = "surface-forest", Label = "Темный зеленый" },
        new AdminSelectOptionViewModel { Value = "surface-ink", Label = "Черный" }
    ];

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> ButtonStyleOptions =
    [
        new AdminSelectOptionViewModel { Value = "button--primary", Label = "Темная" },
        new AdminSelectOptionViewModel { Value = "button--secondary", Label = "Светлая с рамкой" },
        new AdminSelectOptionViewModel { Value = "button--gold", Label = "Золотая" },
        new AdminSelectOptionViewModel { Value = "button--sage", Label = "Шалфейная" },
        new AdminSelectOptionViewModel { Value = "button--cream", Label = "Кремовая" },
        new AdminSelectOptionViewModel { Value = "button--charcoal-outline", Label = "Графитовая рамка" },
        new AdminSelectOptionViewModel { Value = "button--brass", Label = "Латунная" },
        new AdminSelectOptionViewModel { Value = "button--copper", Label = "Медная" },
        new AdminSelectOptionViewModel { Value = "button--olive", Label = "Оливковая" },
        new AdminSelectOptionViewModel { Value = "button--forest", Label = "Темно-зеленая" },
        new AdminSelectOptionViewModel { Value = "button--steel", Label = "Стальная" },
        new AdminSelectOptionViewModel { Value = "button--ruby", Label = "Рубиновая" }
    ];
}
