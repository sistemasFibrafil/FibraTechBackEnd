using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using System.Data.SqlClient;
using Net.Business.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using Net.Business.Entities.Web;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Web
{
    public class UsuarioRepository : RepositoryBase<UsuarioEntity>, IUsuarioRepository
    {
        private readonly string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly DataContextSap _dbSap;
        private readonly DataContextSeg _dbSeg;
        private readonly ParametrosTokenConfig _tokenConfig;
        private readonly CompanyProviderSap _companyProviderSap;

        const string DB_ESQUEMA = "";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetUsuarioInsert";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetUsuarioUpdate";
        const string SP_GET_USUARIO = DB_ESQUEMA + "SEG_GetUsuarioPorUsuario";
        const string SP_GET_USUARIO_TOKEN = DB_ESQUEMA + "SEG_GetTokenPorUsuario";
        const string SP_UPDATE_AUTOGENERADA = DB_ESQUEMA + "SEG_SetUsuarioUpdatePassword";
        const string SP_UPDATE_GENERAR_TOKEN = DB_ESQUEMA + "SEG_SetUsuarioUpdateToken";

        public UsuarioRepository(IConnectionSQL context, IConfiguration configuration, IOptions<ParametrosTokenConfig> tokenConfig, DataContextSap dbSap, DataContextSeg dbSeg, CompanyProviderSap companyProviderSap)
            : base(context)
        {
            _dbSap = dbSap;
            _dbSeg = dbSeg;
            _tokenConfig = tokenConfig.Value;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }

        public async Task<ResultadoTransaccionEntity<UsuarioQueryEntity>> GetList()
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsuarioQueryEntity>
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
                resultTransaccion.dataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<UsuarioQueryEntity>> GetListByFilter(UsuarioFilterEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsuarioQueryEntity>
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
                        EF.Functions.Like(EF.Functions.Collate(x.Clave!, GlobalVariables.CI), $"%{filter}%") ||
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
                resultTransaccion.dataList = list;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<UsuarioEntity>> GetById(UsuarioEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsuarioEntity>
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
                resultTransaccion.data = data;
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
        public async Task<ResultadoTransaccionEntity<UsuarioAutenticarEntity>> Autenticar(UsuarioAutenticarEntity entidad)
        {
            var claveDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Clave);
            var usuarioDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Usuario);

            // Obtenemos los datos del usuario
            UsuarioEntity user = await VerificarLogin(new UsuarioEntity { Usuario = usuarioDesEncriptada.ToUpper() });

            ResultadoTransaccionEntity<UsuarioAutenticarEntity> resultadoTransaccion = new ResultadoTransaccionEntity<UsuarioAutenticarEntity>();

            ParametroSistemaRepository parametroSistema = new ParametroSistemaRepository(context);
            ParametroSistemaEntity _ParametroSistema = await parametroSistema.GetById(new ParametroSistemaEntity { IdParametrosSistema = 1 });

            if (_ParametroSistema.TipoAutenticacion.Equals("AUTO-NORMAL"))
            {
                resultadoTransaccion = new ResultadoTransaccionEntity<UsuarioAutenticarEntity>();

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

                await GenerarToken((int)user.IdUsuario, user.Email);
            }

            UsuarioAutenticarEntity UsuarioAutenticar = new UsuarioAutenticarEntity { Usuario = user.Clave.ToUpper(), Token = tokenGenerado, FlgDobleAutenticacion = (bool)_ParametroSistema.FlgDobleAutenticacion, Email = user.Email };


            // Conexión a SAP
            var company = _companyProviderSap.GetCompany();

            resultadoTransaccion.ResultadoCodigo = 0;
            resultadoTransaccion.ResultadoDescripcion = "Se autentico correctamente";
            resultadoTransaccion.data = UsuarioAutenticar;
            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<UsuarioEntity>> Create(UsuarioCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
            {
                try
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter oParam = new SqlParameter("@IdUsuario", value.IdUsuario);
                        oParam.SqlDbType = SqlDbType.Int;
                        oParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(oParam);

                        cmd.Parameters.Add(new SqlParameter("@IdPerfil", value.IdPerfil));
                        cmd.Parameters.Add(new SqlParameter("@IdUserSap", value.IdUserSap));
                        cmd.Parameters.Add(new SqlParameter("@Nombre", value.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@ApellidoPaterno", value.ApellidoPaterno));
                        cmd.Parameters.Add(new SqlParameter("@ApellidoMaterno", value.ApellidoMaterno));
                        cmd.Parameters.Add(new SqlParameter("@NroDocumento", value.NroDocumento));
                        cmd.Parameters.Add(new SqlParameter("@NroTelefono", value.NroTelefono));
                        cmd.Parameters.Add(new SqlParameter("@Usuario", value.Usuario));
                        cmd.Parameters.Add(new SqlParameter("@Clave", value.Clave));
                        cmd.Parameters.Add(new SqlParameter("@Email", value.Email));
                        cmd.Parameters.Add(new SqlParameter("@Imagen", value.Imagen));
                        cmd.Parameters.Add(new SqlParameter("@Firma", value.Firma));
                        cmd.Parameters.Add(new SqlParameter("@ThemeDark", value.ThemeDark));
                        cmd.Parameters.Add(new SqlParameter("@ThemeColor", value.ThemeColor));
                        cmd.Parameters.Add(new SqlParameter("@TypeMenu", value.TypeMenu));
                        cmd.Parameters.Add(new SqlParameter("@Activo", value.Activo));
                        cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                        cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                        await cmd.ExecuteNonQueryAsync();

                        value.IdUsuario = (int)cmd.Parameters["@IdUsuario"].Value;
                    }

                    resultTransaccion.IdRegistro = (int)value.IdUsuario;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Se realizo con Exito...!!!";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }

            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<UsuarioEntity>> Update(UsuarioUpdateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
            {
                try
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.IdUsuario));
                        cmd.Parameters.Add(new SqlParameter("@IdPerfil", value.IdPerfil));
                        cmd.Parameters.Add(new SqlParameter("@IdUserSap", value.IdUserSap));
                        cmd.Parameters.Add(new SqlParameter("@Nombre", value.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@ApellidoPaterno", value.ApellidoPaterno));
                        cmd.Parameters.Add(new SqlParameter("@ApellidoMaterno", value.ApellidoMaterno));
                        cmd.Parameters.Add(new SqlParameter("@NroDocumento", value.NroDocumento));
                        cmd.Parameters.Add(new SqlParameter("@NroTelefono", value.NroTelefono));
                        cmd.Parameters.Add(new SqlParameter("@Clave", value.Clave));
                        cmd.Parameters.Add(new SqlParameter("@Email", value.Email));
                        cmd.Parameters.Add(new SqlParameter("@Imagen", value.Imagen));
                        cmd.Parameters.Add(new SqlParameter("@Firma", value.Firma));
                        cmd.Parameters.Add(new SqlParameter("@ThemeDark", value.ThemeDark));
                        cmd.Parameters.Add(new SqlParameter("@ThemeColor", value.ThemeColor));
                        cmd.Parameters.Add(new SqlParameter("@TypeMenu", value.TypeMenu));
                        cmd.Parameters.Add(new SqlParameter("@Activo", value.Activo));
                        cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                        cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                        await cmd.ExecuteNonQueryAsync();
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Se realizÓ con éxito...!!!";
                }
                catch (Exception ex)
                {
                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }
            
            return resultTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<UsuarioEntity>> Delete(UsuarioEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsuarioEntity>
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
        public async Task<ResultadoTransaccionEntity<UsuarioDatosEntity>> ObtienePermisosPorUsuario(UsuarioDatosEntity entidad)
        {
            var claveDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Clave);
            var usuarioDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Usuario);

            UsuarioEntity user = await VerificarLogin(new UsuarioEntity { Usuario = usuarioDesEncriptada.ToUpper() });

            ResultadoTransaccionEntity<UsuarioDatosEntity> resultadoTransaccion = new ResultadoTransaccionEntity<UsuarioDatosEntity>();

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
            // Obtenemos los datos de configuración general
            var generalSettings = await _dbSap.GeneralSettings.FirstOrDefaultAsync();
            // Obtenemos los datos del usuario logístico
            var logisticUser = await _dbSeg.LogisticUser.Where(n => n.IdUsuario == user.IdUsuario).FirstOrDefaultAsync();
            var userSap = await _dbSap.Users.Where(n => n.USERID == user.IdUserSap).FirstOrDefaultAsync();

            UsuarioDatosEntity UsuarioAutenticar = new UsuarioDatosEntity
            {
                IdUsuario = user.IdUsuario,
                IdPerfil = user.IdPerfil,
                IdUserSap = userSap != null ? userSap.USERID : 0,
                UserSap = userSap !=null? userSap.USER_CODE: "",
                IdLocation = logisticUser != null? logisticUser.IdLocation : 0,
                SuperUser = logisticUser != null? logisticUser.SuperUser : false,
                Usuario = user.Usuario,
                Nombre = user.Nombre,
                Email = user.Email,
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
                WhsCodeSpareParts = generalSettings.U_WhsCodeSp,
                ListaAccesoMenu = listaAccesoMenu
            };

            resultadoTransaccion.ResultadoCodigo = 0;
            resultadoTransaccion.ResultadoDescripcion = "Se autentico correctamente";
            resultadoTransaccion.data = UsuarioAutenticar;
            return resultadoTransaccion;
        }
        public async Task RecuperarPassword(UsuarioRecuperarPasswordEntity entidad)
        {
            var data = FindById(new UsuarioEntity { Clave = entidad.Usuario }, SP_GET_USUARIO);
            var nuevaClaveAutogenerado = GenerarCodigo(6);

            var nuevaClaveEncriptada = EncriptaHelper.EncryptStringAES(nuevaClaveAutogenerado);

            Update(new UsuarioEntity { IdUsuario = data.IdUsuario, Clave = nuevaClaveEncriptada }, SP_UPDATE_AUTOGENERADA);
            EmailSenderRepository emailSenderRepository = new EmailSenderRepository(context);

            string template = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"Resources\txt", "plantillaRecuperarPassword.txt"));
            template = template.Replace("{usuario}", entidad.Usuario);
            template = template.Replace("{password}", nuevaClaveAutogenerado);
            var mensaje = template;
            await emailSenderRepository.SendEmailAsync(data.Email, "Correo Automatico - Recuperar Contraseña", mensaje);
        }
        public async Task GenerarToken(int IdUsuario, string Email)
        {
            await Task.Run(() =>
            {
                var nuevaClaveAutogenerado = GenerarCodigo(6);

                var nuevaClaveEncriptada = EncriptaHelper.EncryptStringAES(nuevaClaveAutogenerado);

                context.ExecuteSqlUpdate(SP_UPDATE_GENERAR_TOKEN, new UsuarioTokenEntity { IdUsuario = IdUsuario, Token = nuevaClaveEncriptada, RegUsuario = IdUsuario, RegEstacion = "Unknown" });

                // AQUI SE ENVIA EL TOKEN POR CORREO
                //EmailSenderRepository emailSenderRepository = new EmailSenderRepository(context);

                //string template = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"Resources\txt", "generarToken.txt"));
                //template = template.Replace("{token}", nuevaClaveAutogenerado);
                //var mensaje = template;
                //await emailSenderRepository.SendEmailAsync(Email, string.Format("AuthCode: {0}", nuevaClaveAutogenerado), mensaje);
            });
        }
        public async Task<ResultadoTransaccionEntity<UsuarioTokenEntity>> ValidarToken(UsuarioTokenEntity value)
        {
            ResultadoTransaccionEntity<UsuarioTokenEntity> resultadoTransaccion = new ResultadoTransaccionEntity<UsuarioTokenEntity>();

            try
            {

                var usuarioDesEncriptada = EncriptaHelper.DecryptStringAES(value.Usuario);

                var data = context.ExecuteSqlViewId<UsuarioTokenEntity>(SP_GET_USUARIO_TOKEN, new UsuarioEntity { Usuario = usuarioDesEncriptada.ToUpper() });

                if (EncriptaHelper.DecryptStringAES(data.Token) == EncriptaHelper.DecryptStringAES(value.Token))
                {

                    DateTime fechaActual = DateTime.Now;

                    if (fechaActual < data.FecExpToken)
                    {
                        resultadoTransaccion.ResultadoCodigo = 0;
                        resultadoTransaccion.ResultadoDescripcion = "OK";
                        return resultadoTransaccion;
                    }
                    else
                    {
                        resultadoTransaccion.ResultadoCodigo = -1;
                        resultadoTransaccion.ResultadoDescripcion = "Token Expiró";
                        return resultadoTransaccion;
                    }
                }
                else
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Token Incorrecto";
                    return resultadoTransaccion;
                }
            }
            catch (Exception ex)
            {
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message;
                return resultadoTransaccion;
            }

        }
        private string GenerarCodigo(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<ResultadoTransaccionEntity<UsuarioEntity>> UpdatePassword(UsuarioUpdatePasswordEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<UsuarioEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            //return Task.Run((Action)(() =>
            //{
            //    //Update(new UsuarioUpdatePasswordEntity { IdUsuario = entidad.IdUsuario, Clave = entidad.Clave }, SP_UPDATE_PASSWORD);
            //}));


            return resultTransaccion;
        }
    }
}

