using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notion_telegram_bot.Models
{
    public class NotionTask
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? DueDate { get; set; }
        // Puedes agregar más propiedades según lo necesites
    }
}
