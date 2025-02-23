# 🤦 Notion Telegram Bot

Este bot de Telegram se conecta con Notion para notificar tareas pendientes y próximas a vencer. Se ejecuta en **Fly.io** y usa **SQLite** para almacenar tareas notificadas.

---

## 🚀 1. Configuración del Proyecto

### 🔹 Requisitos Previos
Antes de empezar, asegúrate de tener instalado:
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Fly.io CLI](https://fly.io/docs/hands-on/install-flyctl/)
- Cuenta en [Telegram](https://telegram.org/) y haber creado un bot con [BotFather](https://t.me/botfather)
- [Cuenta en Notion](https://notion.so) y un **token de integración**

### 🔹 Clonar el Proyecto
```bash
git clone https://github.com/tu-usuario/notion-telegram-bot.git
cd notion-telegram-bot
```

### 🔹 Configurar Variables de Entorno
Crea un archivo **`appsettings.json`** en la raíz del proyecto con:

```json
{
  "Notion": {
    "IntegrationToken": "TU_TOKEN_DE_NOTION",
    "DatabaseId": "TU_DATABASE_ID"
  },
  "Telegram": {
    "BotToken": "TU_BOT_TOKEN",
    "ChatId": "TU_CHAT_ID"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/data/notion_telegram_bot.db"
  }
}
```
📌 **Reemplaza los valores con tus credenciales reales.**

---

## 🛠️ 2. Configurar la Base de Datos SQLite
SQLite es una base de datos ligera que no requiere configuración adicional.

1. **Ejecutar las migraciones para crear la base de datos:**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
2. Esto creará la base de datos en `/data/notion_telegram_bot.db`.

📌 **Nota:** En Fly.io, si la máquina se apaga, la base de datos en `/data` se perderá. Para hacerla persistente, ver la sección **"Cómo mantener la base de datos persistente"**.

---

## 🐳 3. Configurar Docker
Para construir y probar la aplicación en un contenedor localmente:

```bash
docker build -t notion_telegram_bot .
docker run -d --name notion_telegram_bot notion_telegram_bot
```

Si necesitas ver los logs:
```bash
docker logs -f notion_telegram_bot
```

---

## ☁️ 4. Desplegar en Fly.io
### 🔹 1. Autenticarse en Fly.io
```bash
fly auth login
```

### 🔹 2. Crear la Aplicación
```bash
fly launch
```
- **Selecciona un nombre para la app.**
- **Escoge la región más cercana a tus usuarios.**
- **No crees una base de datos en Fly.io (ya usamos SQLite).**

### 🔹 3. Configurar el Escalado
Para asegurarte de que Fly.io mantenga la app corriendo:
```bash
fly scale count 1
```

### 🔹 4. Configurar Variables de Entorno en Fly.io
Ejecuta estos comandos en la terminal para definir las variables de entorno necesarias:

```bash
fly secrets set NOTION_INTEGRATION_TOKEN="TU_TOKEN_DE_NOTION"
fly secrets set NOTION_DATABASE_ID="TU_DATABASE_ID"
fly secrets set TELEGRAM_BOT_TOKEN="TU_BOT_TOKEN"
fly secrets set TELEGRAM_CHAT_ID="TU_CHAT_ID"
```

### 🔹 5. Desplegar la Aplicación en Fly.io
```bash
fly deploy
```

### 🔹 6. Ver Logs en Fly.io
```bash
fly logs
```

---

## 🛠️ 5. Cómo Mantener la Base de Datos SQLite Persistente
Para evitar que la base de datos se pierda al reiniciar Fly.io, monta un volumen persistente:

1. **Crear un volumen en Fly.io:**
   ```bash
   fly volumes create notion-db --size 1
   ```

2. **Editar `fly.toml` y montar el volumen:**
   ```toml
   [[mounts]]
   source = "notion-db"
   destination = "/data"
   ```

3. **Volver a desplegar la aplicación con el volumen configurado:**
   ```bash
   fly deploy
   ```

---

## 🛠️ 6. Actualizar la Aplicación
Si realizas cambios en el código, simplemente vuelve a ejecutar:
```bash
fly deploy
```

---

## ❓ 7. Preguntas Frecuentes
### 🛑 Mi bot no envía mensajes, ¿qué hago?
1. Verifica que `appsettings.json` tiene los **tokens correctos**.
2. Revisa los **logs** con `fly logs` para ver si hay errores.
3. Confirma que la base de datos SQLite está guardando tareas notificadas.

### ⚡ ¿Cómo accedo a la base de datos en Fly.io?
1. Conéctate a la máquina:
   ```bash
   fly ssh console
   ```
2. Ve a la carpeta `/data` y verifica que la base de datos existe:
   ```bash
   cd /data
   ls -l
   ```
3. Para abrir SQLite y ver los datos:
   ```bash
   sqlite3 notion_telegram_bot.db
   ```
4. Para listar las tablas:
   ```sql
   .tables
   ```

### 🔄 ¿Cómo actualizo la base de datos si cambio el modelo?
Ejecuta:
```bash
dotnet ef migrations add NuevaMigracion
dotnet ef database update
```
Luego, despliega la nueva versión con:
```bash
fly deploy
```

---

## 🎉 ¡Listo! Tu Bot de Notion en Telegram está en Producción 🚀

Si necesitas hacer cambios, simplemente actualiza el código y despliega con `fly deploy`.  
Si tienes dudas o mejoras, crea un **issue** en GitHub o contáctame. 😃🔥

