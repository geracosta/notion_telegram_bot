using Microsoft.EntityFrameworkCore;
using notion_telegram_bot.Models;

namespace notion_telegram_bot.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<NotifiedTask> NotifiedTasks { get; set; }
    }
}
