using System.Threading.Tasks;

namespace notion_telegram_bot.Services
{
    public interface ITaskProcessor
    {
        Task ProcessTasksAsync();
    }
}
