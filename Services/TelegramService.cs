using System.Threading.Tasks;
using Telegram.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using notion_telegram_bot.Models;
using notion_telegram_bot.Services;

namespace notion_telegram_bot.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TelegramBotClient _botClient;
        private readonly long _chatId;

        public TelegramService(ILogger<TelegramService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Se asume que en appsettings.json se configuran "Telegram:Token" y "Telegram:ChatId"
            var telegramToken = _configuration["Telegram:Token"];
            _chatId = long.Parse(_configuration["Telegram:ChatId"]);
            _botClient = new TelegramBotClient(telegramToken);
        }

        public async Task SendNotificationAsync(NotionTask task)
        {
            string message = $"¡Atención! La tarea '{task.Name}' se vence a las {task.DueDate?.ToShortTimeString()}";
            await _botClient.SendTextMessageAsync(_chatId, message);
            _logger.LogInformation("Mensaje enviado a Telegram para la tarea {taskName}", task.Name);
        }
    }
}
