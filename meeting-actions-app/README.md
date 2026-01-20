# Meeting Actions - Monorepo

Micro-SaaS para extraer accionables de reuniones usando Azure OpenAI.

## Proyectos

- **MeetingActions.Api** - ASP.NET Core Web API (.NET 10) con Minimal APIs
- **MeetingActions.Contracts** - DTOs y enums compartidos

## Stack tecnológico

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core 10
- SQL Server / LocalDB
- Swagger/OpenAPI

## Quick Start

Ver [SETUP.md](./SETUP.md) para instrucciones detalladas.

```powershell
# 1. Aplicar migraciones (LocalDB crea la DB automáticamente)
cd src\MeetingActions.Api
dotnet ef database update

# 3. Ejecutar API
dotnet run

# 4. Abrir Swagger
# https://localhost:5001/swagger
```

## Estructura

```
meeting-actions-app/
├── MeetingActions.sln
├── README.md
├── SETUP.md
└── src/
    ├── MeetingActions.Api/        # Web API
    └── MeetingActions.Contracts/  # DTOs compartidos
```

## Estado actual

✅ **Iteración 1 completada:**
- CRUD básico de Jobs (crear y consultar)
- Endpoints para obtener estado y resultado
- Endpoint de desarrollo para simular procesamiento
- Validaciones de requests
- PostgreSQL + EF Core con migraciones

🚧 **Próximas iteraciones:**
- Worker de procesamiento con Azure OpenAI
- Transcripción de audio (Whisper)
- Storage de archivos
- Autenticación
- UI / Frontend
