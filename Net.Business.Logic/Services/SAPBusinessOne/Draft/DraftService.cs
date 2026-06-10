using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Net.Business.Logic.Interfaces.Common;
using Net.Business.DTO.SAPBusinessOne.Drafts.Create;
using Net.Business.DTO.SAPBusinessOne.Drafts.Update;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Draft;
using Net.Business.Logic.Mappers.SAPBusinessOne.Draft.Create;
using Net.Business.Logic.Mappers.SAPBusinessOne.Draft.Update;
using Net.Business.DTO.SAPBusinessOne.Drafts.CreateToDocument;
using Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Create;
using Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Update;
using Net.Business.Logic.Mappers.SAPBusinessOne.Drafts.CreateToDocument;
using Net.Business.DTO.SAPBusinessOne.Common.Attachments2.CreateToDocument;
namespace Net.Business.Logic.Services.SAPBusinessOne.Draft
{
    public class DraftService
        (
            IWebHostEnvironment env,
            IFileService fileService,
            IRepositoryWrapper repository,
            IValidator<DraftsCreateRequestDto> validatorCreate,
            IValidator<DraftsUpdateRequestDto> validatorUpdate,
            IValidator<DraftsCreateToDocumentRequestDto> validatorCreateToDocument
        ) : IDraftService
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IFileService _fileService = fileService;
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<DraftsCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<DraftsUpdateRequestDto> _validatorUpdate = validatorUpdate;
        private readonly IValidator<DraftsCreateToDocumentRequestDto> _validatorCreateToDocument = validatorCreateToDocument;


        public async Task<ResultadoTransaccionResponse<object>> SetCreate(DraftsCreateRequestDto dto, IList<IFormFile> files)
        {
            var tempFiles = new List<(string tempPath, string finalPath)>();

            var tempRoot = Path.Combine(_env.ContentRootPath, "temp");
            Directory.CreateDirectory(tempRoot);

            var requestFolder = _fileService.CreateTempFolder(tempRoot);

            try
            {
                // 🔹 Inicializar Attachments
                dto.Attachments2 ??= new Attachments2CreateRequestDto();

                // 🔹 SIEMPRE setear SourcePath para SAP
                if (dto.Attachments2.Lines != null)
                {
                    foreach (var line in dto.Attachments2.Lines)
                    {
                        line.SrcPath = requestFolder;
                    }
                }

                // 🔹 GUARDAR ARCHIVOS EN TEMP
                if (files != null && files.Any() && dto.Attachments2.Lines != null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (i >= dto.Attachments2.Lines.Count)
                            break;

                        var f = files[i];
                        var line = dto.Attachments2.Lines[i];

                        var expectedName = $"{line.FileName}.{line.FileExt}";
                        var incomingName = Path.GetFileName(f.FileName);

                        // 🔹 Validar nombre
                        if (!string.Equals(incomingName, expectedName, StringComparison.OrdinalIgnoreCase))
                            continue;

                        // 🔹 Validar ruta destino
                        if (string.IsNullOrWhiteSpace(line.TrgtPath) || !Directory.Exists(line.TrgtPath))
                        {
                            return ResponseHelper.Error<object>($"Ruta no existe: {line.TrgtPath}");
                        }

                        // 🔐 Validar permisos
                        try
                        {
                            var testFile = Path.Combine(line.TrgtPath, $"test_{Guid.NewGuid()}.tmp");
                            File.WriteAllText(testFile, "test");
                            File.Delete(testFile);
                        }
                        catch
                        {
                            return ResponseHelper.Error<object>($"Sin permisos en: {line.TrgtPath}");
                        }

                        // 🔹 Guardar archivo en TEMP
                        var tempPath = await _fileService.SaveFileAsync(f, requestFolder, expectedName);

                        // 🔥 Validar existencia real (evita error SAP)
                        var fullTempPath = Path.Combine(requestFolder, expectedName);
                        if (!File.Exists(fullTempPath))
                        {
                            return ResponseHelper.Error<object>($"No se pudo guardar el archivo en TEMP: {expectedName}");
                        }

                        var finalPath = Path.Combine(line.TrgtPath, expectedName);

                        tempFiles.Add((tempPath, finalPath));
                    }
                }

                // 🔹 VALIDACIÓN DTO
                var validation = await _validatorCreate.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }

                // 🔹 REPOSITORY (SAP)
                var entity = DraftsCreateMapper.ToEntity(dto);
                var result = await _repository.Drafts.SetCreate(entity);

                if (result.ResultadoCodigo == -1)
                {
                    _fileService.DeleteDirectory(requestFolder);
                    return ResponseHelper.From(result);
                }

                // 🔹 MOVER ARCHIVOS A DESTINO FINAL
                foreach (var (tempPath, finalPath) in tempFiles)
                {
                    _fileService.MoveFile(tempPath, finalPath);
                }

                // 🔹 LIMPIAR TEMP
                _fileService.DeleteDirectory(requestFolder);

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                _fileService.DeleteDirectory(requestFolder);
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetSaveDraftToDocument(DraftsCreateToDocumentRequestDto dto, IList<IFormFile> files)
        {
            var tempFiles = new List<(string tempPath, string finalPath)>();

            var tempRoot = Path.Combine(_env.ContentRootPath, "temp");
            Directory.CreateDirectory(tempRoot);

            var requestFolder = _fileService.CreateTempFolder(tempRoot);

            try
            {
                dto.Attachments2 ??= new Attachments2CreateToDocumentRequestDto();

                var hasLines = dto.Attachments2.Lines != null && dto.Attachments2.Lines.Any();

                // 🔹 Indexar files (pueden ser 0, 1 o varios)
                var fileDict = files?
                    .ToDictionary(f => Path.GetFileName(f.FileName), StringComparer.OrdinalIgnoreCase)
                    ?? new Dictionary<string, IFormFile>();

                // =========================================================
                // 🔥 PROCESAMIENTO POR CADA LINE (MEZCLA REAL)
                // =========================================================
                if (hasLines)
                {
                    foreach (var line in dto.Attachments2.Lines)
                    {
                        // 🔴 Validaciones básicas
                        if (string.IsNullOrWhiteSpace(line.FileName) ||
                            string.IsNullOrWhiteSpace(line.FileExt))
                        {
                            return ResponseHelper.Error<object>(
                                $"Nombre/extensión inválido en Record: {line.Record}");
                        }

                        if (string.IsNullOrWhiteSpace(line.TrgtPath) ||
                            !Directory.Exists(line.TrgtPath))
                        {
                            return ResponseHelper.Error<object>(
                                $"Ruta no existe: {line.TrgtPath}");
                        }

                        var expectedName = $"{line.FileName}.{line.FileExt}";
                        var sourcePath = Path.Combine(line.TrgtPath, expectedName);

                        string tempPath;

                        // =====================================================
                        // 🟢 1. SI EXISTE EN FILES → USAR REQUEST
                        // =====================================================
                        if (fileDict.TryGetValue(expectedName, out var file))
                        {
                            tempPath = await _fileService
                                .SaveFileAsync(file, requestFolder, expectedName);
                        }
                        // =====================================================
                        // 🟡 2. SI NO → BUSCAR EN DISCO
                        // =====================================================
                        else if (File.Exists(sourcePath))
                        {
                            var destinationPath = Path.Combine(requestFolder, expectedName);

                            File.Copy(sourcePath, destinationPath, overwrite: true);

                            tempPath = destinationPath;
                        }
                        // =====================================================
                        // 🔴 3. NO EXISTE EN NINGÚN LADO
                        // =====================================================
                        else
                        {
                            return ResponseHelper.Error<object>(
                                $"Archivo no encontrado: {expectedName}");
                        }

                        // 🔥 Validación clave (SAP)
                        var fullTempPath = Path.Combine(requestFolder, expectedName);
                        if (!File.Exists(fullTempPath))
                        {
                            return ResponseHelper.Error<object>(
                                $"No se pudo preparar archivo en TEMP: {expectedName}");
                        }

                        // 🔹 Setear SrcPath
                        line.SrcPath = requestFolder;

                        var finalPath = Path.Combine(line.TrgtPath, expectedName);

                        tempFiles.Add((tempPath, finalPath));
                    }
                }

                // =========================================================
                // 🔹 VALIDACIÓN DTO
                // =========================================================
                var validation = await _validatorCreateToDocument.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }

                // =========================================================
                // 🔹 SAP
                // =========================================================
                var entity = DraftsCreateToDocumentMapper.ToEntity(dto);
                var result = await _repository.Drafts.SetSaveDraftToDocument(entity);

                if (result.ResultadoCodigo == -1)
                {
                    _fileService.DeleteDirectory(requestFolder);
                    return ResponseHelper.From(result);
                }

                // =========================================================
                // 🔹 MOVER A DESTINO FINAL
                // =========================================================
                foreach (var (tempPath, finalPath) in tempFiles)
                {
                    _fileService.MoveFile(tempPath, finalPath);
                }

                _fileService.DeleteDirectory(requestFolder);

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                _fileService.DeleteDirectory(requestFolder);
                return ResponseHelper.Error<object>(ex.Message);
            }
        }

        public async Task<ResultadoTransaccionResponse<object>> SetUpdate(DraftsUpdateRequestDto dto, IList<IFormFile> files)
        {
            var tempFiles = new List<(string tempPath, string finalPath)>();

            var tempRoot = Path.Combine(_env.ContentRootPath, "temp");
            Directory.CreateDirectory(tempRoot);

            var requestFolder = _fileService.CreateTempFolder(tempRoot);

            try
            {
                // 🔹 Inicializar Attachments
                dto.Attachments2 ??= new Attachments2UpdateRequestDto();

                // ============================================================
                // 🔹 VALIDAR SI EXISTE ALGÚN RECORD = 3 O RECORD = 1
                // ============================================================
                bool existeRecord3o1 = dto?.Attachments2?.Lines?.Any(x => x.Record == 3 || x.Record == 1) == true;

                // ============================================================
                // 🔹 PROCESAR ARCHIVOS (SOLO FRONTEND)
                // ============================================================
                if (dto?.Attachments2?.Lines != null)
                {
                    foreach (var line in dto.Attachments2.Lines)
                    {
                        var fileName = $"{line.FileName}.{line.FileExt}";
                        var finalPath = Path.Combine(line.TrgtPath ?? "", fileName);

                        if (!existeRecord3o1)
                            continue;

                        // 🔥 IGNORAR PLACEHOLDER (backend)
                        if (line.Record == 1 && string.Equals(line.FileName, "attachment-placeholder", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // 🔥 RECORD = 2
                        if (line.Record == 2)
                        {
                            if (File.Exists(finalPath))
                            {
                                var destinoTemp = Path.Combine(requestFolder, fileName);

                                File.Copy(finalPath, destinoTemp, true);

                                line.SrcPath = requestFolder;
                            }

                            continue;
                        }

                        // 🔥 RECORD = 1 (solo archivos reales)
                        if (line.Record == 1)
                        {
                            if (files == null || files.Count == 0)
                                continue;

                            var f = files.FirstOrDefault(x => string.Equals(Path.GetFileName(x.FileName), fileName, StringComparison.OrdinalIgnoreCase));

                            if (f == null)
                                continue;

                            line.SrcPath = requestFolder;

                            if (string.IsNullOrWhiteSpace(line.TrgtPath) || !Directory.Exists(line.TrgtPath))
                            {
                                return ResponseHelper.Error<object>($"Ruta no existe: {line.TrgtPath}");
                            }

                            try
                            {
                                var testFile = Path.Combine(line.TrgtPath, $"test_{Guid.NewGuid()}.tmp");

                                File.WriteAllText(testFile, "test");
                                File.Delete(testFile);
                            }
                            catch
                            {
                                return ResponseHelper.Error<object>($"Sin permisos en: {line.TrgtPath}");
                            }

                            var tempPath = await _fileService.SaveFileAsync(f, requestFolder, fileName);

                            var fullTempPath = Path.Combine(requestFolder, fileName);

                            if (!File.Exists(fullTempPath))
                            {
                                return ResponseHelper.Error<object>($"No se pudo guardar el archivo en TEMP: {fileName}");
                            }

                            tempFiles.Add((tempPath, finalPath));
                        }
                    }
                }

                // ============================================================
                // 🔥 CASO ESPECIAL: TODOS SON DELETE (Record = 3)
                // ============================================================
                if (dto?.Attachments2?.Lines != null && dto.Attachments2.Lines.Count > 0 && dto.Attachments2.Lines.All(x => x.Record == 3))
                {
                    var deletedLine = dto.Attachments2.Lines[0];

                    if (string.IsNullOrWhiteSpace(deletedLine.TrgtPath) || !Directory.Exists(deletedLine.TrgtPath))
                    {
                        return ResponseHelper.Error<object>($"Ruta no existe: {deletedLine.TrgtPath}");
                    }

                    // 🔐 Validar permisos
                    try
                    {
                        var testFile = Path.Combine(deletedLine.TrgtPath, $"test_{Guid.NewGuid()}.tmp");
                        File.WriteAllText(testFile, "test");
                        File.Delete(testFile);
                    }
                    catch
                    {
                        return ResponseHelper.Error<object>($"Sin permisos en: {deletedLine.TrgtPath}");
                    }

                    var placeholderRelativePath = Path.Combine("Resources", "placeholders", "attachment-placeholder.txt");
                    var placeholderFullPath = Path.Combine(_env.ContentRootPath, placeholderRelativePath);

                    if (!File.Exists(placeholderFullPath))
                    {
                        return ResponseHelper.Error<object>("No existe el archivo placeholder.");
                    }

                    var placeholderName = Path.GetFileName(placeholderFullPath);
                    var placeholderFileName = Path.GetFileNameWithoutExtension(placeholderFullPath);
                    var placeholderExt = Path.GetExtension(placeholderFullPath).TrimStart('.');

                    var tempPlaceholderPath = Path.Combine(requestFolder, placeholderName);
                    File.Copy(placeholderFullPath, tempPlaceholderPath, true);

                    dto.Attachments2.Lines.Add(new Attachments2LinesUpdateRequestDto
                    {
                        SrcPath = requestFolder,
                        TrgtPath = deletedLine.TrgtPath,
                        FileName = "attachment-placeholder",
                        FileExt = placeholderExt,
                        Date = DateTime.Now,
                        Record = 1
                    });

                    var finalPath = Path.Combine(deletedLine.TrgtPath ?? "", placeholderName);
                    tempFiles.Add((tempPlaceholderPath, finalPath));
                }

                // ============================================================
                // 🔹 VALIDACIÓN DTO
                // ============================================================
                var validation = await _validatorUpdate.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));
                }

                // ============================================================
                // 🔹 REPOSITORY (SAP)
                // ============================================================
                var entity = DraftsUpdateMapper.ToEntity(dto);
                var result = await _repository.Drafts.SetUpdate(entity);

                if (result.ResultadoCodigo == -1)
                {
                    _fileService.DeleteDirectory(requestFolder);
                    return ResponseHelper.From(result);
                }


                // ============================================================
                // 🔹 MOVER ARCHIVOS (incluye placeholder)
                // ============================================================
                foreach (var (tempPath, finalPath) in tempFiles)
                {
                    _fileService.MoveFile(tempPath, finalPath);
                }

                // ============================================================
                // 🔹 LIMPIAR TEMP
                // ============================================================
                _fileService.DeleteDirectory(requestFolder);

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
                _fileService.DeleteDirectory(requestFolder);
                return ResponseHelper.Error<object>(ex.Message);
            }
        }
    }
}
