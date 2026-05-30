using ComfortRooms.Models;
using ComfortRooms.Services;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ComfortRoomsDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IAdminPasswordHasher>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        Directory.CreateDirectory(Path.Combine(environment.ContentRootPath, "App_Data"));
        await dbContext.Database.EnsureCreatedAsync();
        await SeedPagesAsync(dbContext);
        await SeedGalleryAsync(dbContext);
        await SeedAdminAsync(dbContext, configuration, passwordHasher);
    }

    private static async Task SeedPagesAsync(ComfortRoomsDbContext dbContext)
    {
        if (await dbContext.SitePages.AnyAsync())
        {
            return;
        }

        dbContext.SitePages.AddRange(
            new SitePage { Slug = PageSlugs.Home, Title = "Главная страница", SortOrder = 10 },
            new SitePage { Slug = PageSlugs.CustomOrder, Title = "Изготовление люстр под заказ", SortOrder = 20 },
            new SitePage { Slug = PageSlugs.Cooperation, Title = "Сотрудничество", SortOrder = 30 },
            new SitePage { Slug = PageSlugs.Designers, Title = "Дизайнерам", SortOrder = 40 },
            new SitePage { Slug = PageSlugs.Shops, Title = "Магазинам", SortOrder = 50 },
            new SitePage { Slug = PageSlugs.Ecommerce, Title = "Интернет-магазинам", SortOrder = 60 },
            new SitePage { Slug = PageSlugs.About, Title = "О компании", SortOrder = 70 },
            new SitePage { Slug = PageSlugs.Contacts, Title = "Контакты", SortOrder = 80 });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedGalleryAsync(ComfortRoomsDbContext dbContext)
    {
        var page = await dbContext.SitePages.SingleAsync(page => page.Slug == PageSlugs.CustomOrder);
        if (await dbContext.PageImages.AnyAsync(image => image.SitePageId == page.Id))
        {
            return;
        }

        dbContext.PageImages.AddRange(
            new PageImage { SitePageId = page.Id, SortOrder = 10, Title = "Золото Роял", ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/109d82bb8-7d01-4b67-9aa3-2b000e635158.png", AltText = "Люстра в золотой отделке" },
            new PageImage { SitePageId = page.Id, SortOrder = 20, Title = "Классика Бронза", ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/111275a01-589c-4089-96be-fc07fb47357d.png", AltText = "Классическая бронзовая люстра" },
            new PageImage { SitePageId = page.Id, SortOrder = 30, Title = "Латунь и Кристалл", ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/1ab29de9e-b6b6-46fd-b809-83c08fdbf763.png", AltText = "Современная люстра из латуни и хрусталя" },
            new PageImage { SitePageId = page.Id, SortOrder = 40, Title = "Арт-Объект Медь", ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/194cf7431-b515-4024-82dd-ec4c7a499814.png", AltText = "Дизайнерский светильник в медной отделке" });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedAdminAsync(ComfortRoomsDbContext dbContext, IConfiguration configuration, IAdminPasswordHasher passwordHasher)
    {
        if (await dbContext.AdminUsers.AnyAsync())
        {
            return;
        }

        var login = configuration["AdminSeed:Login"];
        var password = configuration["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        dbContext.AdminUsers.Add(new AdminUser
        {
            Login = login,
            PasswordHash = passwordHasher.HashPassword(password)
        });

        await dbContext.SaveChangesAsync();
    }
}
