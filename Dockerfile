# Etapa de compilación: Usa una imagen base de .NET SDK 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa de ejecución: Usa la imagen runtime de .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "notion_telegram_bot.dll"]
