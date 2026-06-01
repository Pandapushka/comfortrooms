using System.Diagnostics;
using ComfortRooms.Services;
using Microsoft.AspNetCore.Mvc;
using ComfortRooms.Models;

namespace ComfortRooms.Controllers;

public class HomeController(IPageContentService pageContentService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var textBlocks = await pageContentService.GetTextBlocksAsync(PageSlugs.Home, cancellationToken);
        ViewData["TextBlocks"] = textBlocks;
        ViewData["HeroTitle"] = PageContentService.GetText(textBlocks, "hero-title", "Свет, который становится частью архитектуры");
        ViewData["HeroDescription"] = PageContentService.GetText(textBlocks, "hero-description", "Comfort Rooms создает выразительные световые решения для частных интерьеров, дизайнерских проектов, салонов и онлайн-партнеров.");
        var images = await pageContentService.GetGalleryImagesAsync(PageSlugs.Home, cancellationToken);
        var heroImageUrl = images.FirstOrDefault()?.ImageUrl; 
        ViewData["HeroImageUrl"] = heroImageUrl;
        return View();
    }

    [HttpGet("o-kompanii")]
    public async Task<IActionResult> About(CancellationToken cancellationToken)
    {
        var textBlocks = await pageContentService.GetTextBlocksAsync(PageSlugs.About, cancellationToken);
        ViewData["TextBlocks"] = textBlocks;
        ViewData["HeroTitle"] = PageContentService.GetText(textBlocks, "hero-title", "О компании");
        ViewData["HeroDescription"] = PageContentService.GetText(textBlocks, "hero-description", "Comfort Rooms работает со светом как с архитектурным акцентом: помогает подобрать готовые решения, спроектировать индивидуальные изделия и поддержать интерьерные проекты на всех этапах.");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
