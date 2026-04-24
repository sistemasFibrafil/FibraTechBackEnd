using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Net.BusinessLogic.Interfaces.Common;
using Net.Business.DTO.SAPBusinessOne.Drafts.Create;
using Net.Business.DTO.SAPBusinessOne.Drafts.Update;
using Net.BusinessLogic.Interfaces.SAPBusinessOne.Draft;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Draft.Create;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Draft.Update;
using Net.Business.DTO.SAPBusinessOne.Drafts.CreateToDocument;
using Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Create;
using Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Update;
using Net.BusinessLogic.Mappers.SAPBusinessOne.Drafts.CreateToDocument;
namespace Net.BusinessLogic.Services.SAPBusinessOne.Draft
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

        public async Task<ResultadoTransaccionResponse<object>> SetSaveDraftToDocument(DraftsCreateToDocumentRequestDto dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorCreateToDocument.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 REPOSITORY (SAP)
                var entity = DraftsCreateToDocumentMapper.ToEntity(dto);
                var result = await _repository.Drafts.SetSaveDraftToDocument(entity);

                if (result.ResultadoCodigo == -1)
                {
                    return ResponseHelper.From(result);
                }

                return ResponseHelper.Success<object>("OK");
            }
            catch (Exception ex)
            {
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
                bool existeRecord3o1 =
                    dto.Attachments2.Lines?.Any(x => x.Record == 3 || x.Record == 1) == true;

                // ============================================================
                // 🔹 PROCESAR ARCHIVOS
                // ============================================================
                if (dto.Attachments2.Lines != null)
                {
                    foreach (var line in dto.Attachments2.Lines)
                    {
                        var fileName = $"{line.FileName}.{line.FileExt}";
                        var finalPath = Path.Combine(line.TrgtPath ?? "", fileName);

                        // ====================================================
                        // 🔹 SOLO SI EXISTE RECORD = 1 O RECORD = 3
                        // ====================================================
                        if (!existeRecord3o1)
                            continue;

                        // ====================================================
                        // 🔥 RECORD = 2
                        // Copiar archivo existente desde TrgtPath a TEMP
                        // ====================================================
                        if (line.Record == 2)
                        {
                            if (File.Exists(finalPath))
                            {
                                var destinoTemp = Path.Combine(requestFolder, fileName);

                                File.Copy(finalPath, destinoTemp, true);

                                // 🔥 Actualizar SrcPath
                                line.SrcPath = requestFolder;
                            }

                            continue;
                        }

                        // ====================================================
                        // 🔥 RECORD = 1
                        // Buscar archivo en files por nombre/extensión
                        // ====================================================
                        if (line.Record == 1)
                        {
                            if (files == null || files.Count == 0)
                                continue;

                            var f = files.FirstOrDefault(x => string.Equals(Path.GetFileName(x.FileName), fileName, StringComparison.OrdinalIgnoreCase));

                            if (f == null)
                                continue;

                            // 🔥 Actualizar SrcPath
                            line.SrcPath = requestFolder;

                            // 🔹 Validar ruta
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

                            // 🔹 Guardar en TEMP
                            var tempPath = await _fileService.SaveFileAsync(
                                f,
                                requestFolder,
                                fileName
                            );

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
                // 🔥 ELIMINAR ARCHIVOS FÍSICOS (RECORD = 3)
                // ============================================================
                if (dto.Attachments2.Lines != null)
                {
                    foreach (var line in dto.Attachments2.Lines.Where(x => x.Record == 3))
                    {
                        var deleteName = $"{line.FileName}.{line.FileExt}";
                        var deletePath = Path.Combine(line.TrgtPath ?? "", deleteName);

                        try
                        {
                            if (File.Exists(deletePath))
                                File.Delete(deletePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error eliminando archivo: {deletePath}. Error: {ex.Message}");
                        }
                    }
                }

                // ============================================================
                // 🔹 MOVER ARCHIVOS NUEVOS (RECORD = 1)
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
