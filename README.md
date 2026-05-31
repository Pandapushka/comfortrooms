# Comfort Rooms

ASP.NET Core MVC-проект для нового сайта Comfort Rooms.

Проект копирует структуру существующего сайта, но визуально ориентируется на новую светлую premium/luxury страницу «Изготовление люстр под заказ».

## Технологии

- .NET 8
- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- SQLite на старте
- Архитектура хранения изображений через сервисы для будущего перехода на S3

## Запуск

```bash
/Users/panda.pushka/.dotnet/dotnet restore
/Users/panda.pushka/.dotnet/dotnet run --project ComfortRooms.csproj
```

Локальный адрес по профилю проекта:

```text
http://localhost:5110
```

При ручном запуске на фиксированном порту:

```bash
ASPNETCORE_URLS=http://localhost:5000 /Users/panda.pushka/.dotnet/dotnet run --project ComfortRooms.csproj --no-launch-profile
```

Если запуск идет из ограниченного терминального окружения и сервер долго не открывает порт, можно отключить наблюдение за изменениями конфигурации:

```bash
DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false ASPNETCORE_URLS=http://localhost:5000 /Users/panda.pushka/.dotnet/dotnet run --project ComfortRooms.csproj --no-launch-profile
```

## Основные страницы

- `/` — главная
- `/sotrudnichestvo/na-zakaz` — изготовление люстр под заказ
- `/sotrudnichestvo/opt` — оптовым партнерам
- `/sotrudnichestvo/roznitsa` — розничным клиентам
- `/sotrudnichestvo/dizayneram` — дизайнерам
- `/sotrudnichestvo/magazinam` — магазинам
- `/sotrudnichestvo/internet-magazinam` — интернет-магазинам
- `/o-kompanii` — о компании
- `/kontakty` — контакты

Пункт «Интернет-магазин» в меню ведет на внешний сайт Comfort Rooms.

## Админ-панель

- `/admin/setup` — первичное создание администратора, доступно только пока в базе нет администратора
- `/admin/login` — вход администратора
- `/admin` — разделы сайта
- `/admin/leads` — заявки
- `/admin/pages/{slug}/content` — управление текстами страницы
- `/admin/pages/{slug}/images` — управление изображениями страницы

Обычные пользователи не регистрируются и не входят на сайт.

Hero-заголовки и описания публичных страниц берутся из таблицы `PageContentBlocks`, поэтому изменения в разделе «Тексты» отображаются на сайте без правки Razor-файлов.

## Изображения

На первом этапе изображения хранятся в:

```text
wwwroot/uploads
```

Работа с файлами идет через:

- `IImageStorageService`
- `LocalImageStorageService`

Это оставляет путь для будущего `S3ImageStorageService` без переписывания контроллеров.

## База данных

SQLite-файл создается в:

```text
App_Data/comfortrooms.db
```

Файл базы и WAL/SHM-файлы исключены из Git.

Миграции находятся в:

```text
Migrations
```

Локальный инструмент EF:

```bash
/Users/panda.pushka/.dotnet/dotnet tool restore
/Users/panda.pushka/.dotnet/dotnet tool run dotnet-ef database update
```

## Git

Основной remote:

```text
https://github.com/Pandapushka/comfortrooms.git
```

Коммиты и push выполняются только после явного разрешения владельца проекта.

## Рабочий журнал

Контекст разработки фиксируется в:

```text
WORKLOG.md
```

Перед продолжением работы после паузы или перезагрузки сначала прочитать этот файл.
