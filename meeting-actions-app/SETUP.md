# Meeting Actions API - Setup

## Prerequisitos

- .NET 10 SDK
- SQL Server LocalDB (incluido con Visual Studio) o SQL Server Express
- Visual Studio 2022 o Rider (o VS Code con extensión C#)

## Base de datos

### 1. Verificar SQL Server LocalDB

Si usas Visual Studio, LocalDB ya está instalado. Verifica con:

```powershell
sqllocaldb info
```

Si no está instalado, descarga SQL Server Express desde:
https://www.microsoft.com/sql-server/sql-server-downloads

### 2. Configurar connection string (opcional)

La configuración por defecto usa LocalDB. Si necesitas cambiarla, edita `src/MeetingActions.Api/appsettings.json`:

**LocalDB (default):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MeetingActions;Trusted_Connection=True;TrustServerCertificate=True"
}
```

**SQL Server con autenticación Windows:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MeetingActions;Trusted_Connection=True;TrustServerCertificate=True"
}
```

**SQL Server con usuario/password:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MeetingActions;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True"
}
```

### 3. Crear base de datos y aplicar migraciones

**Nota:** Con SQL Server LocalDB, EF Core creará automáticamente la base de datos cuando ejecutes `Update-Database`. No necesitas crearla manualmente.

#### Opción A: Package Manager Console (Visual Studio)

Abre **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console):

```powershell
# Asegúrate de seleccionar MeetingActions.Api como Default project en el dropdown

# Crear migración inicial
Add-Migration InitialCreate -Project MeetingActions.Api

# Aplicar migración a la base de datos
Update-Database -Project MeetingActions.Api
```

#### Opción B: .NET CLI

```powershell
cd src\MeetingActions.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Si no tienes `dotnet ef` instalado:

```powershell
dotnet tool install --global dotnet-ef
```

## Ejecutar la API

Desde la raíz del monorepo:

```powershell
cd src\MeetingActions.Api
dotnet run
```

O desde Visual Studio: F5 (Debug) o Ctrl+F5 (Run without debugging)

La API estará disponible en:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger

## Probar endpoints

### 1. Crear un job (POST /v1/jobs)

```powershell
$body = @{
    meetingType = "daily"
    inputType = 0
    transcriptText = "María: hoy prioricemos terminar autenticación antes del viernes."
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/v1/jobs" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body `
    -SkipCertificateCheck
```

**Respuesta esperada:**
```json
{
  "jobId": "550e8400-e29b-41d4-a716-446655440000"
}
```

### 2. Consultar estado del job (GET /v1/jobs/{jobId})

```powershell
Invoke-RestMethod -Uri "https://localhost:5001/v1/jobs/550e8400-e29b-41d4-a716-446655440000" `
    -SkipCertificateCheck
```

**Respuesta esperada:**
```json
{
  "jobId": "550e8400-e29b-41d4-a716-446655440000",
  "status": 0,
  "createdAtUtc": "2026-01-11T10:30:00Z",
  "updatedAtUtc": "2026-01-11T10:30:00Z",
  "errorMessage": null
}
```

**Estados:**
- `0` = Pending
- `1` = Processing
- `2` = Done
- `3` = Failed

### 3. Completar job manualmente (POST /v1/jobs/{jobId}/_dev/complete)

**⚠️ Solo para desarrollo - simula el procesamiento del worker**

```powershell
$resultJson = @{
    meeting_type = "daily"
    decisions = @("priorizar autenticación")
    actions = @()
    implicit_dates = @()
    risks_open_questions = @()
    notes_ignored = @()
} | ConvertTo-Json -Compress

$body = @{
    resultJson = $resultJson
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/v1/jobs/550e8400-e29b-41d4-a716-446655440000/_dev/complete" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body `
    -SkipCertificateCheck
```

**Respuesta esperada:**
```json
{
  "message": "Job completed successfully"
}
```

### 4. Obtener resultado (GET /v1/jobs/{jobId}/result)

```powershell
Invoke-RestMethod -Uri "https://localhost:5001/v1/jobs/550e8400-e29b-41d4-a716-446655440000/result" `
    -SkipCertificateCheck
```

**Respuesta esperada:**
```json
{
  "jobId": "550e8400-e29b-41d4-a716-446655440000",
  "resultJson": "{\"meeting_type\":\"daily\",\"decisions\":[\"priorizar autenticación\"],\"actions\":[]}"
}
```

**Códigos de error:**
- `404` - Job no encontrado o resultado no existe
- `409 Conflict` - Job no está completado (status != Done)

## Validaciones

El endpoint `POST /v1/jobs` valida:

✅ `meetingType` requerido (máx 30 caracteres)  
✅ `inputType` debe ser 0 (Text), 1 (Audio) o 2 (Video)  
✅ Si `inputType = 0 (Text)`, entonces `transcriptText` es requerido (mín 10 caracteres)

**Ejemplo de error de validación:**

```powershell
$body = @{
    meetingType = "daily"
    inputType = 0
    transcriptText = "corto"  # < 10 caracteres
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/v1/jobs" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body `
    -SkipCertificateCheck
```

**Respuesta:**
```json
{
  "error": "transcriptText must be at least 10 characters"
}
```

## Comandos útiles de EF Core

### Package Manager Console

```powershell
# Ver migraciones aplicadas
Get-Migration -Project MeetingActions.Api

# Revertir a migración anterior
Update-Database -Migration <NombreMigracionAnterior> -Project MeetingActions.Api

# Eliminar última migración (solo si NO se aplicó)
Remove-Migration -Project MeetingActions.Api

# Generar script SQL de todas las migraciones
Script-Migration -Project MeetingActions.Api

# Generar script SQL desde una migración específica
Script-Migration -From <MigracionInicial> -To <MigracionFinal> -Project MeetingActions.Api
```

### .NET CLI

```powershell
cd src\MeetingActions.Api

# Ver migraciones
dotnet ef migrations list

# Revertir migración
dotnet ef database update <NombreMigracionAnterior>

# Eliminar última migración
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script
```

## Estructura del proyecto

```
meeting-actions-app/
├── MeetingActions.sln
├── SETUP.md                    # Este archivo
├── .gitignore
└── src/
    ├── MeetingActions.Contracts/
    │   ├── Enums/
    │   │   ├── InputType.cs
    │   │   └── JobStatus.cs
    │   ├── Requests/
    │   │   ├── CreateJobRequest.cs
    │   │   └── CompleteJobRequest.cs
    │   └── Responses/
    │       ├── CreateJobResponse.cs
    │       ├── JobStatusResponse.cs
    │       └── JobResultResponse.cs
    └── MeetingActions.Api/
        ├── Data/
        │   ├── MeetingActionsDbContext.cs
        │   └── Migrations/             # Generado por EF
        ├── Entities/
        │   ├── Job.cs
        │   └── JobResult.cs
        ├── Validators/
        │   └── CreateJobRequestValidator.cs
        ├── appsettings.json
        ├── appsettings.Development.json
        └── Program.cs
```

## Próximos pasos

Esta iteración implementa **SOLO la API + DB + CRUD básico**.

**NO incluye:**
- ❌ Worker de procesamiento real
- ❌ Integración con Azure OpenAI
- ❌ Whisper / transcripción de audio
- ❌ Storage de archivos
- ❌ Autenticación / autorización
- ❌ UI / Frontend
- ❌ Billing

Para simular el procesamiento, usa el endpoint `POST /v1/jobs/{jobId}/_dev/complete`.

En la próxima iteración se agregará el worker que:
1. Escucha jobs con status `Pending`
2. Llama a Azure OpenAI (reutilizando código del spike)
3. Guarda el resultado en `JobResult`
4. Actualiza el status a `Done` o `Failed`
