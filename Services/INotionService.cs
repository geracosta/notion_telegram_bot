using System.Collections.Generic;
using System.Threading.Tasks;
using notion_telegram_bot.Models;

namespace notion_telegram_bot.Services
{
    public interface INotionService
    {
        Task<IEnumerable<NotionTask>> GetPendingTasksAsync();
    }
}
