using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notion.Client;
using notion_telegram_bot.Models;

namespace notion_telegram_bot.Services
{
    public class NotionService : INotionService
    {
        private readonly ILogger<NotionService> _logger;
        private readonly IConfiguration _configuration;
        private readonly NotionClient _client;
        private readonly string _databaseId;

        public NotionService(ILogger<NotionService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Lee el token y el Database ID desde la configuración
            string token = _configuration["Notion:IntegrationToken"];
            _databaseId = _configuration["Notion:DatabaseId"];

            // Inicializa el cliente de Notion usando el factory para configurar correctamente los parámetros
            _client = NotionClientFactory.Create(new ClientOptions { AuthToken = token });
        }

        public async Task<IEnumerable<NotionTask>> GetPendingTasksAsync()
        {
            var tasks = new List<NotionTask>();

            try
            {
                _logger.LogInformation("Consultando la base de datos de Notion a las {Time}", DateTime.Now);
                var queryParameters = new DatabasesQueryParameters();
                var queryResponse = await _client.Databases.QueryAsync(_databaseId, queryParameters);
                _logger.LogInformation("Se han recibido {Count} registros de Notion.", queryResponse.Results.Count);

                foreach (var page in queryResponse.Results)
                {
                    string taskName = "";
                    DateTime? dueDate = null;

                    // Extrae la propiedad "Tarea" (de tipo Title)
                    if (page.Properties.TryGetValue("Tarea", out var titleProperty))
                    {
                        if (titleProperty is TitlePropertyValue titleValue)
                        {
                            taskName = string.Join("", titleValue.Title.Select(t => t.PlainText));
                        }
                    }

                    // Extrae la propiedad "Fecha" (de tipo Date) incluyendo fecha y hora
                    if (page.Properties.TryGetValue("Fecha", out var dateProperty))
                    {
                        if (dateProperty is DatePropertyValue dateValue)
                        {
                            if (dateValue.Date.Start != null)
                            {
                                dueDate = dateValue.Date.Start.Value;
                            }
                        }
                    }

                    // Filtrado: solo agregar tareas que sean de hoy y se vencen en menos de una hora
                    if (dueDate.HasValue)
                    {
                        // Convierte la fecha obtenida (probablemente en UTC) a hora local
                        DateTime localDueDate = dueDate.Value.ToLocalTime();
                        DateTime now = DateTime.Now;
                        DateTime oneHourLater = now.AddHours(1);

                        // Comprueba que la tarea sea para hoy y que se encuentre entre el momento actual y una hora después
                        if (localDueDate.Date == now.Date && localDueDate >= now && localDueDate <= oneHourLater)
                        {
                            tasks.Add(new NotionTask
                            {
                                Id = page.Id,
                                Name = taskName,
                                DueDate = localDueDate
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar la base de datos de Notion.");
            }

            return tasks;
        }
    }
}
