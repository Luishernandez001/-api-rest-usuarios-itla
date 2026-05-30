# API REST de Usuarios

## Descripción
API REST desarrollada con ASP.NET Core Web API, Entity Framework Core y SQL Server usando Code First.

## Requisitos
- .NET 6 o superior
- SQL Server o LocalDB
- Visual Studio / VS Code

## Paquetes usados
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

## Cadena de conexión
Configurar en `appsettings.json`.

## Migraciones
Ejecutar:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update