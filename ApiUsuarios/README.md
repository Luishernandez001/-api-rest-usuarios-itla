# API REST de Usuarios

## Descripción
Este proyecto consiste en una API REST desarrollada con ASP.NET Core Web API, Entity Framework Core y SQL Server, utilizando el enfoque Code First.

La API permite gestionar usuarios mediante operaciones CRUD:
- Crear usuarios
- Consultar usuarios
- Actualizar usuarios
- Eliminar usuarios

Además, incluye una validación para evitar que existan usuarios con el mismo correo electrónico.

---

## Tecnologías utilizadas
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger
- Code First

---

## Requisitos
Para ejecutar este proyecto se necesita tener instalado:

- .NET 6 o superior
- SQL Server / SQL Server Express
- Visual Studio 2022, Visual Studio Code o cualquier editor compatible
- Entity Framework Core Tools

---

## Paquetes usados
Instalar los siguientes paquetes NuGet:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
