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

        var blocks = page.ContentBlocks
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
            .ToList();

        var model = new AdminPageContentViewModel
        {
            PageSlug = page.Slug,
            PageTitle = page.Title,
            Blocks = blocks,
            Groups = BuildGroups(blocks)
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

    private static IReadOnlyList<AdminPageContentGroupViewModel> BuildGroups(IReadOnlyList<AdminPageContentBlockViewModel> blocks)
    {
        var blockByKey = blocks.ToDictionary(block => block.Key, StringComparer.OrdinalIgnoreCase);
        var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var groups = new List<AdminPageContentGroupViewModel>();

        foreach (var definition in GroupDefinitions)
        {
            var groupBlocks = blocks
                .Where(block => definition.Prefixes.Any(prefix => block.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(block => block.SortOrder)
                .ThenBy(block => block.Id)
                .ToList();

            if (groupBlocks.Count == 0)
            {
                continue;
            }

            var background = groupBlocks.FirstOrDefault(block => block.Key.EndsWith("-background", StringComparison.OrdinalIgnoreCase));
            if (background is not null)
            {
                usedKeys.Add(background.Key);
            }

            var items = BuildItems(groupBlocks, blockByKey, usedKeys);
            groups.Add(new AdminPageContentGroupViewModel
            {
                Title = definition.Title,
                Description = definition.Description,
                BackgroundBlock = background,
                Items = items
            });
        }

        var remainingItems = BuildItems(
            blocks.Where(block => !usedKeys.Contains(block.Key)).OrderBy(block => block.SortOrder).ThenBy(block => block.Id).ToList(),
            blockByKey,
            usedKeys);

        if (remainingItems.Count > 0)
        {
            groups.Add(new AdminPageContentGroupViewModel
            {
                Title = "Остальные настройки",
                Description = "Блоки, которые пока не привязаны к отдельной секции.",
                Items = remainingItems
            });
        }

        return groups;
    }

    private static IReadOnlyList<AdminPageContentItemViewModel> BuildItems(
        IReadOnlyList<AdminPageContentBlockViewModel> blocks,
        IReadOnlyDictionary<string, AdminPageContentBlockViewModel> blockByKey,
        ISet<string> usedKeys)
    {
        var items = new List<AdminPageContentItemViewModel>();

        foreach (var block in blocks)
        {
            if (usedKeys.Contains(block.Key) || IsPairedSetting(block.Key))
            {
                continue;
            }

            var colorBlock = FindPair(blockByKey, block.Key, "-color");
            var styleBlock = FindButtonStylePair(blockByKey, block.Key);

            usedKeys.Add(block.Key);
            if (colorBlock is not null)
            {
                usedKeys.Add(colorBlock.Key);
            }

            if (styleBlock is not null)
            {
                usedKeys.Add(styleBlock.Key);
            }

            items.Add(new AdminPageContentItemViewModel
            {
                TextBlock = block,
                ColorBlock = colorBlock,
                StyleBlock = styleBlock
            });
        }

        return items;
    }

    private static AdminPageContentBlockViewModel? FindPair(
        IReadOnlyDictionary<string, AdminPageContentBlockViewModel> blockByKey,
        string key,
        string suffix)
    {
        return blockByKey.TryGetValue($"{key}{suffix}", out var exactPair)
            ? exactPair
            : null;
    }

    private static AdminPageContentBlockViewModel? FindButtonStylePair(
        IReadOnlyDictionary<string, AdminPageContentBlockViewModel> blockByKey,
        string key)
    {
        if (!key.EndsWith("-text", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var styleKey = $"{key[..^"-text".Length]}-style";
        return blockByKey.TryGetValue(styleKey, out var styleBlock) ? styleBlock : null;
    }

    private static bool IsPairedSetting(string key)
    {
        return key.EndsWith("-color", StringComparison.OrdinalIgnoreCase)
            || key.EndsWith("-button-style", StringComparison.OrdinalIgnoreCase)
            || key.EndsWith("-background", StringComparison.OrdinalIgnoreCase);
    }

    private static readonly IReadOnlyList<ContentGroupDefinition> GroupDefinitions =
    [
        new ContentGroupDefinition("Страница", "Общий фон пространства вокруг отдельных блоков.", ["page-"]),
        new ContentGroupDefinition("Hero", "Первый экран страницы: заголовки, описание, кнопки и фон.", ["hero-"]),
        new ContentGroupDefinition("Цифры доверия", "Статистические показатели и фон второго блока главной страницы.", ["stats-"]),
        new ContentGroupDefinition("Направления", "Заголовок раздела и карточки основных направлений.", ["directions-", "direction-"]),
        new ContentGroupDefinition("Подход", "Блок про визуальную систему и карточки преимуществ.", ["approach-", "feature-"]),
        new ContentGroupDefinition("Отзывы", "Старые настройки заголовка отзывов. Новые отзывы добавляются как блок третьего шаблона в конструкторе блоков.", ["testimonials-"]),
        new ContentGroupDefinition("CTA", "Финальный призыв к действию и кнопка.", ["cta-"]),
        new ContentGroupDefinition("Форма", "Тексты формы и пояснения рядом с ней.", ["form-", "contact-"]),
        new ContentGroupDefinition("Процесс", "Этапы работы и процессные блоки.", ["process-", "intro-", "why-", "portfolio-"]),
        new ContentGroupDefinition("Принципы", "Принципы, преимущества и дополнительные секции.", ["principles-", "accent-"])
    ];

    private sealed record ContentGroupDefinition(string Title, string Description, IReadOnlyList<string> Prefixes);

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
