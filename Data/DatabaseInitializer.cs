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
        if (await HasLegacyEnsureCreatedSchemaAsync(dbContext))
        {
            await EnsureLegacySchemaUpdatesAsync(dbContext);
        }
        else
        {
            await dbContext.Database.MigrateAsync();
        }

        await SeedPagesAsync(dbContext);
        await SeedContentBlocksAsync(dbContext);
        await SeedGalleryAsync(dbContext);
        await SeedAdminAsync(dbContext, configuration, passwordHasher);
    }

    private static async Task<bool> HasLegacyEnsureCreatedSchemaAsync(ComfortRoomsDbContext dbContext)
    {
        var hasSitePages = await HasTableAsync(dbContext, "SitePages");
        var hasMigrationsHistory = await HasTableAsync(dbContext, "__EFMigrationsHistory");

        return hasSitePages && !hasMigrationsHistory;
    }

    private static async Task EnsureLegacySchemaUpdatesAsync(ComfortRoomsDbContext dbContext)
    {
        if (await HasTableAsync(dbContext, "PageContentBlocks"))
        {
            return;
        }

        var connection = dbContext.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            await connection.OpenAsync();
        }

        try
        {
            await using var createCommand = connection.CreateCommand();
            createCommand.CommandText = """
                CREATE TABLE "PageContentBlocks" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_PageContentBlocks" PRIMARY KEY AUTOINCREMENT,
                    "SitePageId" INTEGER NOT NULL,
                    "Key" TEXT NOT NULL,
                    "Label" TEXT NOT NULL,
                    "Value" TEXT NOT NULL,
                    "SortOrder" INTEGER NOT NULL,
                    CONSTRAINT "FK_PageContentBlocks_SitePages_SitePageId" FOREIGN KEY ("SitePageId") REFERENCES "SitePages" ("Id") ON DELETE CASCADE
                );
                """;
            await createCommand.ExecuteNonQueryAsync();

            await using var indexCommand = connection.CreateCommand();
            indexCommand.CommandText = """
                CREATE UNIQUE INDEX "IX_PageContentBlocks_SitePageId_Key"
                ON "PageContentBlocks" ("SitePageId", "Key");
                """;
            await indexCommand.ExecuteNonQueryAsync();
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task<bool> HasTableAsync(ComfortRoomsDbContext dbContext, string tableName)
    {
        var connection = dbContext.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            await connection.OpenAsync();
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = $tableName";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "$tableName";
            parameter.Value = tableName;
            command.Parameters.Add(parameter);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task SeedPagesAsync(ComfortRoomsDbContext dbContext)
    {
        var existingSlugs = await dbContext.SitePages
            .Select(page => page.Slug)
            .ToListAsync();
        var existingSlugSet = existingSlugs.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var pages = new[]
        {
            new SitePage { Slug = PageSlugs.Home, Title = "Главная страница", SortOrder = 10 },
            new SitePage { Slug = PageSlugs.CustomOrder, Title = "Изготовление люстр под заказ", SortOrder = 20 },
            new SitePage { Slug = PageSlugs.Cooperation, Title = "Сотрудничество", SortOrder = 30 },
            new SitePage { Slug = PageSlugs.Wholesale, Title = "Оптовым партнерам", SortOrder = 40 },
            new SitePage { Slug = PageSlugs.Retail, Title = "Розничным клиентам", SortOrder = 50 },
            new SitePage { Slug = PageSlugs.Designers, Title = "Дизайнерам", SortOrder = 60 },
            new SitePage { Slug = PageSlugs.Shops, Title = "Магазинам", SortOrder = 70 },
            new SitePage { Slug = PageSlugs.Ecommerce, Title = "Интернет-магазинам", SortOrder = 80 },
            new SitePage { Slug = PageSlugs.About, Title = "О компании", SortOrder = 90 },
            new SitePage { Slug = PageSlugs.Contacts, Title = "Контакты", SortOrder = 100 }
        };

        var missingPages = pages
            .Where(page => !existingSlugSet.Contains(page.Slug))
            .ToList();

        if (missingPages.Count == 0)
        {
            return;
        }

        dbContext.SitePages.AddRange(missingPages);

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

    private static async Task SeedContentBlocksAsync(ComfortRoomsDbContext dbContext)
    {
        var pages = await dbContext.SitePages
            .Include(page => page.ContentBlocks)
            .ToDictionaryAsync(page => page.Slug);
        var blocks = new List<PageContentBlock>();

        AddBlock(pages, blocks, PageSlugs.Home, "hero-title", "Главный заголовок", "Свет, который становится частью архитектуры", 10);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-description", "Описание hero", "Comfort Rooms создает выразительные световые решения для частных интерьеров, дизайнерских проектов, салонов и онлайн-партнеров.", 20);
        AddBlock(pages, blocks, PageSlugs.Home, "directions-title", "Заголовок направлений", "Основные разделы сайта", 30);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-title", "Заголовок подхода", "Единый стиль для всех страниц", 40);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-description", "Описание подхода", "Визуальная система строится вокруг готовой страницы «Изготовление люстр под заказ»: светлый фон, золотые акценты, крупная типографика, аккуратные карточки, галереи и спокойные анимации.", 50);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-title", "Заголовок CTA", "Нужен светильник под конкретный интерьер?", 60);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-description", "Описание CTA", "Оставьте заявку на расчет индивидуального изделия или перейдите к странице с подробным процессом работы.", 70);
        AddBlock(pages, blocks, PageSlugs.About, "hero-title", "Заголовок страницы", "О компании", 10);
        AddBlock(pages, blocks, PageSlugs.About, "hero-description", "Описание страницы", "Comfort Rooms работает со светом как с архитектурным акцентом: помогает подобрать готовые решения, спроектировать индивидуальные изделия и поддержать интерьерные проекты на всех этапах.", 20);
        AddBlock(pages, blocks, PageSlugs.About, "hero-accent", "Акцент hero", "Светильник должен не просто освещать пространство, а собирать интерьер в единую композицию.", 30);
        AddBlock(pages, blocks, PageSlugs.About, "principles-title", "Заголовок принципов", "Что важно в нашей работе", 40);
        AddBlock(pages, blocks, PageSlugs.About, "process-title", "Заголовок процесса", "От идеи до готового светильника", 50);
        AddBlock(pages, blocks, PageSlugs.About, "process-description", "Описание процесса", "Для нестандартных задач мы начинаем с эскиза, референса или чертежа, затем уточняем материалы, размеры, отделку и способ монтажа.", 60);
        AddBlock(pages, blocks, PageSlugs.About, "cta-title", "Заголовок CTA", "Обсудим проект или партнерство?", 70);
        AddBlock(pages, blocks, PageSlugs.About, "cta-description", "Описание CTA", "Перейдите к странице изготовления под заказ или оставьте заявку через контакты.", 80);
        AddBlock(pages, blocks, PageSlugs.Contacts, "hero-title", "Заголовок страницы", "Контакты", 10);
        AddBlock(pages, blocks, PageSlugs.Contacts, "hero-description", "Описание страницы", "Обсудим индивидуальный светильник, партнерство, поставки или комплектацию проекта.", 20);
        AddBlock(pages, blocks, PageSlugs.Contacts, "form-title", "Заголовок формы", "Расскажите о задаче", 30);
        AddBlock(pages, blocks, PageSlugs.Contacts, "form-description", "Описание формы", "Форма сохранит обращение в админ-панели. Менеджер сможет увидеть заявку в разделе «Заявки».", 40);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "hero-title", "Заголовок страницы", "Изготовление люстр под заказ", 10);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "hero-description", "Описание страницы", "От идеи до воплощения: эксклюзивные светильники для дизайнеров интерьеров, архитекторов и частных заказчиков.", 20);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "intro-title", "Заголовок вводного блока", "Ваш проект требует особого решения?", 30);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "intro-description", "Описание вводного блока", "Стандартные люстры не подходят к уникальному интерьеру? Мы специализируемся на производстве люстр и светильников на заказ любой сложности. Мы воплощаем в жизнь самые смелые идеи: от чертежей дизайнера до вашей фотографии или эскиза.", 40);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "intro-accent", "Акцент вводного блока", "Наше призвание — создавать не просто освещение, а арт-объекты, которые становятся смысловым центром пространства.", 50);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "process-title", "Заголовок процесса", "Как мы работаем?", 60);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "why-title", "Заголовок преимуществ", "Почему выбирают нас?", 70);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "portfolio-title", "Заголовок портфолио", "Наши работы", 80);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "portfolio-description", "Описание портфолио", "Листайте, чтобы увидеть больше", 90);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "contact-title", "Заголовок формы заявки", "Изготовить светильник?", 100);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "contact-description", "Описание формы заявки", "Хотите эксклюзивный светильник или воссоздать люстру по картинке или чертежу? Тогда смело оставляйте заявку!", 110);
        AddBlock(pages, blocks, PageSlugs.Wholesale, "hero-title", "Заголовок страницы", "Оптовым партнерам", 10);
        AddBlock(pages, blocks, PageSlugs.Wholesale, "hero-description", "Описание страницы", "Раздел для оптовых партнеров, комплектации объектов и регулярных поставок светильников Comfort Rooms.", 20);
        AddBlock(pages, blocks, PageSlugs.Retail, "hero-title", "Заголовок страницы", "Розничным клиентам", 10);
        AddBlock(pages, blocks, PageSlugs.Retail, "hero-description", "Описание страницы", "Помогаем частным клиентам и розничным покупателям подобрать светильники для интерьера или заказать изделие под конкретную задачу.", 20);
        AddBlock(pages, blocks, PageSlugs.Designers, "hero-title", "Заголовок страницы", "Дизайнерам", 10);
        AddBlock(pages, blocks, PageSlugs.Designers, "hero-description", "Описание страницы", "Помогаем дизайнерам интерьеров решать задачи со светом: от подбора готовых моделей до изготовления авторских светильников под конкретный проект.", 20);
        AddBlock(pages, blocks, PageSlugs.Shops, "hero-title", "Заголовок страницы", "Магазинам", 10);
        AddBlock(pages, blocks, PageSlugs.Shops, "hero-description", "Описание страницы", "Развиваем партнерство с магазинами и салонами света: помогаем формировать ассортимент, работать с запросами клиентов и поддерживать продажи.", 20);
        AddBlock(pages, blocks, PageSlugs.Ecommerce, "hero-title", "Заголовок страницы", "Интернет-магазинам", 10);
        AddBlock(pages, blocks, PageSlugs.Ecommerce, "hero-description", "Описание страницы", "Готовим основу для онлайн-продаж: карточки, изображения, описания, категории и понятную работу с заявками по индивидуальным изделиям.", 20);

        foreach (var slug in new[] { PageSlugs.Wholesale, PageSlugs.Retail, PageSlugs.Designers, PageSlugs.Shops, PageSlugs.Ecommerce })
        {
            AddBlock(pages, blocks, slug, "help-title", "Заголовок возможностей", "Как мы помогаем", 30);
            AddBlock(pages, blocks, slug, "process-title", "Заголовок процесса", "Прозрачный процесс", 40);
            AddBlock(pages, blocks, slug, "process-description", "Описание процесса", "Фиксируем вводные, согласовываем ожидания и ведем задачу до понятного результата.", 50);
            AddBlock(pages, blocks, slug, "cta-title", "Заголовок CTA", "Готовы обсудить сотрудничество?", 60);
            AddBlock(pages, blocks, slug, "cta-description", "Описание CTA", "Расскажите о формате работы, объекте или канале продаж, а мы предложим следующий шаг.", 70);
        }

        if (blocks.Count == 0)
        {
            return;
        }

        dbContext.PageContentBlocks.AddRange(blocks);
        await dbContext.SaveChangesAsync();
    }

    private static void AddBlock(
        IReadOnlyDictionary<string, SitePage> pages,
        ICollection<PageContentBlock> blocks,
        string pageSlug,
        string key,
        string label,
        string value,
        int sortOrder)
    {
        if (!pages.TryGetValue(pageSlug, out var page))
        {
            return;
        }

        var exists = page.ContentBlocks.Any(block => block.Key == key);
        if (exists)
        {
            return;
        }

        blocks.Add(new PageContentBlock
        {
            SitePageId = page.Id,
            Key = key,
            Label = label,
            Value = value,
            SortOrder = sortOrder
        });
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
