using System;
using System.ComponentModel.DataAnnotations;

namespace notion_telegram_bot.Models
{
    public class NotifiedTask
    {
        [Key]
        public int Id { get; set; }
        // Guardamos el Id de la tarea en Notion para identificarla
        public string NotionTaskId { get; set; }
        // La fecha y hora en que se notificó
        public DateTime NotifiedAt { get; set; }
    }
}
