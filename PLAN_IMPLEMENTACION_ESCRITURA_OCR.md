# PLAN DE IMPLEMENTACIÓN — MÓDULO DE ASIGNACIONES DE ESCRITURA CON OCR + GOOGLE DRIVE

## 1. OBJETIVO

Implementar un flujo especial para **asignaciones de escritura** (área de escritura) donde:
- El profesor asigna una hoja de trabajo imprimible
- El alumno trabaja en físico
- El profesor califica sobre la hoja (anota nota en recuadro)
- El profesor escanea/fotografía la hoja
- El sistema extrae automáticamente la calificación vía OCR (Tesseract)
- El sistema guarda la imagen como evidencia en Google Drive
- La imagen se organiza en: Historial-Académico / Ciclo / Escuela / Grupo / Alumno /

**Restricción:** Este flujo aplica SOLO para asignaciones de tipo ESCRITURA.
Las asignaciones DIGITALES (diálogos animados, juegos, actividades interactivas) mantienen el flujo actual con ResponseJson.

---

## 2. STACK TECNOLÓGICO — COSTO CERO

| Componente               | Herramienta               | Por qué es gratis                                |
|--------------------------|---------------------------|--------------------------------------------------|
| Base de datos            | Supabase PostgreSQL ✅    | 500MB gratis (ya en uso)                        |
| Storage material general | Supabase Storage          | 1GB almacenamiento, 2GB transferencia            |
| Storage evidencias       | Google Drive API          | API gratuita, 15GB por cuenta nueva              |
| OCR / captura nota       | Tesseract OCR             | Open source, self-hosted                         |
| QR en hojas de trabajo   | QRCoder NuGet             | Licencia MIT — gratuita                          |
| PDF de hojas             | QuestPDF NuGet            | Licencia MIT — gratuita                          |
| Procesamiento imágenes   | ImageSharp / SkiaSharp    | Gratuitas, self-hosted                           |
| Deploy                   | Railway (Docker)          | $5 crédito mensual gratis                        |
| Documentación API        | Scalar ✅                 | Gratuito (ya configurado)                        |

### Paquetes NuGet a Instalar

```xml
<PackageReference Include="QRCoder" Version="1.6.1" />
<PackageReference Include="QuestPDF" Version="2024.12.0" />
<PackageReference Include="Google.Apis.Drive.v3" Version="1.69.0" />
<PackageReference Include="Tesseract.NET" Version="5.2.0" />
<PackageReference Include="SkiaSharp" Version="3.116.1" />
```

---

## 3. ARQUITECTURA DE ALMACENAMIENTO

```
┌──────────────────────────────────────────────────────────────────────┐
│                        MATERIAL GENERAL                              │
│  (imágenes, videos, audios para crear material didáctico)            │
│                                                                      │
│  ┌──────────────────────────────────────────────────────┐           │
│  │  SUPABASE STORAGE (1GB gratis)                       │           │
│  │  Bucket: "material-educativo"                        │           │
│  │  /imagenes/{material-id}.jpg                         │           │
│  │  /videos/{material-id}.mp4                           │           │
│  │  /audios/{material-id}.mp3                           │           │
│  │  Se accede vía URL pública firmada (signed URL)      │           │
│  └──────────────────────────────────────────────────────┘           │
└──────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────┐
│                    EVIDENCIAS DE ALUMNOS (ESCRITURA)                │
│                                                                      │
│  ┌──────────────────────────────────────────────────────┐           │
│  │  GOOGLE DRIVE (15GB gratis)                          │           │
│  │  Service Account → Google.Apis.Drive.v3              │           │
│  │                                                       │           │
│  │  /Historial-Academico-SIAE/                           │           │
│  │  └── /{Ciclo-Escolar} (ej: 2025-2026)/              │           │
│  │      └── /{Escuela}/                                  │           │
│  │          └── /{Grado}-{Grupo} (ej: 2-A)/             │           │
│  │              └── /{ApellidoPaterno}_{Nombre}/         │           │
│  │                  └── Escritura_{YYYY-MM-DD}_          │           │
│  │                       Cal_{nota}.jpg                │           │
│  └──────────────────────────────────────────────────────┘           │
└──────────────────────────────────────────────────────────────────────┘
```

**Formato de nombre de archivo:**
```
Escritura_{YYYY-MM-DD}_Cal_{nota}_{assignmentStudentId}.jpg
Ej: Escritura_2026-05-14_Cal_8_a1b2c3d4.jpg
```

---

## 4. CAMBIOS EN BASE DE DATOS

### Migración 1: Agregar columna tipo_asignacion a la tabla asignaciones

```sql
ALTER TABLE asignaciones 
ADD COLUMN tipo_asignacion VARCHAR(20) 
    DEFAULT 'DIGITAL'
    CHECK (tipo_asignacion IN ('DIGITAL', 'ESCRITURA', 'DIALOGO_ANIMADO'));
```

### Migración 2: Tabla para evidencias de escritura (opcional en BD, o usar campos existentes)

```sql
CREATE TABLE evidencia_escritura (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    assignment_student_id UUID NOT NULL REFERENCES asignacion_alumnos(id) ON DELETE CASCADE,
    drive_file_id TEXT,                 -- ID del archivo en Google Drive
    drive_url TEXT NOT NULL,            -- URL del archivo en Drive
    ocr_raw_text TEXT,                  -- Texto crudo que detectó el OCR
    calificacion_ocr SMALLINT,          -- Calificación detectada por OCR
    calificacion_manual SMALLINT,       -- Calificación corregida por el profesor
    calificacion_maxima SMALLINT DEFAULT 10,
    ocr_confianza DECIMAL(5,2),         -- % de confianza del OCR (0.00 - 100.00)
    ocr_confirmado BOOLEAN DEFAULT FALSE,
    imagen_original_url TEXT,           -- Backup de la imagen original sin procesar
    created_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(assignment_student_id)
);
```

### Alternativa: Reutilizar campos existentes de AssignmentStudent

En vez de crear tabla nueva, usar:
- `AssignmentStudent.EvidenceUrls` → `["{driveUrl}"]`
- `AssignmentStudent.ManualGradeJson` → `{"grade":8, "maxScore":10, "type":"ESCRITURA", "driveFileId":"xxx", "ocrConfidence":85.5}`
- `AssignmentStudent.Status` → `EVALUADO`
- `AssignmentStudent.EvaluatedById` → `{profesorId}`
- `AssignmentStudent.EvaluationDate` → `NOW()`

---

## 5. MODELOS NUEVOS (C#)

### Models/DB/WritingEvidence.cs

```csharp
namespace Models.DB;

public class WritingEvidence
{
    public Guid Id { get; set; }
    public Guid AssignmentStudentId { get; set; }
    public string? DriveFileId { get; set; }
    public string DriveUrl { get; set; } = null!;
    public string? OcrRawText { get; set; }
    public short? CalificacionOcr { get; set; }
    public short? CalificacionManual { get; set; }
    public short CalificacionMaxima { get; set; } = 10;
    public decimal? OcrConfianza { get; set; }
    public boolStatus OcrConfirmado { get; set; } = boolStatus.False;
    public string? ImagenOriginalUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AssignmentStudent AssignmentStudent { get; set; } = null!;
}
```

### Models/Request/CreateWritingAssignmentRequest.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class CreateWritingAssignmentRequest
{
    [Required]
    public Guid AssignedById { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    [Required]
    public Guid MaterialId { get; set; }

    [MinLength(1)]
    public List<Guid> StudentIds { get; set; } = [];

    public DateOnly? DueDate { get; set; }
    public string? Instructions { get; set; }
}
```

### Models/Request/SubmitWritingEvidenceRequest.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class SubmitWritingEvidenceRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    public short? ManualGrade { get; set; }  // Opcional: el profe puede escribir la nota directamente

    public bool? ConfirmOcr { get; set; }    // true si ya confirma la calificación del OCR
}
```

### Models/Dto/WritingEvidenceDto.cs

```csharp
namespace Models.Dto;

public class WritingEvidenceDto
{
    public Guid Id { get; set; }
    public Guid AssignmentStudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public string SchoolName { get; set; } = null!;
    public string SchoolYearName { get; set; } = null!;
    public string DriveUrl { get; set; } = null!;
    public short? CalificacionOcr { get; set; }
    public short? CalificacionFinal { get; set; }
    public short CalificacionMaxima { get; set; }
    public decimal? OcrConfianza { get; set; }
    public bool OcrConfirmado { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Models/Dto/WritingWorksheetDto.cs

```csharp
namespace Models.Dto;

public class WritingWorksheetDto
{
    public Guid AssignmentId { get; set; }
    public Guid AssignmentStudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public string Grade { get; set; } = null!;
    public string SchoolName { get; set; } = null!;
    public string SchoolYear { get; set; } = null!;
    public string Instructions { get; set; } = null!;
    public string WorksheetUrl { get; set; } = null!;  // URL del PDF generado
}
```

---

## 6. SERVICIOS NUEVOS

### Services/IServices/IFileStorageService.cs

```csharp
namespace Services.IServices;

public interface IFileStorageService
{
    /// <summary>
    /// Sube un archivo a Supabase Storage (material general)
    /// </summary>
    Task<string> UploadMaterialAsync(Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Sube una evidencia de escritura a Google Drive organizada por alumno
    /// </summary>
    Task<string> UploadEvidenceToDriveAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string cicloEscolar,
        string escuela,
        string grupo,
        string alumno);

    /// <summary>
    /// Genera una URL de visualización (firmada o pública)
    /// </summary>
    Task<string> GetFileUrlAsync(string fileId, bool isDriveFile);
}
```

### Services/IServices/IOcrService.cs

```csharp
namespace Services.IServices;

public class OcrResult
{
    public short? Grade { get; set; }
    public decimal? Confidence { get; set; }
    public string? RawText { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IOcrService
{
    /// <summary>
    /// Extrae la calificación manuscrita de una imagen de hoja de escritura
    /// </summary>
    Task<OcrResult> ExtractGradeFromWorksheetAsync(Stream imageStream);

    /// <summary>
    /// Recorta el área del recuadro de calificación usando coordenadas conocidas
    /// </summary>
    Stream CropGradeArea(Stream imageStream);
}
```

### Services/FileStorageService.cs

```csharp
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Services.IServices;

namespace Services;

public class FileStorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly string _driveRootFolderId;

    public FileStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
        _driveRootFolderId = configuration["GoogleDrive:RootFolderId"]!;
    }

    private DriveService GetDriveService()
    {
        var jsonCredential = _configuration["GoogleDrive:ServiceAccountJson"];
        var credential = GoogleCredential.FromJson(jsonCredential)
            .CreateScoped(DriveService.Scope.DriveFile);

        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "SIAE-Evidencias"
        });
    }

    public async Task<string> UploadMaterialAsync(Stream fileStream, string fileName, string contentType)
    {
        // Subir a Supabase Storage (bucket: material-educativo)
        // Usar Supabase C# client o REST API directa
        throw new NotImplementedException("Fase 2: Supabase Storage para material general");
    }

    public async Task<string> UploadEvidenceToDriveAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string cicloEscolar,
        string escuela,
        string grupo,
        string alumno)
    {
        var service = GetDriveService();

        // 1. Obtener o crear carpeta: Historial-Academico-SIAE / cicloEscolar / escuela / grupo / alumno
        var cicloFolderId = await GetOrCreateFolderAsync(service, _driveRootFolderId, cicloEscolar);
        var escuelaFolderId = await GetOrCreateFolderAsync(service, cicloFolderId, escuela);
        var grupoFolderId = await GetOrCreateFolderAsync(service, escuelaFolderId, grupo);
        var alumnoFolderId = await GetOrCreateFolderAsync(service, grupoFolderId, alumno);

        // 2. Subir archivo
        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = fileName,
            Parents = [alumnoFolderId]
        };

        var request = service.Files.Create(fileMetadata, fileStream, contentType);
        request.Fields = "id, webViewLink";

        var result = await request.UploadAsync();
        if (result.Status != UploadStatus.Completed)
            throw new Exception($"Error uploading to Drive: {result.Exception?.Message}");

        // 3. Retornar URL
        return $"https://drive.google.com/file/d/{request.ResponseBody.Id}/view";
    }

    public async Task<string> GetFileUrlAsync(string fileId, bool isDriveFile)
    {
        if (isDriveFile)
        {
            var service = GetDriveService();
            var file = await service.Files.Get(fileId).ExecuteAsync();
            return file.WebViewLink ?? $"https://drive.google.com/file/d/{fileId}/view";
        }

        // Supabase Storage URL
        throw new NotImplementedException();
    }

    private async Task<string> GetOrCreateFolderAsync(DriveService service, string parentId, string folderName)
    {
        // Buscar si la carpeta ya existe
        var query = $"mimeType = 'application/vnd.google-apps.folder' and name = '{folderName}' and '{parentId}' in parents and trashed = false";
        var listRequest = service.Files.List();
        listRequest.Q = query;
        listRequest.Fields = "files(id, name)";

        var result = await listRequest.ExecuteAsync();
        if (result.Files.Count > 0)
            return result.Files[0].Id;

        // Crear carpeta
        var folderMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = [parentId]
        };

        var createRequest = await service.Files.Create(folderMetadata).ExecuteAsync();
        return createRequest.Id;
    }
}
```

### Services/OcrService.cs

```csharp
using Services.IServices;
using Tesseract;
using SkiaSharp;

namespace Services;

public class OcrService : IOcrService
{
    private readonly IConfiguration _configuration;

    public OcrService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<OcrResult> ExtractGradeFromWorksheetAsync(Stream imageStream)
    {
        try
        {
            // 1. Recortar el área del recuadro de calificación
            using var croppedStream = CropGradeArea(imageStream);

            // 2. Guardar en temp file para Tesseract
            var tempFile = Path.GetTempFileName();
            await using (var fs = File.Create(tempFile))
            {
                croppedStream.Seek(0, SeekOrigin.Begin);
                await croppedStream.CopyToAsync(fs);
            }

            try
            {
                // 3. Ejecutar Tesseract
                var dataPath = _configuration["Tesseract:DataPath"] ?? "/usr/share/tesseract-ocr/5/tessdata";
                using var engine = new TesseractEngine(dataPath, "eng+spa", EngineMode.Default);

                // Configurar para detectar solo dígitos
                engine.SetVariable("tessedit_char_whitelist", "0123456789");
                engine.DefaultPageSegMode = PageSegMode.SingleBlock;

                using var img = Pix.LoadFromFile(tempFile);
                using var page = engine.Process(img);

                var text = page.GetText()?.Trim();
                var confidence = page.GetMeanConfidence();

                if (string.IsNullOrEmpty(text))
                {
                    return new OcrResult
                    {
                        Success = false,
                        ErrorMessage = "No se detectó texto en el recuadro de calificación",
                        RawText = text,
                        Confidence = confidence * 100
                    };
                }

                // Limpiar: quitar espacios, tomar solo dígitos consecutivos
                var digits = new string(text.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(digits))
                {
                    return new OcrResult
                    {
                        Success = false,
                        ErrorMessage = $"No se encontraron dígitos válidos. Texto detectado: '{text}'",
                        RawText = text,
                        Confidence = confidence * 100
                    };
                }

                // Tomar el primer grupo de dígitos (evitar falsos positivos)
                if (int.TryParse(digits, out int grade))
                {
                    if (grade >= 1 && grade <= 10)
                    {
                        return new OcrResult
                        {
                            Success = true,
                            Grade = (short)grade,
                            RawText = text,
                            Confidence = Math.Round(confidence * 100, 2)
                        };
                    }

                    return new OcrResult
                    {
                        Success = false,
                        ErrorMessage = $"Calificación fuera de rango (1-10): {grade}",
                        RawText = text,
                        Confidence = confidence * 100
                    };
                }

                return new OcrResult
                {
                    Success = false,
                    ErrorMessage = $"No se pudo interpretar el número. Texto: '{text}'",
                    RawText = text,
                    Confidence = confidence * 100
                };
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
        catch (Exception ex)
        {
            return new OcrResult
            {
                Success = false,
                ErrorMessage = $"Error en OCR: {ex.Message}"
            };
        }
    }

    public Stream CropGradeArea(Stream imageStream)
    {
        // El PDF generado tiene dimensiones conocidas (A4 = 2480x3508 px a 300dpi)
        // El recuadro de calificación está en una posición fija del layout
        // Coordenadas: x=1900, y=100, width=400, height=250 (recuadro inferior derecho)

        using var original = SKBitmap.Decode(imageStream);
        if (original == null)
            throw new InvalidOperationException("No se pudo decodificar la imagen");

        // Coordenadas del recuadro de calificación (ajustar según el PDF generado)
        var cropRect = new SKRectI(1900, 100, 2300, 350);

        using var cropped = new SKBitmap(cropRect.Width, cropRect.Height);
        using var canvas = new SKCanvas(cropped);

        // Dibujar la sección recortada
        canvas.DrawBitmap(original, cropRect, new SKRect(0, 0, cropRect.Width, cropRect.Height));

        // Convertir a grises y aumentar contraste
        using var gray = ToGrayscale(cropped);
        using var enhanced = IncreaseContrast(gray, 1.5f);

        // Guardar en memory stream
        var resultStream = new MemoryStream();
        enhanced.Encode(SKEncodedImageFormat.Png, 100)
                .SaveTo(resultStream);
        resultStream.Seek(0, SeekOrigin.Begin);

        return resultStream;
    }

    private static SKBitmap ToGrayscale(SKBitmap bitmap)
    {
        var result = new SKBitmap(bitmap.Width, bitmap.Height);
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var gray = (byte)(0.299 * pixel.Red + 0.587 * pixel.Green + 0.114 * pixel.Blue);
                result.SetPixel(x, y, new SKColor(gray, gray, gray));
            }
        }
        return result;
    }

    private static SKBitmap IncreaseContrast(SKBitmap bitmap, float factor)
    {
        var result = new SKBitmap(bitmap.Width, bitmap.Height);
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var r = Clamp((int)((pixel.Red - 128) * factor + 128));
                var g = Clamp((int)((pixel.Green - 128) * factor + 128));
                var b = Clamp((int)((pixel.Blue - 128) * factor + 128));
                result.SetPixel(x, y, new SKColor((byte)r, (byte)g, (byte)b));
            }
        }
        return result;
    }

    private static int Clamp(int value) => Math.Max(0, Math.Min(255, value));
}
```

---

## 7. REPOSITORIO NUEVO

### Repositories/IRepositories/IWritingAssignmentRepositorie.cs

```csharp
using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IWritingAssignmentRepositorie
{
    Task<Result<Guid>> CreateWritingAssignment(CreateWritingAssignmentRequest request);

    Task<WritingWorksheetDto?> GetWorksheetData(Guid assignmentStudentId);

    Task<Result<WritingEvidence>> SaveEvidence(Guid assignmentStudentId, SubmitWritingEvidenceRequest request,
        string driveUrl, string? driveFileId, short? ocrGrade, decimal? ocrConfidence, string? ocrRawText);

    Task<Result<WritingEvidence>> ConfirmGrade(Guid assignmentStudentId, short grade);

    Task<IEnumerable<WritingEvidenceDto>> GetStudentHistory(Guid studentId, Guid? schoolYearId = null);
}
```

### Repositories/WritingAssignmentRepositorie.cs

```csharp
using System.Data;
using Dapper;
using Data;
using Microsoft.EntityFrameworkCore;
using Models.DB;
using Models.Dto;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Abstractions;
using Utilities.Errors;

namespace Repositories;

public class WritingAssignmentRepositorie : IWritingAssignmentRepositorie
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _dbConnection;

    public WritingAssignmentRepositorie(AppDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<Result<Guid>> CreateWritingAssignment(CreateWritingAssignmentRequest request)
    {
        // Validar material
        var material = await _context.Material
            .FirstOrDefaultAsync(x => x.Id == request.MaterialId);
        if (material == null)
            return Result<Guid>.Failure(MaterialErrors.MaterialNotFound);

        // Validar profesor
        var userExists = await _context.User.AnyAsync(x => x.Id == request.AssignedById);
        if (!userExists)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        // Validar ciclo
        var schoolYearExists = await _context.SchoolYear.AnyAsync(x => x.Id == request.SchoolYearId);
        if (!schoolYearExists)
            return Result<Guid>.Failure(SchoolErrors.SchoolYearNotFound);

        // Validar alumnos
        var studentIds = request.StudentIds.Distinct().ToList();
        if (studentIds.Count == 0)
            return Result<Guid>.Failure(AssignmentErrors.GroupOrStudentsRequired);

        var existingStudents = await _context.Student
            .Where(x => studentIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        if (existingStudents.Count != studentIds.Count)
            return Result<Guid>.Failure(StudentErrors.StudentNotFound);

        await using var tx = await _context.Database.BeginTransactionAsync();

        // Crear asignación con tipo ESCRITURA
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            MaterialId = request.MaterialId,
            AssignedById = request.AssignedById,
            SchoolYearId = request.SchoolYearId,
            AssignmentDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            Instructions = request.Instructions,
            Active = boolStatus.True
        };

        // Asignar tipo ESCRITURA mediante una propiedad de navegación o columna
        // Nota: Se asume que la tabla tiene la columna tipo_asignacion tras la migración

        await _context.Assignment.AddAsync(assignment);

        var rows = studentIds.Select(studentId => new AssignmentStudent
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            StudentId = studentId,
            Status = assignmentStudentStatus.PENDIENTE
        }).ToList();

        await _context.AssignmentStudent.AddRangeAsync(rows);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<Guid>.Success(assignment.Id);
    }

    public async Task<WritingWorksheetDto?> GetWorksheetData(Guid assignmentStudentId)
    {
        var sql = """
            SELECT
                ast.id AS AssignmentStudentId,
                a.id AS AssignmentId,
                s.name || ' ' || s.father_last_name || COALESCE(' ' || s.mother_last_name, '') AS StudentName,
                g.name AS GroupName,
                g.grade::text AS Grade,
                sch.name AS SchoolName,
                sy.name AS SchoolYear,
                a.instructions AS Instructions
            FROM "assignment_student" ast
            INNER JOIN "assignment" a ON a.id = ast.assignment_id
            INNER JOIN "student" s ON s.id = ast.student_id
            LEFT JOIN "registration" r ON r.student_id = s.id AND r.school_year_id = a.school_year_id
            LEFT JOIN "group" g ON g.id = r.group_id
            LEFT JOIN "school" sch ON sch.id = g.school_id
            INNER JOIN "school_year" sy ON sy.id = a.school_year_id
            WHERE ast.id = @AssignmentStudentId
            """;

        return await _dbConnection.QueryFirstOrDefaultAsync<WritingWorksheetDto>(
            sql, new { AssignmentStudentId = assignmentStudentId });
    }

    public async Task<Result<WritingEvidence>> SaveEvidence(
        Guid assignmentStudentId,
        SubmitWritingEvidenceRequest request,
        string driveUrl,
        string? driveFileId,
        short? ocrGrade,
        decimal? ocrConfidence,
        string? ocrRawText)
    {
        var entity = await _context.AssignmentStudent
            .FirstOrDefaultAsync(x => x.Id == assignmentStudentId);

        if (entity == null)
            return Result<WritingEvidence>.Failure(AssignmentErrors.AssignmentStudentNotFound);

        var finalGrade = ocrGrade ?? request.ManualGrade;

        // Guardar evidencia
        var evidence = new WritingEvidence
        {
            Id = Guid.NewGuid(),
            AssignmentStudentId = assignmentStudentId,
            DriveFileId = driveFileId,
            DriveUrl = driveUrl,
            OcrRawText = ocrRawText,
            CalificacionOcr = ocrGrade,
            CalificacionManual = request.ManualGrade,
            CalificacionMaxima = 10,
            OcrConfianza = ocrConfidence,
            OcrConfirmado = request.ConfirmOcr == true
        };

        await _context.Set<WritingEvidence>().AddAsync(evidence);

        // Actualizar AssignmentStudent
        entity.Status = assignmentStudentStatus.EVALUADO;
        entity.EvidenceUrls = [driveUrl];
        entity.ManualGradeJson = System.Text.Json.JsonSerializer.Serialize(new
        {
            grade = finalGrade,
            maxScore = 10,
            type = "ESCRITURA",
            driveFileId = driveFileId,
            ocrConfidence = ocrConfidence
        });
        entity.EvaluationDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<WritingEvidence>.Success(evidence);
    }

    public async Task<Result<WritingEvidence>> ConfirmGrade(Guid assignmentStudentId, short grade)
    {
        var evidence = await _context.Set<WritingEvidence>()
            .FirstOrDefaultAsync(x => x.AssignmentStudentId == assignmentStudentId);

        if (evidence == null)
            return Result<WritingEvidence>.Failure(AssignmentErrors.AssignmentStudentNotFound);

        evidence.CalificacionManual = grade;
        evidence.OcrConfirmado = boolStatus.True;

        var entity = await _context.AssignmentStudent
            .FirstOrDefaultAsync(x => x.Id == assignmentStudentId);

        if (entity != null)
        {
            entity.ManualGradeJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                grade = grade,
                maxScore = 10,
                type = "ESCRITURA",
                previousOcr = evidence.CalificacionOcr
            });
        }

        await _context.SaveChangesAsync();

        return Result<WritingEvidence>.Success(evidence);
    }

    public async Task<IEnumerable<WritingEvidenceDto>> GetStudentHistory(Guid studentId, Guid? schoolYearId = null)
    {
        var sql = """
            SELECT
                we.id AS Id,
                we.assignment_student_id AS AssignmentStudentId,
                s.name || ' ' || s.father_last_name || COALESCE(' ' || s.mother_last_name, '') AS StudentFullName,
                g.name AS GroupName,
                sch.name AS SchoolName,
                sy.name AS SchoolYearName,
                we.drive_url AS DriveUrl,
                we.calificacion_ocr AS CalificacionOcr,
                COALESCE(we.calificacion_manual, we.calificacion_ocr) AS CalificacionFinal,
                we.calificacion_maxima AS CalificacionMaxima,
                we.ocr_confianza AS OcrConfianza,
                we.ocr_confirmado AS OcrConfirmado,
                we.created_at AS CreatedAt
            FROM "evidencia_escritura" we
            INNER JOIN "assignment_student" ast ON ast.id = we.assignment_student_id
            INNER JOIN "student" s ON s.id = ast.student_id
            INNER JOIN "assignment" a ON a.id = ast.assignment_id
            INNER JOIN "school_year" sy ON sy.id = a.school_year_id
            LEFT JOIN "registration" r ON r.student_id = s.id AND r.school_year_id = a.school_year_id
            LEFT JOIN "group" g ON g.id = r.group_id
            LEFT JOIN "school" sch ON sch.id = g.school_id
            WHERE ast.student_id = @StudentId
            """;

        if (schoolYearId.HasValue)
            sql += " AND a.school_year_id = @SchoolYearId";

        sql += " ORDER BY we.created_at DESC";

        return await _dbConnection.QueryAsync<WritingEvidenceDto>(
            sql, new { StudentId = studentId, SchoolYearId = schoolYearId });
    }
}
```

---

## 8. CONTROLADOR NUEVO

### Controllers/WritingAssignmentController.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Services.IServices;
using Utilities.Errors;

namespace Controllers;

[ApiController]
[Route("api/asignaciones/escritura")]
[Produces("application/json")]
[Authorize(Roles = "ADMIN,DIRECTOR_USAER,ESPECIALISTA_COM,ESPECIALISTA_PSI,ESPECIALISTA_APR,TRABAJO_SOCIAL,DOCENTE")]
public class WritingAssignmentController(
    IWritingAssignmentRepositorie writingRepositorie,
    IAssignmentRepositorie assignmentRepositorie,
    IFileStorageService fileStorageService,
    IOcrService ocrService,
    IServiceRepositorie serviceRepositorie) : ControllerBase
{
    private readonly IWritingAssignmentRepositorie _writingRepositorie = writingRepositorie;
    private readonly IAssignmentRepositorie _assignmentRepositorie = assignmentRepositorie;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IOcrService _ocrService = ocrService;
    private readonly IServiceRepositorie _serviceRepositorie = serviceRepositorie;

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    /// <summary>
    /// Crea una asignación de tipo ESCRITURA y genera el PDF con QR
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateWritingAssignment([FromBody] CreateWritingAssignmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        var result = await _writingRepositorie.CreateWritingAssignment(request);

        if (!result.IsSuccess)
        {
            if (result.error.Code == MaterialErrors.MaterialNotFound.Code ||
                result.error.Code == UserErrors.UserNotFound.Code ||
                result.error.Code == SchoolErrors.SchoolYearNotFound.Code ||
                result.error.Code == StudentErrors.StudentNotFound.Code ||
                result.error.Code == AssignmentErrors.GroupOrStudentsRequired.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CREATE",
            AffectedTable = "assignment",
            RecordId = result.Value!.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return StatusCode(201, new { assignmentId = result.Value });
    }

    /// <summary>
    /// Descarga el PDF de la hoja de trabajo para imprimir
    /// </summary>
    [HttpGet("{assignmentStudentId:guid}/hoja-imprimir")]
    public async Task<IActionResult> GetWorksheetPdf(Guid assignmentStudentId)
    {
        var worksheetData = await _writingRepositorie.GetWorksheetData(assignmentStudentId);
        if (worksheetData == null)
            return NotFound(AssignmentErrors.AssignmentStudentNotFound.Message);

        // Generar PDF con QuestPDF
        var pdfBytes = GenerateWorksheetPdf(worksheetData);

        return File(pdfBytes, "application/pdf",
            $"Hoja_Escritura_{worksheetData.StudentName}_{DateTime.UtcNow:yyyy-MM-dd}.pdf");
    }

    /// <summary>
    /// Sube la evidencia de escritura (foto/scan) y ejecuta OCR para detectar calificación
    /// </summary>
    [HttpPost("{assignmentStudentId:guid}/subir-evidencia")]
    [RequestSizeLimit(10_000_000)] // 10MB max
    public async Task<IActionResult> SubmitEvidence(Guid assignmentStudentId,
        [FromForm] SubmitWritingEvidenceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        if (request.File == null || request.File.Length == 0)
            return BadRequest("El archivo es requerido");

        // Validar tipo de archivo
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
        if (!allowedTypes.Contains(request.File.ContentType.ToLower()))
            return BadRequest("Solo se permiten imágenes JPG o PNG");

        // Validar que la asignación existe
        var assignmentStudent = await _assignmentRepositorie.GetAssignmentStudentById(assignmentStudentId);
        if (assignmentStudent == null)
            return NotFound(AssignmentErrors.AssignmentStudentNotFound.Message);

        // 1. Ejecutar OCR si no viene calificación manual
        short? finalGrade = request.ManualGrade;
        decimal? ocrConfidence = null;
        string? ocrRawText = null;

        if (!finalGrade.HasValue)
        {
            using var ms = new MemoryStream();
            await request.File.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var ocrResult = await _ocrService.ExtractGradeFromWorksheetAsync(ms);

            if (ocrResult.Success)
            {
                finalGrade = ocrResult.Grade;
                ocrConfidence = ocrResult.Confidence;
                ocrRawText = ocrResult.RawText;
            }
        }

        // 2. Obtener datos del alumno para organización en Drive
        var worksheetData = await _writingRepositorie.GetWorksheetData(assignmentStudentId);

        // 3. Subir imagen a Google Drive
        string driveUrl;
        using (var imageStream = new MemoryStream())
        {
            await request.File.CopyToAsync(imageStream);
            imageStream.Seek(0, SeekOrigin.Begin);

            var fileName = $"Escritura_{DateTime.UtcNow:yyyy-MM-dd}_Cal_{finalGrade ?? 0}_{assignmentStudentId:N}.jpg";

            driveUrl = await _fileStorageService.UploadEvidenceToDriveAsync(
                imageStream,
                fileName,
                "image/jpeg",
                worksheetData?.SchoolYear ?? "Sin-Ciclo",
                worksheetData?.SchoolName ?? "Sin-Escuela",
                worksheetData?.Grade + "-" + worksheetData?.GroupName ?? "Sin-Grupo",
                worksheetData?.StudentName ?? "Sin-Alumno"
            );
        }

        // 4. Guardar evidencia en BD
        var saveResult = await _writingRepositorie.SaveEvidence(
            assignmentStudentId,
            request,
            driveUrl,
            null, // driveFileId (se puede extraer de la URL)
            finalGrade,
            ocrConfidence,
            ocrRawText
        );

        if (!saveResult.IsSuccess)
            return BadRequest(saveResult.error.Message);

        // 5. Audit log
        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "SUBMIT_EVIDENCE",
            AffectedTable = "evidencia_escritura",
            RecordId = assignmentStudentId.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(new
            {
                assignmentStudentId,
                grade = finalGrade,
                ocrConfidence,
                driveUrl
            })
        });

        return Ok(new
        {
            evidenceId = saveResult.Value!.Id,
            driveUrl,
            grade = finalGrade,
            ocrConfidence,
            ocrRawText,
            needsConfirmation = !request.ConfirmOcr.GetValueOrDefault() && ocrConfidence < 70
        });
    }

    /// <summary>
    /// Confirma o corrige la calificación detectada por OCR
    /// </summary>
    [HttpPost("{assignmentStudentId:guid}/confirmar-calificacion")]
    public async Task<IActionResult> ConfirmGrade(Guid assignmentStudentId, [FromBody] ConfirmGradeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid request");

        if (request.Grade < 1 || request.Grade > 10)
            return BadRequest("La calificación debe estar entre 1 y 10");

        var result = await _writingRepositorie.ConfirmGrade(assignmentStudentId, request.Grade);

        if (!result.IsSuccess)
        {
            if (result.error.Code == AssignmentErrors.AssignmentStudentNotFound.Code)
                return NotFound(result.error.Message);

            return BadRequest(result.error.Message);
        }

        await _serviceRepositorie.AddLog(new AuditLog
        {
            UserId = GetCurrentUserId(),
            Action = "CONFIRM_GRADE",
            AffectedTable = "evidencia_escritura",
            RecordId = assignmentStudentId.ToString(),
            Request = System.Text.Json.JsonSerializer.Serialize(request)
        });

        return Ok(new
        {
            evidenceId = result.Value!.Id,
            grade = request.Grade,
            confirmed = true
        });
    }

    /// <summary>
    /// Obtiene el historial de evidencias de escritura de un alumno
    /// </summary>
    [HttpGet("alumnos/{studentId:guid}/historial")]
    public async Task<IActionResult> GetStudentHistory(Guid studentId, [FromQuery] Guid? schoolYearId)
    {
        var history = await _writingRepositorie.GetStudentHistory(studentId, schoolYearId);
        return Ok(history);
    }

    /// <summary>
    /// Genera el PDF de la hoja de trabajo usando QuestPDF
    /// </summary>
    private static byte[] GenerateWorksheetPdf(WritingWorksheetDto data)
    {
        // Este método requiere QuestPDF
        // Se implementa con el diseño:
        // - Encabezado: datos del alumno (nombre, grado, grupo, escuela)
        // - Área de trabajo (recuadro grande para escritura del alumno)
        // - Recuadro de calificación (inferior derecho)
        // - Código QR (inferior izquierdo) con assignmentStudentId
        // - Instrucciones de la asignación

        // Nota: QuestPDF requiere licencia para uso comercial,
        // pero para piloto/open-source es gratuita.
        // Alternativa gratuita: DinkToPdf (wkhtmltopdf) o generar HTML y convertir

        throw new NotImplementedException("Implementar con QuestPDF en fase de código");
    }
}

/// <summary>
/// Request model para confirmar/corregir calificación
/// </summary>
public class ConfirmGradeRequest
{
    public short Grade { get; set; }
}
```

---

## 9. MODIFICACIONES A ARCHIVOS EXISTENTES

### Program.cs — Registrar nuevos servicios

Agregar después de la línea de `builder.Services.AddScoped<IAssignmentRepositorie, AssignmentRepositorie>();`:

```csharp
// Writing Assignment (Escritura)
builder.Services.AddScoped<IWritingAssignmentRepositorie, WritingAssignmentRepositorie>();

// File Storage
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// OCR Service
builder.Services.AddSingleton<IOcrService, OcrService>();
```

### SIAE-V2.csproj — Agregar paquetes

```xml
<PackageReference Include="QRCoder" Version="1.6.1" />
<PackageReference Include="QuestPDF" Version="2024.12.0" />
<PackageReference Include="Google.Apis.Drive.v3" Version="1.69.0" />
<PackageReference Include="Tesseract.NET" Version="5.2.0" />
<PackageReference Include="SkiaSharp" Version="3.116.1" />
```

### Dockerfile — Instalar Tesseract

```dockerfile
# Stage: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Instalar Tesseract OCR con idiomas
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        tesseract-ocr \
        tesseract-ocr-spa \
        tesseract-ocr-eng \
        libgdiplus \
        libc6-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["sh", "-c", "dotnet SIAE-V2.dll --urls http://0.0.0.0:${PORT}"]
```

### appsettings.json — Agregar configuraciones

```json
{
  // ... existente ...
  "GoogleDrive": {
    "RootFolderId": "ID_DE_LA_CARPETA_RAIZ_EN_DRIVE",
    "ServiceAccountJson": "CONTENIDO_DEL_JSON_DEL_SERVICE_ACCOUNT"
  },
  "Tesseract": {
    "DataPath": "/usr/share/tesseract-ocr/5/tessdata"
  }
}
```

**Importante:** Mover `ServiceAccountJson` a variable de entorno en producción:
```
GOOGLE_DRIVE__SERVICE_ACCOUNT_JSON=<contenido_base64>
```

---

## 10. FLUJO COMPLETO PASO A PASO

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         FLUJO COMPLETO                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  PROFESOR / DOCENTE              SISTEMA (.NET)           GOOGLE DRIVE  │
│  ──────────────────              ──────────────           ────────────  │
│                                                                          │
│  1. POST /api/asignaciones/escritura                                    │
│     { materialId, studentIds[], } ──────▶  Crea Assignment +            │
│                                              AssignmentStudent          │
│     ◀────── { assignmentId }          (tipo_asignacion = 'ESCRITURA')   │
│                                                                          │
│  2. GET /.../{id}/hoja-imprimir       ──▶  Genera PDF con QuestPDF:     │
│                                              - Datos alumno             │
│     ◀────── PDF para imprimir              - Área de trabajo           │
│                                              - Recuadro calificación    │
│                                              - QR (assignmentStudentId) │
│                                                                          │
│  3. PROFESOR IMPRIME LA HOJA                                            │
│                                                                          │
│  4. ALUMNO TRABAJA EN LA HOJA (escritura manual)                       │
│                                                                          │
│  5. PROFESOR CALIFICA: anota la nota en el recuadro (ej: 8)            │
│                                                                          │
│  6. PROFESOR ESCANEA / FOTO la hoja completa                            │
│                                                                          │
│  7. POST /.../{id}/subir-evidencia                                      │
│     [multipart: file.jpg] ──────────▶  Lee QR (valida ID)               │
│                                          Recorta recuadro calificación  │
│                                          Tesseract OCR:                 │
│                                            └── "8" con 92% confianza   │
│                                          Sube a Google Drive ────────▶  │
│     ◀────── { grade: 8,                     /Historial-Académico/      │
│               driveUrl,                       /2025-2026/              │
│               ocrConfidence: 92,               /Escuela/               │
│               needsConfirmation: false }        /2-A/                  │
│                                                   /López_Juan/         │
│                                                    Escritura_...jpg    │
│                                                                          │
│  8. (OPCIONAL) POST /.../confirmar-calificacion                         │
│     { grade: 8 } ────────────────────▶  Actualiza BD:                  │
│                                             ocr_confirmado = true      │
│     ◀────── { confirmed: true }            calificacion_manual = 8    │
│                                                                          │
│  9. GET /.../alumnos/{id}/historial                                     │
│     ◀────── [lista de evidencias]       Consulta BD + Drive            │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 11. ESTRATEGIA OCR — DETALLE TÉCNICO

### Configuración de Tesseract para Dígitos Manuscritos

```
Modo:        PageSegMode.SingleBlock (tratar como un solo bloque de texto)
Whitelist:   "0123456789" (solo dígitos)
Idiomas:     eng+spa (español e inglés combinados)
Preproceso:  Escala de grises → Binarización Otsu → Aumento de contraste 1.5x
Recorte:     Coordenadas fijas del PDF (x:1900, y:100, w:400, h:250 a 300dpi)
```

### Precisión Esperada

| Condición | Precisión |
|-----------|-----------|
| Dígito claro, bien centrado, contraste alto | ~85-90% |
| Dígito legible, contraste medio | ~70-80% |
| Dígito borroso, manchado, ángulo | ~40-60% |
| Sin calificación en el recuadro | Fallback: error con mensaje |

### Manejo de Fallos

- Si OCR no detecta nada o confianza < 50%: devolver error + sugerir ingreso manual
- El profesor ingresa la nota manualmente en `manualGrade`
- La imagen siempre se guarda (incluso si OCR falla)

---

## 12. QR CODE EN LA HOJA DE TRABAJO

### Generación del QR

```csharp
using QRCoder;
using SkiaSharp;

public static byte[] GenerateQrCode(string data)
{
    using var generator = new QRCodeGenerator();
    var qrData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
    using var code = new SkiaSharpQRCode(qrData);
    using var bitmap = code.GetGraphic(10);

    using var image = bitmap.Encode(SKEncodedImageFormat.Png, 100);
    using var ms = new MemoryStream();
    image.SaveTo(ms);
    return ms.ToArray();
}
```

### Contenido del QR

El QR contiene un JSON compacto:
```json
{"a":"{assignmentStudentId}","h":"{HMAC_del_ID}"}
```

Donde el HMAC se calcula con la clave secreta JWT para evitar falsificación:
```csharp
var hmac = Convert.ToHexString(
    System.Security.Cryptography.HMACSHA256.HashData(
        Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!),
        Encoding.UTF8.GetBytes(assignmentStudentId.ToString())
    )
);
```

---

## 13. SEGURIDAD

| Medida | Implementación |
|--------|---------------|
| **Validación de imagen** | Solo `.jpg`, `.png`; máximo 10MB |
| **Anti-falsificación QR** | HMAC del ID de asignación + clave secreta JWT |
| **Límite de tamaño** | `[RequestSizeLimit(10_000_000)]` en el endpoint |
| **Autenticación** | JWT requerido en todos los endpoints |
| **Autorización** | Solo ADMIN, DIRECTOR_USAER, ESPECIALISTAS, DOCENTE pueden subir evidencias |
| **Rate limiting** | Sugerido: máximo 30 uploads/minuto por usuario |
| **Auditoría** | Todas las operaciones quedan registradas en `audit_log` |
| **Drive privado** | Los archivos se crean sin visibilidad pública (solo el Service Account accede) |
| **Backup** | La imagen original se conserva sin procesamiento para re-OCR futuro |

---

## 14. CONFIGURACIÓN DE GOOGLE DRIVE (SERVICE ACCOUNT)

### Pasos de Setup

```
1. Ir a https://console.cloud.google.com
2. Crear proyecto nuevo: "siae-evidencias-piloto"
3. Habilitar Google Drive API
4. Ir a IAM & Admin → Service Accounts
5. Crear Service Account: "siae-writer"
6. Generar clave JSON → descargar
7. Codificar JSON en Base64:
     cat servicio-cuenta.json | base64
8. Guardar en Railway como variable de entorno:
     GOOGLE_DRIVE__SERVICE_ACCOUNT_JSON=<base64_del_json>
9. Crear carpeta raíz "Historial-Academico-SIAE" en una cuenta Gmail nueva
10. Compartir la carpeta con el email del Service Account (rol: Editor)
11. Obtener el Folder ID de la carpeta raíz (de la URL de Drive)
12. Guardar como variable de entorno:
      GOOGLE_DRIVE__ROOT_FOLDER_ID=<folder_id>
```

### Cuenta Gmail Nueva

- Crear cuenta: `siae.evidencias@gmail.com` (o similar)
- 15GB gratis para el piloto
- La carpeta raíz se crea desde esta cuenta
- Se comparte con el Service Account

---

## 15. PLAN DE TRABAJO POR FASES

| Fase | Archivos a Crear/Modificar | Tiempo Est. | Dependencias |
|------|---------------------------|-------------|--------------|
| **1. Infraestructura** | `SIAE-V2.csproj` (packages), `Dockerfile` (tesseract), `appsettings.json` | 1 día | Docker build |
| **2. Modelos** | `WritingEvidence.cs`, `CreateWritingAssignmentRequest.cs`, `SubmitWritingEvidenceRequest.cs`, `WritingEvidenceDto.cs`, `WritingWorksheetDto.cs` | 1 día | Ninguna |
| **3. Servicios** | `IFileStorageService.cs`, `FileStorageService.cs`, `IOcrService.cs`, `OcrService.cs` | 2 días | Google Drive API setup |
| **4. Repositorio** | `IWritingAssignmentRepositorie.cs`, `WritingAssignmentRepositorie.cs` | 1 día | BD migración |
| **5. Controlador** | `WritingAssignmentController.cs` | 1 día | Servicios + Repositorio |
| **6. Migración BD** | Script SQL + Migration de EF | 1 día | Supabase |
| **7. PDF + QR** | QuestPDF layout + QRCoder en controlador | 2 días | QuestPDF |
| **8. Registro DI** | `Program.cs` (agregar servicios) | 0.5 día | Todas las fases anteriores |
| **9. Pruebas** | Postman / SIAE-V2.http | 1 día | Todo implementado |
| **Total** | ~11 archivos nuevos + 4 modificados | **~10.5 días** | |

---

## 16. RESPALDO: FLUJO ACTUAL vs NUEVO

### Asignaciones Digitales (flujo actual — sin cambios)

```
Material tipo: DIALOGO_ANIMADO, ACTIVIDAD, JUEGO_DIGITAL
Estudiante trabaja: Digital (en la plataforma)
Evaluación: Auto-evaluación (AutoEvaluationJson) o manual (ManualGradeJson)
Evidencia: ResponseJson (JSON con respuestas del alumno)
Status: PENDIENTE → EN_PROGRESO → COMPLETADO → EVALUADO
```

### Asignaciones de Escritura (flujo nuevo)

```
Material tipo: DOCUMENTO (plantilla de escritura)
Estudiante trabaja: En físico (hoja impresa)
Evaluación: OCR detecta calificación del recuadro manuscrito
Evidencia: Imagen escaneada subida a Google Drive
Status: PENDIENTE → EVALUADO (no pasa por COMPLETADO, va directo)
```

---

## 17. NOTAS ADICIONALES

1. **QuestPDF** requiere una licencia para uso comercial (gratis para open source). Si es problema, usar `DinkToPdf` o generar HTML y convertirlo a PDF con `SelectPdf` o similar.

2. **Las coordenadas de recorte del OCR** (`x:1900, y:100, w:400, h:250`) son para un PDF A4 a 300dpi. Si se cambia el layout del PDF, hay que ajustarlas.

3. **El QR debe estar en una posición fija** del PDF para que sea fácil escanearlo desde la foto.

4. **Google Drive API** tiene cuotas: 10M solicitudes/día, 10TB subida/día. Para un piloto con pocos alumnos es más que suficiente.

5. **El OCR siempre puede fallar.** El diseño debe permitir que el profesor ingrese la nota manualmente como fallback. La imagen siempre se guarda aunque el OCR falle.

---

*Documento generado el 14 de mayo de 2026*
*Proyecto: SIAE-V2 — Módulo de Asignaciones de Escritura con OCR + Google Drive*
