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
        await SeedHomeTestimonialsAsync(dbContext);
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
        var connection = dbContext.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            await connection.OpenAsync();
        }

        try
        {
            if (!await HasTableAsync(dbContext, "PageContentBlocks"))
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

            if (!await HasTableAsync(dbContext, "HomeTestimonials"))
            {
                await using var createTestimonialsCommand = connection.CreateCommand();
                createTestimonialsCommand.CommandText = """
                    CREATE TABLE "HomeTestimonials" (
                        "Id" INTEGER NOT NULL CONSTRAINT "PK_HomeTestimonials" PRIMARY KEY AUTOINCREMENT,
                        "Title" TEXT NOT NULL,
                        "Text" TEXT NOT NULL,
                        "Author" TEXT NULL,
                        "ImageUrl" TEXT NOT NULL,
                        "AltText" TEXT NULL,
                        "SortOrder" INTEGER NOT NULL,
                        "IsPublished" INTEGER NOT NULL
                    );
                    """;
                await createTestimonialsCommand.ExecuteNonQueryAsync();
            }
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
        await Task.CompletedTask;
    }

    private static async Task SeedContentBlocksAsync(ComfortRoomsDbContext dbContext)
    {
        var pages = await dbContext.SitePages
            .Include(page => page.ContentBlocks)
            .ToDictionaryAsync(page => page.Slug);
        var blocks = new List<PageContentBlock>();

        AddBlock(pages, blocks, PageSlugs.Home, "hero-eyebrow", "Hero: верхняя надпись", "Премиальный свет для интерьеров", 10);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-eyebrow-color", "Hero: цвет верхней надписи", "text-accent-gold", 20);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-background", "Hero: фон блока", "surface-cream", 25);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-title", "Hero: главный заголовок", "Свет, который становится частью архитектуры", 30);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-title-color", "Hero: цвет главного заголовка", "text-accent-charcoal", 35);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-description", "Hero: описание", "Comfort Rooms создает выразительные световые решения для частных интерьеров, дизайнерских проектов, салонов и онлайн-партнеров.", 40);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-description-color", "Hero: цвет описания", "text-accent-warm-gray", 45);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-primary-button-text", "Hero: текст первой кнопки", "Изготовление под заказ", 50);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-primary-button-style", "Hero: цвет первой кнопки", "button--primary", 60);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-secondary-button-text", "Hero: текст второй кнопки", "Связаться", 70);
        AddBlock(pages, blocks, PageSlugs.Home, "hero-secondary-button-style", "Hero: цвет второй кнопки", "button--secondary", 80);
        AddBlock(pages, blocks, PageSlugs.Home, "directions-background", "Направления: фон блока", "surface-white", 85);
        AddBlock(pages, blocks, PageSlugs.Home, "directions-eyebrow", "Направления: верхняя надпись", "Направления", 90);
        AddBlock(pages, blocks, PageSlugs.Home, "directions-eyebrow-color", "Направления: цвет верхней надписи", "text-accent-gold", 95);
        AddBlock(pages, blocks, PageSlugs.Home, "directions-title", "Направления: заголовок", "Основные разделы сайта", 100);
        AddBlock(pages, blocks, PageSlugs.Home, "directions-title-color", "Направления: цвет заголовка", "text-accent-charcoal", 105);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-custom-title", "Карточка: люстры под заказ — заголовок", "Люстры под заказ", 110);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-custom-description", "Карточка: люстры под заказ — описание", "Индивидуальные светильники по чертежам, фото, эскизам и дизайнерским задачам.", 120);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-designers-title", "Карточка: дизайнерам — заголовок", "Дизайнерам", 130);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-designers-description", "Карточка: дизайнерам — описание", "Партнерство для интерьерных проектов, комплектации объектов и авторских изделий.", 140);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-shops-title", "Карточка: магазинам — заголовок", "Магазинам", 150);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-shops-description", "Карточка: магазинам — описание", "Структура для салонов света, розничных партнеров и демонстрационных коллекций.", 160);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-ecommerce-title", "Карточка: интернет-магазинам — заголовок", "Интернет-магазинам", 170);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-ecommerce-description", "Карточка: интернет-магазинам — описание", "Контентная и коммерческая база для будущего e-commerce сотрудничества.", 180);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-wholesale-title", "Карточка: оптовым партнерам — заголовок", "Оптовым партнерам", 190);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-wholesale-description", "Карточка: оптовым партнерам — описание", "Условия для поставок, складских остатков, регионов и регулярных закупок.", 200);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-retail-title", "Карточка: розничным клиентам — заголовок", "Розничным клиентам", 210);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-retail-description", "Карточка: розничным клиентам — описание", "Подбор светильников для дома, покупка online/offline, доставка и монтаж.", 220);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-tenders-title", "Карточка: тендеры — заголовок", "Тендеры и аукционы", 230);
        AddBlock(pages, blocks, PageSlugs.Home, "direction-tenders-description", "Карточка: тендеры — описание", "Обсуждаем проектные поставки, комплектацию объектов и документацию.", 240);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-background", "Подход: фон блока", "surface-cream", 245);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-eyebrow", "Подход: верхняя надпись", "Подход", 250);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-eyebrow-color", "Подход: цвет верхней надписи", "text-accent-gold", 255);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-title", "Подход: заголовок", "Единый стиль для всех страниц", 260);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-title-color", "Подход: цвет заголовка", "text-accent-charcoal", 265);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-description", "Подход: описание", "Визуальная система строится вокруг готовой страницы «Изготовление люстр под заказ»: светлый фон, золотые акценты, крупная типографика, аккуратные карточки, галереи и спокойные анимации.", 270);
        AddBlock(pages, blocks, PageSlugs.Home, "approach-description-color", "Подход: цвет описания", "text-accent-warm-gray", 275);
        AddBlock(pages, blocks, PageSlugs.Home, "feature-gallery-title", "Блок подхода: галереи — заголовок", "Галереи из админки", 280);
        AddBlock(pages, blocks, PageSlugs.Home, "feature-gallery-description", "Блок подхода: галереи — описание", "Изображения страниц хранятся в базе и управляются отдельно по разделам.", 290);
        AddBlock(pages, blocks, PageSlugs.Home, "feature-leads-title", "Блок подхода: заявки — заголовок", "Заявки сохраняются", 300);
        AddBlock(pages, blocks, PageSlugs.Home, "feature-leads-description", "Блок подхода: заявки — описание", "Форма на странице «под заказ» уже пишет обращения в SQLite.", 310);
        AddBlock(pages, blocks, PageSlugs.Home, "feature-ready-title", "Блок подхода: расширение — заголовок", "Готово к расширению", 320);
        AddBlock(pages, blocks, PageSlugs.Home, "feature-ready-description", "Блок подхода: расширение — описание", "Сервисы хранения изображений сразу разделены под будущий переход на S3.", 330);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-background", "CTA: фон блока", "surface-cream", 335);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-eyebrow", "CTA: верхняя надпись", "Проектирование света", 340);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-eyebrow-color", "CTA: цвет верхней надписи", "text-accent-gold", 345);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-title", "CTA: заголовок", "Нужен светильник под конкретный интерьер?", 350);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-title-color", "CTA: цвет заголовка", "text-accent-charcoal", 355);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-description", "CTA: описание", "Оставьте заявку на расчет индивидуального изделия или перейдите к странице с подробным процессом работы.", 360);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-description-color", "CTA: цвет описания", "text-accent-warm-gray", 365);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-button-text", "CTA: текст кнопки", "Оставить заявку", 370);
        AddBlock(pages, blocks, PageSlugs.Home, "cta-button-style", "CTA: цвет кнопки", "button--primary", 380);
        AddBlock(pages, blocks, PageSlugs.Home, "testimonials-background", "Отзывы: фон блока", "surface-white", 385);
        AddBlock(pages, blocks, PageSlugs.Home, "testimonials-eyebrow", "Отзывы: верхняя надпись", "Отзывы", 390);
        AddBlock(pages, blocks, PageSlugs.Home, "testimonials-eyebrow-color", "Отзывы: цвет верхней надписи", "text-accent-gold", 395);
        AddBlock(pages, blocks, PageSlugs.Home, "testimonials-title", "Отзывы: заголовок", "Что говорят клиенты и партнеры", 400);
        AddBlock(pages, blocks, PageSlugs.Home, "testimonials-title-color", "Отзывы: цвет заголовка", "text-accent-charcoal", 405);
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
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "contract-title", "Заголовок этапов договора", "Этапы договора и согласований", 120);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "contract-description", "Описание этапов договора", "Процесс строится так, чтобы заказчик, дизайнер и производство одинаково понимали сроки, ответственность и результат.", 130);
        AddBlock(pages, blocks, PageSlugs.CustomOrder, "materials-title", "Заголовок материалов", "Светильник под интерьер, а не наоборот", 140);
        AddBlock(pages, blocks, PageSlugs.Cooperation, "hero-title", "Заголовок страницы", "Сотрудничество", 10);
        AddBlock(pages, blocks, PageSlugs.Cooperation, "hero-description", "Описание страницы", "Собрали основные форматы работы с дизайнерами, магазинами, интернет-партнерами, оптовыми клиентами и частными покупателями.", 20);
        AddBlock(pages, blocks, PageSlugs.Cooperation, "help-title", "Заголовок возможностей", "Направления сотрудничества", 30);
        AddBlock(pages, blocks, PageSlugs.Cooperation, "process-title", "Заголовок процесса", "Как выбрать формат", 40);
        AddBlock(pages, blocks, PageSlugs.Cooperation, "process-description", "Описание процесса", "Выберите направление или опишите задачу, а мы предложим материалы, документы и следующий шаг.", 50);
        AddBlock(pages, blocks, PageSlugs.Cooperation, "cta-title", "Заголовок CTA", "Не знаете, с чего начать?", 60);
        AddBlock(pages, blocks, PageSlugs.Cooperation, "cta-description", "Описание CTA", "Оставьте заявку, и мы направим обращение в подходящий формат сотрудничества.", 70);
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

    private static Task SeedHomeTestimonialsAsync(ComfortRoomsDbContext dbContext)
    {
        return Task.CompletedTask;
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
