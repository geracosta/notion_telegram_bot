using Microsoft.EntityFrameworkCore;
using notion_telegram_bot.Data;
using notion_telegram_bot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Registrar el DbContext para SQLite
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        // Registro del servicio en segundo plano
        services.AddHostedService<Worker>();

        // Registra otros servicios
        services.AddSingleton<INotionService, NotionService>();
        services.AddSingleton<ITelegramService, TelegramService>();
        services.AddScoped<ITaskProcessor, TaskProcessor>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

// Aplicar migraciones automáticamente
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

host.Run();
