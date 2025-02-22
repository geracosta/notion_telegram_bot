using Microsoft.EntityFrameworkCore;
using notion_telegram_bot.Data;
using notion_telegram_bot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Registrar el DbContext para SQLite
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        // Registro del servicio en segundo plano
        services.AddHostedService<Worker>();

        // Registra los servicios de Notion y Telegram (estas pueden ser singleton o lo que convenga)
        services.AddSingleton<INotionService, NotionService>();
        services.AddSingleton<ITelegramService, TelegramService>();

        // Registrar TaskProcessor como Scoped, ya que consume AppDbContext (scoped)
        services.AddScoped<ITaskProcessor, TaskProcessor>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build()
    .Run();
