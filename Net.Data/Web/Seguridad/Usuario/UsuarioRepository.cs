using System;
using System.IO;
using AutoMapper;
using System.Linq;
using System.Data;
using System.Text;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Security.Claims;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Net.Connection.ConnectionSAPBusinessOne;
namespace Net.Data.Web
{
    public class UsuarioRepository : RepositoryBase<UsuarioEntity>, IUsuarioRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IMapper _mapper;
        private readonly DataContextSeguridad _dbSeg;
        private readonly DataContextSAPBusinessOne _dbSap;

        private readonly ParametrosTokenConfig _tokenConfig;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public UsuarioRepository(IConnectionSQL context, IOptions<ParametrosTokenConfig> tokenConfig, DataContextSAPBusinessOne dbSap, DataContextSeguridad dbSeg, CompanyProviderSAPBusinessOne companyProviderSap, IMapper mapper)
            : base(context)
        {
            _dbSap = dbSap;
            _dbSeg = dbSeg;
            _mapper = mapper;
            _tokenConfig = tokenConfig.Value;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }

        public async Task<ResultadoTransaccionResponse<UsuarioQueryEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var list = await _dbSeg.Usuario
                .Where(u => u.Eliminado == false)
                .Select(u => new UsuarioQueryEntity
                {
                    IdUsuario = u.IdUsuario,
                    NombreCompleto = u.Nombre + " " + u.ApellidoPaterno
                }).ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<UsuarioQueryEntity>> GetListByFilter(UsuarioFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var query = _dbSeg.Usuario
                .AsNoTracking()
                .Include(u => u.Perfil)
                .Where(u => u.Eliminado == false);

                // FILTRO
                if (!string.IsNullOrWhiteSpace(value.Filter))
                {
                    var filter = value.Filter.Trim();

                    query = query.Where(x =>
                        EF.Functions.Like(EF.Functions.Collate(x.Usuario!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.Nombre!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.ApellidoPaterno!, GlobalVariables.CI), $"%{filter}%") ||
                        EF.Functions.Like(EF.Functions.Collate(x.ApellidoMaterno!, GlobalVariables.CI), $"%{filter}%")
                    );
                }
                

                // PROYECCIÓN FINAL
                var list = await query
                .Select(u => new UsuarioQueryEntity
                {
                    IdUsuario = u.IdUsuario,
                    Usuario = u.Usuario,
                    NombreCompleto = u.ApellidoPaterno + " " + u.ApellidoMaterno + " " + u.Nombre,
                    NroDocumento = u.NroDocumento,
                    DescripcionPerfil = u.Perfil.DescripcionPerfil,
                    Activo = u.Activo
                })
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = $"Registros Totales {list.Count}";
                resultTransaccion.DataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<UsuarioEntity>> GetById(UsuarioEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _dbSeg.Usuario
                .Where(u => u.Eliminado == false && u.IdUsuario == value.IdUsuario)
                .Select(u => new UsuarioEntity
                {
                    IdUsuario = u.IdUsuario,
                    IdPerfil = u.IdPerfil,
                    IdUserSap = u.IdUserSap,
                    SlpCode = u.SlpCode,
                    Nombre = u.Nombre,
                    ApellidoPaterno = u.ApellidoPaterno,
                    ApellidoMaterno = u.ApellidoMaterno,
                    NroDocumento = u.NroDocumento,
                    NroTelefono = u.NroTelefono,
                    Usuario = u.Usuario,
                    Clave = u.Clave,
                    Email = u.Email,
                    Imagen = u.Imagen,
                    ThemeDark = u.ThemeDark,
                    ThemeColor = u.ThemeColor,
                    TypeMenu = u.TypeMenu,
                    Activo = u.Activo
                }).FirstOrDefaultAsync();


                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.Data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        private async Task<UsuarioEntity> VerificarLogin(UsuarioEntity value)
        {
            var dat = await _dbSeg.Usuario
            .Where(u => u.Eliminado == false && u.Usuario == value.Usuario)
            .Select(u => new UsuarioEntity
            {
                IdUsuario = u.IdUsuario,
                IdPerfil = u.IdPerfil,
                IdUserSap = u.IdUserSap,
                Nombre = u.Nombre + " " + u.ApellidoPaterno,
                Usuario = u.Usuario,
                Clave = u.Clave,
                Email = u.Email,
                Imagen = u.Imagen
            }).FirstOrDefaultAsync();

            return dat;
        }

        public async Task<ResultadoTransaccionResponse<UsuarioAutenticarEntity>> Autenticar(UsuarioAutenticarEntity entidad)
        {
            var claveDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Clave);
            var usuarioDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Usuario);

            // Obtenemos los datos del usuario
            UsuarioEntity user = await VerificarLogin(new UsuarioEntity { Usuario = usuarioDesEncriptada.ToUpper() });

            ResultadoTransaccionResponse<UsuarioAutenticarEntity> resultadoTransaccion = new ResultadoTransaccionResponse<UsuarioAutenticarEntity>();

            ParametroSistemaRepository parametroSistema = new ParametroSistemaRepository(context);
            ParametroSistemaEntity _ParametroSistema = await parametroSistema.GetById(new ParametroSistemaEntity { IdParametrosSistema = 1 });

            if (_ParametroSistema.TipoAutenticacion.Equals("AUTO-NORMAL"))
            {
                resultadoTransaccion = new ResultadoTransaccionResponse<UsuarioAutenticarEntity>();

                if (user.Clave != entidad.Clave)
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Usuario y/o Contraseña incorrecto.";
                    return resultadoTransaccion;
                }

                if (user == null)
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Usuario y/o Contraseña incorrecto.";
                    return resultadoTransaccion;
                }
            }
            else
            {
                //NF
                //resultadoTransaccion = await AutenticarUsuarioDirectorioActivo(usuarioDesEncriptada, claveDesEncriptada);
                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    return resultadoTransaccion;
                }

                if (user == null)
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Usuario existe en DA. pero no se encuentra registrado en el Portal (Coordinar con el Area de TI)";
                    return resultadoTransaccion;
                }
            }

            //SEMILLA
            string semilla = _tokenConfig.Semilla;

            var claims = new[]
            {
                new Claim("usuario", entidad.Usuario)
            };

            //firma - header
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(semilla));
            var signCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //generador de JWT
            var token = new JwtSecurityToken(
                issuer: _tokenConfig.Emisor,
                audience: _tokenConfig.Destinatario,
                claims: claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: signCredentials
            );

            string tokenGenerado = new JwtSecurityTokenHandler().WriteToken(token);

            // Validamos si esta habilitado doble autenticación
            if ((bool)_ParametroSistema.FlgDobleAutenticacion)
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.IdRegistro = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Usuario no cuenta con correo registrado";
                    return resultadoTransaccion;
                }

                await GenerarToken(user.IdUsuario, user.Email);
            }

            UsuarioAutenticarEntity UsuarioAutenticar = new UsuarioAutenticarEntity
            {
                Usuario = user.Clave.ToUpper(),
                Token = tokenGenerado,
                FlgDobleAutenticacion = (bool)_ParametroSistema.FlgDobleAutenticacion,
                Email = user.Email
            };


            // Conexión a SAP
            var company = _companyProviderSap.GetCompany();

            resultadoTransaccion.ResultadoCodigo = 0;
            resultadoTransaccion.ResultadoDescripcion = "Se autentico correctamente";
            resultadoTransaccion.Data = UsuarioAutenticar;
            return resultadoTransaccion;
        }

        public async Task<ResultadoTransaccionResponse<UsuarioEntity>> SetCreate(UsuarioCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var entity = _mapper.Map<UsuarioEntity>(value);

                entity.Eliminado = false;

                await _dbSeg.Usuario.AddAsync(entity);
                await _dbSeg.SaveChangesAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se realizó con éxito...!!!";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        
        public async Task<ResultadoTransaccionResponse<UsuarioEntity>> SetUpdate(UsuarioUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var entity = await _dbSeg.Usuario
                .FirstOrDefaultAsync(x => x.IdUsuario == value.IdUsuario && x.Eliminado == false)
                ?? throw new Exception("No se encontró el usuario.");

                var entry = _dbSeg.Entry(entity);
                entry.CurrentValues.SetValues(value);
                await _dbSeg.SaveChangesAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se realizó con éxito...!!!";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionResponse<UsuarioEntity>> Delete(UsuarioEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                _dbSeg.Attach(value);
                _dbSeg.Entry(value).Property(x => x.Eliminado).IsModified = true;
                await _dbSeg.SaveChangesAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se realizó con éxito...!!!";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            
            return resultTransaccion;
        }        

        public async Task<ResultadoTransaccionResponse<UsuarioDatosEntity>> ObtienePermisosPorUsuario(UsuarioDatosEntity entidad)
        {
            UsuarioDatosEntity UsuarioAutenticar = null;
            var claveDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Clave);
            var usuarioDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Usuario);

            UsuarioEntity user = await VerificarLogin(new UsuarioEntity { Usuario = usuarioDesEncriptada.ToUpper() });

            ResultadoTransaccionResponse<UsuarioDatosEntity> resultadoTransaccion = new ResultadoTransaccionResponse<UsuarioDatosEntity>();

            try
            {
                if (user.Clave != entidad.Clave)
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Usuario y/o Contraseña incorrecto.";
                    return resultadoTransaccion;
                }

                if (user == null)
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Usuario y/o Contraseña incorrecto.";
                    return resultadoTransaccion;
                }

                MenuRepository menuRepository = new MenuRepository(context);

                var listaAccesoMenu = menuRepository.GetAllPorIdUsuario(user.IdUsuario).Result.ToList();

                // Obtenemos los datos de la sociedad
                var adminInfo = await _dbSap.AdminInfo.FirstOrDefaultAsync();
                // Obtenemos las rutas de vías de acceso
                var attachmentsSettings = await _dbSap.AttachmentsSettings.FirstOrDefaultAsync();
                // Obtenemos los datos de configuración general
                var generalSettings = await _dbSap.GeneralSettings.FirstOrDefaultAsync();
                // Obtenemos los datos del usuario logístico
                var logisticUser = await _dbSeg.LogisticUser.Where(n => n.IdUsuario == user.IdUsuario).FirstOrDefaultAsync();
                var userSap = await _dbSap.Users.Where(n => n.USERID == user.IdUserSap).FirstOrDefaultAsync();

                UsuarioAutenticar = new UsuarioDatosEntity
                {
                    IdUsuario = user == null ? 0 : user.IdUsuario,
                    IdPerfil = user == null ? 0 : user.IdPerfil ?? 0,
                    IdUserSap = userSap == null ? 0 : userSap.USERID,
                    UserSap = userSap == null ? "" : userSap.USER_CODE ?? "",
                    IdLocation = logisticUser == null ? 0 : logisticUser.IdLocation ?? 0,
                    SuperUser = logisticUser == null ? false : logisticUser.SuperUser ?? false,
                    Usuario = user == null ? "" : user.Usuario ?? "",
                    Nombre = user == null ? "" : user.Nombre ?? "",
                    Email = user == null ? "" : user.Email ?? "",
                    Imagen = user.Imagen,

                    CompnyName = adminInfo.CompnyName,
                    CompnyAddr = adminInfo.CompnyAddr,
                    PrintHeadr = adminInfo.PrintHeadr,
                    TaxIdNum = adminInfo.TaxIdNum,
                    Phone1 = adminInfo.Phone1,
                    Phone2 = adminInfo.Phone2,
                    MainCurncy = adminInfo.MainCurncy,
                    SysCurrncy = adminInfo.SysCurrncy,
                    DfltWhs = adminInfo.DfltWhs,
                    DfltSlp = adminInfo.DfltSlp,
                    DfCustTerm = adminInfo.DfCustTerm,
                    AttachPath  = attachmentsSettings == null ? "" : attachmentsSettings.AttachPath ?? "",

                    WhsCodeSpaPar = generalSettings == null ? "" : generalSettings.U_WhsCodeSpaPar ?? "",
                    CodGrpSuppNat = generalSettings == null ? 0 : generalSettings.U_CodGrpSuppNat ?? 0,
                    CodGrpSuppFor = generalSettings == null ? 0 : generalSettings.U_CodGrpSuppFor ?? 0,
                    CodGrpCustNat = generalSettings == null ? 0 : generalSettings.U_CodGrpCustNat ?? 0,
                    CodGrpCustFor = generalSettings == null ? 0 : generalSettings.U_CodGrpCustFor ?? 0,

                    ListaAccesoMenu = listaAccesoMenu
                };

                resultadoTransaccion.ResultadoCodigo = 0;
                resultadoTransaccion.ResultadoDescripcion = "Se autentico correctamente";
                resultadoTransaccion.Data = UsuarioAutenticar;
            }
            catch (Exception ex)
            {
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultadoTransaccion;
        }

        public async Task RecuperarPassword(UsuarioRecuperarPasswordEntity value)
        {
            var data = await _dbSeg.Usuario
            .AsNoTracking()
            .Where(u => u.Usuario == value.Usuario)
            .Select(u => new UsuarioEntity
            {
                IdUsuario = u.IdUsuario,
                Email = u.Email

            })
            .FirstOrDefaultAsync();

            var nuevaClaveAutogenerado = GenerarCodigo(6);

            var nuevaClaveEncriptada = EncriptaHelper.EncryptStringAES(nuevaClaveAutogenerado);


            var entity = new UsuarioEntity
            {
                IdUsuario = data.IdUsuario,
                Clave = nuevaClaveEncriptada
            };

            _dbSeg.Usuario.Attach(entity);
            _dbSeg.Entry(entity).Property(x => x.Clave).IsModified = true;
            await _dbSeg.SaveChangesAsync();


            EmailSenderRepository emailSenderRepository = new EmailSenderRepository(context);

            string template = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"Resources\txt", "plantillaRecuperarPassword.txt"));
            template = template.Replace("{usuario}", value.Usuario);
            template = template.Replace("{password}", nuevaClaveAutogenerado);
            var mensaje = template;
            await emailSenderRepository.SendEmailAsync(data.Email, "Correo Automatico - Recuperar Contraseña", mensaje);
        }

        public async Task GenerarToken(int idUsuario, string email)
        {
            var nuevaTokenAutogenerado = GenerarCodigo(6);

            var nuevaTokenEncriptada = EncriptaHelper.EncryptStringAES(nuevaTokenAutogenerado);

            var entity = await _dbSeg.Usuario
            .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario && x.Eliminado == false)
            ?? throw new Exception("No se encontró el usuario.");

            entity.Token = nuevaTokenEncriptada;
            entity.FecExpToken = DateTime.Now;

            _dbSeg.Entry(entity).Property(x => x.Token).IsModified = true;
            _dbSeg.Entry(entity).Property(x => x.FecExpToken).IsModified = true;

            await _dbSeg.SaveChangesAsync();

            // AQUI SE ENVIA EL TOKEN POR CORREO
            //EmailSenderRepository emailSenderRepository = new EmailSenderRepository(context);

            //string template = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"Resources\txt", "generarToken.txt"));

            //template = template.Replace("{token}", nuevaTokenAutogenerado);

            //await emailSenderRepository.SendEmailAsync(email, $"AuthCode: {nuevaTokenAutogenerado}", template);
        }

        public async Task<ResultadoTransaccionResponse<UsuarioTokenEntity>> ValidarToken(UsuarioTokenEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioTokenEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var usuario = EncriptaHelper.DecryptStringAES(value.Usuario)?.Trim();

                if (string.IsNullOrWhiteSpace(usuario))
                {
                    throw new Exception("Usuario inválido.");
                }

                var data = await _dbSeg.Usuario
                .AsNoTracking()
                .Where(u => u.Usuario == usuario)
                .Select(u => new UsuarioTokenEntity
                {
                    IdUsuario = u.IdUsuario,
                    Usuario = u.Usuario,
                    Token = u.Token,
                    FecExpToken = u.FecExpToken
                })
                .FirstOrDefaultAsync();

                if (data == null)
                {
                    throw new Exception("Usuario no encontrado.");
                }

                var tokenRegistrado = EncriptaHelper.DecryptStringAES(data.Token);
                var tokenIngresado = EncriptaHelper.DecryptStringAES(value.Token);

                if (tokenRegistrado != tokenIngresado)
                {
                    throw new Exception("Token incorrecto.");
                }

                if (DateTime.Now >= data.FecExpToken)
                {
                    throw new Exception("Token expiró.");
                }

                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "OK";
            }
            catch (Exception ex)
            {
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }

        private static string GenerarCodigo(int length)
        {
            Random random = new();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
        }

        public async Task<ResultadoTransaccionResponse<UsuarioEntity>> UpdatePassword(UsuarioUpdatePasswordEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionResponse<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var entity = new UsuarioEntity
                {
                    IdUsuario = value.IdUsuario,
                    Clave = value.Clave
                };

                _dbSeg.Usuario.Attach(entity);
                _dbSeg.Entry(entity).Property(x => x.Clave).IsModified = true;
                await _dbSeg.SaveChangesAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Se realizó con éxito...!!!";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
    }
}

