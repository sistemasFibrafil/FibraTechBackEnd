using Net.Data;
using Net.CrossCotting;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Net.Business.Logic.Interfaces.Common;
using Net.Business.Logic.Interfaces.SAPBusinessOne.Sales;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Close;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Create;
using Net.Business.DTO.SAPBusinessOne.Sales.Orders.Update;
using Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Create;
using Net.Business.DTO.SAPBusinessOne.Common.Attachments2.Update;
using Net.Business.Logic.Mappers.SAPBusinessOne.Sales.Orders.Close;
using Net.Business.Logic.Mappers.SAPBusinessOne.Sales.Orders.Create;
using Net.Business.Logic.Mappers.SAPBusinessOne.Sales.Orders.Update;
namespace Net.Business.Logic.Services.SAPBusinessOne.Sales
{
    public class OrdersService
        (
            IWebHostEnvironment env,
            IFileService fileService,
            IRepositoryWrapper repository,
            IValidator<OrdersCloseRequestDto> validatorClose,
            IValidator<OrdersCreateRequestDto> validatorCreate,
            IValidator<OrdersUpdateRequestDto> validatorUpdate
        ) : IOrdersService
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IFileService _fileService = fileService;
        private readonly IRepositoryWrapper _repository = repository;
        private readonly IValidator<OrdersCloseRequestDto> _validatorClose = validatorClose;
        private readonly IValidator<OrdersCreateRequestDto> _validatorCreate = validatorCreate;
        private readonly IValidator<OrdersUpdateRequestDto> _validatorUpdate = validatorUpdate;

        public async Task<ResultadoTransaccionResponse<object>> SetCreate(OrdersCreateRequestDto dto, IList<IFormFile> files)
        {
            var tempFiles = new List<(string tempPath, string finalPath)>();

            var tempRoot = Path.Combine(_env.ContentRootPath, "temp");
            Directory.CreateDirectory(tempRoot);

            var requestFolder = _fileService.CreateTempFolder(tempRoot);

            try
            {
                dto.Attachments2 ??= new Attachments2CreateRequestDto();

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

                        var fileExt = line.FileExt.Trim().TrimStart('.');
                        var originalFileName = line.FileName.Trim();
                        var originalExpectedName = $"{originalFileName}.{fileExt}";

                        if (!fileDict.TryGetValue(originalExpectedName, out var file))
                        {
                            return ResponseHelper.Error<object>(
                                $"Archivo no encontrado en la petición: {originalExpectedName}");
                        }

                        var finalPathOriginal = Path.Combine(line.TrgtPath, originalExpectedName);

                        string finalFileName = originalFileName;
                        string finalExpectedName = originalExpectedName;

                        // Si ya existe en la ruta final, generar nuevo nombre
                        if (File.Exists(finalPathOriginal))
                        {
                            var suffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            finalFileName = $"{originalFileName}_{suffix}";
                            finalExpectedName = $"{finalFileName}.{fileExt}";
                        }

                        var tempPath = await _fileService.SaveFileAsync(
                            file,
                            requestFolder,
                            finalExpectedName
                        );

                        if (!File.Exists(tempPath))
                        {
                            return ResponseHelper.Error<object>(
                                $"No se pudo preparar archivo en TEMP: {finalExpectedName}");
                        }

                        // Actualizar datos que irán a SAP
                        line.FileName = finalFileName;
                        line.FileExt = fileExt;
                        line.SrcPath = requestFolder;

                        var finalPath = Path.Combine(line.TrgtPath, finalExpectedName);

                        tempFiles.Add((tempPath, finalPath));
                    }
                }

                // =========================================================
                // 🔹 VALIDACIÓN DTO
                // =========================================================
                var validation = await _validatorCreate.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }

                // =========================================================
                // 🔹 SAP
                // =========================================================
                var entity = OrdersCreateMapper.ToEntity(dto);
                var result = await _repository.Orders.SetCreate(entity);

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

        public async Task<ResultadoTransaccionResponse<object>> SetUpdate(OrdersUpdateRequestDto dto, IList<IFormFile> files)
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
                var entity = OrdersUpdateMapper.ToEntity(dto);
                var result = await _repository.Orders.SetUpdate(entity);

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

        public async Task<ResultadoTransaccionResponse<object>> SetClose(OrdersCloseRequestDto dto)
        {
            try
            {
                // 🔹 VALIDACIÓN
                var validation = await _validatorClose.ValidateAsync(dto);

                if (!validation.IsValid)
                {
                    return ResponseHelper.Error<object>(
                        string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
                    );
                }


                // 🔹 REPOSITORY (SAP)
                var entity = OrdersCloseMapper.ToEntity(dto);
                var result = await _repository.Orders.SetClose(entity);

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
    }
}
