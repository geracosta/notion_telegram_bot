using Microsoft.EntityFrameworkCore;
using notion_telegram_bot.Data;
using notion_telegram_bot.Models;

namespace notion_telegram_bot.Services
{
    public class TaskProcessor : ITaskProcessor
    {
        private readonly INotionService _notionService;
        private readonly ITelegramService _telegramService;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TaskProcessor> _logger;

        public TaskProcessor(INotionService notionService, ITelegramService telegramService, AppDbContext dbContext, ILogger<TaskProcessor> logger)
        {
            _notionService = notionService;
            _telegramService = telegramService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ProcessTasksAsync()
        {
            // Obtiene las tareas pendientes desde Notion
            var tasks = await _notionService.GetPendingTasksAsync();

            foreach (var task in tasks)
            {
                // Verifica si la tarea ya fue notificada
                bool alreadyNotified = await _dbContext.NotifiedTasks.AnyAsync(nt => nt.NotionTaskId == task.Id);
                if (alreadyNotified)
                {
                    _logger.LogInformation("La tarea {taskName} ya fue notificada previamente.", task.Name);
                    continue;
                }

                // Enviar notificación vía Telegram
                await _telegramService.SendNotificationAsync(task);
                _logger.LogInformation("Notificado la tarea: {taskName}", task.Name);

                // Registrar en la base de datos que la tarea fue notificada
                var notifiedTask = new NotifiedTask
                {
                    NotionTaskId = task.Id,
                    NotifiedAt = DateTime.Now
                };
                _dbContext.NotifiedTasks.Add(notifiedTask);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
