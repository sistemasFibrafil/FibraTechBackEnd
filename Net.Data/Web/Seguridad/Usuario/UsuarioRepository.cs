using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using System.Data.SqlClient;
using Net.Business.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using System.DirectoryServices;
using Net.Business.Entities.Web;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices.AccountManagement;
namespace Net.Data.Web
{
    public class UsuarioRepository : RepositoryBase<UsuarioEntity>, IUsuarioRepository
    {
        private readonly string _cnxSap;
        private readonly string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly ParametrosTokenConfig _tokenConfig;

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "SEG_GetUsuarioAll";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetUsuarioPorId";
        const string SP_GET_USUARIO = DB_ESQUEMA + "SEG_GetUsuarioPorUsuario";
        const string SP_GET_USUARIO_TOKEN = DB_ESQUEMA + "SEG_GetTokenPorUsuario";
        const string SP_UPDATE_AUTOGENERADA = DB_ESQUEMA + "SEG_SetUsuarioUpdatePassword";
        const string SP_UPDATE_GENERAR_TOKEN = DB_ESQUEMA + "SEG_SetUsuarioUpdateToken";
        const string SP_UPDATE_PASSWORD = DB_ESQUEMA + "SEG_SetUsuarioUpdatePassword";

        const string SP_GET_DETALLE_SOCIEDAD = DB_ESQUEMA + "GES_GetDetalleSociedad";

        public UsuarioRepository(IConnectionSQL context, IConfiguration configuration, IOptions<ParametrosTokenConfig> tokenConfig)
            : base(context)
        {
            _tokenConfig = tokenConfig.Value;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }

        public UsuarioRepository(IConnectionSQL context)
            : base(context)
        {
            _aplicacionName = GetType().Name;
        }

        public Task<IEnumerable<UsuarioEntity>> GetAll(UsuarioEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET));
        }
        public Task<UsuarioEntity> GetById(UsuarioEntity entidad)
        {
            return Task.Run(() => FindById(entidad, SP_GET_ID));
        }
        public UsuarioEntity VerificarLogin(UsuarioEntity entidad)
        {
            return FindById(entidad, SP_GET_USUARIO);
        }

        public async Task<ResultadoTransaccionEntity<UsuarioAutenticarEntity>> Autenticar(UsuarioAutenticarEntity entidad)
        {
            var claveDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Clave);
            var usuarioDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Usuario);

            // Obtenemos los datos del usuario
            UsuarioEntity user = VerificarLogin(new UsuarioEntity { Usuario = usuarioDesEncriptada.ToUpper() });

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
                resultadoTransaccion = await AutenticarUsuarioDirectorioActivo(usuarioDesEncriptada, claveDesEncriptada);
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

            UsuarioAutenticarEntity UsuarioAutenticar = new UsuarioAutenticarEntity { Usuario = user.Usuario.ToUpper(), Token = tokenGenerado, FlgDobleAutenticacion = (bool)_ParametroSistema.FlgDobleAutenticacion, Email = user.Email };

            resultadoTransaccion.ResultadoCodigo = 0;
            resultadoTransaccion.ResultadoDescripcion = "Se autentico correctamente";
            resultadoTransaccion.data = UsuarioAutenticar;
            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<UsuarioDatosEntity>> ObtienePermisosPorUsuario(UsuarioDatosEntity entidad)
        {
            var claveDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Clave);
            var usuarioDesEncriptada = EncriptaHelper.DecryptStringAES(entidad.Usuario);

            UsuarioEntity user = VerificarLogin(new UsuarioEntity { Usuario = usuarioDesEncriptada.ToUpper() });

            ParametroSistemaRepository parametroSistema = new ParametroSistemaRepository(context);
            ParametroSistemaEntity _ParametroSistema = await parametroSistema.GetById(new ParametroSistemaEntity { IdParametrosSistema = 1 });

            ResultadoTransaccionEntity<UsuarioDatosEntity> resultadoTransaccion = new ResultadoTransaccionEntity<UsuarioDatosEntity>();

            if (_ParametroSistema.TipoAutenticacion.Equals("AUTO-NORMAL"))
            {
                resultadoTransaccion = new ResultadoTransaccionEntity<UsuarioDatosEntity>();

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
                var _ResultadoTransaccion = await AutenticarUsuarioDirectorioActivo(usuarioDesEncriptada, claveDesEncriptada);

                if (_ResultadoTransaccion.ResultadoCodigo == -1)
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = _ResultadoTransaccion.ResultadoDescripcion;
                    return resultadoTransaccion;
                }

                if (user == null)
                {
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = "Usuario existe en DA. pero no se encuentra registrado en el Portal (Coordinar con el Area de TI)";
                    return resultadoTransaccion;
                }
            }

            MenuRepository menuRepository = new MenuRepository(context);

            var listaAccesoMenu = menuRepository.GetAllPorIdUsuario(user.IdUsuario).Result.ToList();

            var detalleSociedad = await GetDetalleSociedad();

            UsuarioDatosEntity UsuarioAutenticar = new UsuarioDatosEntity
            {
                Usuario = user.Usuario,
                IdUsuario = user.IdUsuario,
                Imagen = user.Imagen,
                Nombre = user.Nombre,
                Email = user.Email,
                IdPersona = user.IdPersona,
                IdPerfil = user.IdPerfil,
                CodSede = user.CodSede,
                IsNotRestAlmacen = user.IsNotRestAlmacen,
                CodVendedorSAP = user.CodVendedorSAP,
                CompnyName = detalleSociedad.data.CompnyName,
                CompnyAddr = detalleSociedad.data.CompnyAddr,
                PrintHeadr = detalleSociedad.data.PrintHeadr,
                TaxIdNum = detalleSociedad.data.TaxIdNum,
                Phone1 = detalleSociedad.data.Phone1,
                Phone2 = detalleSociedad.data.Phone2,
                MainCurncy = detalleSociedad.data.MainCurncy,
                SysCurrncy = detalleSociedad.data.SysCurrncy,
                WarehouseDefault = user.WarehouseDefault,
                ListaAccesoMenu = listaAccesoMenu
            };

            resultadoTransaccion.ResultadoCodigo = 0;
            resultadoTransaccion.ResultadoDescripcion = "Se autentico correctamente";
            resultadoTransaccion.data = UsuarioAutenticar;
            return resultadoTransaccion;
        }

        public async Task RecuperarPassword(UsuarioRecuperarPasswordEntity entidad)
        {
            var data = FindById(new UsuarioEntity { Usuario = entidad.Usuario }, SP_GET_USUARIO);
            var nuevaClaveAutogenerado = GenerarCodigo(6);

            var nuevaClaveEncriptada = EncriptaHelper.EncryptStringAES(nuevaClaveAutogenerado);

            Update(new UsuarioEntity { IdUsuario = data.IdUsuario, Clave = nuevaClaveEncriptada, RegUsuario = data.IdUsuario, RegEstacion = "Unknown" }, SP_UPDATE_AUTOGENERADA);
            EmailSenderRepository emailSenderRepository = new EmailSenderRepository(context);

            string template = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"Resources\txt", "plantillaRecuperarPassword.txt"));
            template = template.Replace("{usuario}", entidad.Usuario);
            template = template.Replace("{password}", nuevaClaveAutogenerado);
            var mensaje = template;
            await emailSenderRepository.SendEmailAsync(data.Email, "Correo Automatico - Recuperar Contraseña", mensaje);
        }

        public async Task GenerarToken(int IdUsuario, string Email)
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

        public Task UpdatePassword(UsuarioEntity entidad)
        {
            return Task.Run(() =>
            {
                Update(new UsuarioEntity { IdUsuario = entidad.IdUsuario, Clave = entidad.ClaveOrigen, RegUsuario = entidad.RegUsuario, RegEstacion = entidad.RegEstacion }, SP_UPDATE_PASSWORD);
            });
        }

        public Task<ResultadoTransaccionEntity<UsuarioAutenticarEntity>> AutenticarUsuarioDirectorioActivo(string usuario, string password)
        {

            ResultadoTransaccionEntity<UsuarioAutenticarEntity> vResultadoTransaccion = new ResultadoTransaccionEntity<UsuarioAutenticarEntity>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            string domainName = "CLISAFENET";

            //SearchResultCollection results = null;

            return Task.Run(() =>
            {

                try
                {
                    DirectoryEntry de = new DirectoryEntry("LDAP://" + domainName, usuario, password);
                    DirectorySearcher dsearch = new DirectorySearcher(de);

                    PrincipalContext AD = new PrincipalContext(ContextType.Domain, domainName);
                    UserPrincipal u = new UserPrincipal(AD);
                    UserPrincipal user = UserPrincipal.FindByIdentity(AD, usuario);

                    u.SamAccountName = usuario;

                    if (user != null)
                    {

                        if (user.LastPasswordSet.HasValue == false)
                        {
                            if (user.PasswordNeverExpires == false)
                            {
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = "Favor de cambiar su password en una maquina con dominio";

                                return vResultadoTransaccion;
                            }
                        }

                        //long daysLeft = 0;
                        //if (!u.PasswordNeverExpires)
                        //{
                        //    using (DirectoryEntry entry2 = new DirectoryEntry("WinNT://" + domainName + '/' + usuario + ",user"))
                        //    {
                        //        var PasswordExpirationDate = entry2.InvokeGet("PasswordExpirationDate");

                        //        if (PasswordExpirationDate != null)
                        //        {
                        //            daysLeft = long.Parse(Math.Round((DateTime.Now - (DateTime)PasswordExpirationDate).TotalDays).ToString());
                        //            if (daysLeft > 0)
                        //            {
                        //                vResultadoTransaccion.ResultadoCodigo = -1;
                        //                vResultadoTransaccion.ResultadoDescripcion = "La contraseña del usuario ha caducado";

                        //                return vResultadoTransaccion;

                        //            }
                        //        }
                        //    }
                        //}

                    }

                    //dsearch.PropertiesToLoad.Add("name");
                    //dsearch.PropertiesToLoad.Add("mail");
                    //dsearch.PropertiesToLoad.Add("givenname");
                    //dsearch.PropertiesToLoad.Add("sn");
                    //dsearch.PropertiesToLoad.Add("userPrincipalName");
                    //dsearch.PropertiesToLoad.Add("distinguishedName");
                    //dsearch.PropertiesToLoad.Add("samaccountname");
                    //dsearch.PropertiesToLoad.Add("displayname");
                    //dsearch.PropertiesToLoad.Add("title");
                    //dsearch.PropertiesToLoad.Add("st");
                    //dsearch.PropertiesToLoad.Add("department");

                    //dsearch.Filter = "samaccountname=" + usuario;

                    //results = dsearch.FindAll();
                    //List<string> nombres = new List<string>();
                    //object objUser = new { nombre = string.Empty, usuario = string.Empty };
                    //string nombre, nombreCompleto, mail, givenname, sn, title, department = string.Empty;
                    //string samaccountnamename = string.Empty;
                    //int userCount = 0;

                    //nombre = string.Empty;
                    //nombreCompleto = string.Empty;

                    //foreach (SearchResult sr in results)
                    //{
                    //    userCount += 1;
                    //    if (sr.Properties["mail"].Count > 0) mail = sr.Properties["mail"][0].ToString();
                    //    if (sr.Properties["givenname"].Count > 0) givenname = sr.Properties["givenname"][0].ToString();
                    //    if (sr.Properties["sn"].Count > 0) sn = sr.Properties["sn"][0].ToString();
                    //    if (sr.Properties["title"].Count > 0) title = sr.Properties["title"][0].ToString();
                    //    if (sr.Properties["department"].Count > 0) department = sr.Properties["department"][0].ToString();
                    //    if (sr.Properties["displayname"].Count > 0) nombreCompleto = sr.Properties["displayname"][0].ToString();
                    //}

                    //if (userCount > 1)
                    //{
                    //    vResultadoTransaccion.ResultadoCodigo = -1;
                    //    vResultadoTransaccion.ResultadoDescripcion = "error se encontraron 2 cuentas iguales: " + usuario;

                    //    return vResultadoTransaccion;
                    //}

                    if (!AD.ValidateCredentials(usuario, password))
                    {
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Usuario o contraseña, inválidos";

                        return vResultadoTransaccion;
                    }

                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Acceso Correcto" + usuario;

                    return vResultadoTransaccion;

                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();

                    return vResultadoTransaccion;
                }
            });

        }

        public Task<ResultadoTransaccionEntity<bool>> ValidaExisteUsuarioDirectorioActivo(string usuario)
        {

            ResultadoTransaccionEntity<bool> vResultadoTransaccion = new ResultadoTransaccionEntity<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            string domainName = "CLISAFENET";

            bool userExists = false;

            return Task.Run(() =>
            {

                try
                {
                    using (var ctx = new PrincipalContext(ContextType.Domain, domainName))
                    {
                        using (var user = UserPrincipal.FindByIdentity(ctx, usuario))
                        {
                            if (user != null)
                            {
                                userExists = true;
                                user.Dispose();
                            }
                            else
                            {
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = "Usuario no existe en el Directorio Activo";
                                vResultadoTransaccion.data = userExists;
                                return vResultadoTransaccion;
                            }
                        }
                    }

                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Acceso Correcto" + usuario;
                    vResultadoTransaccion.data = userExists;

                    return vResultadoTransaccion;

                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    vResultadoTransaccion.data = userExists;
                    return vResultadoTransaccion;
                }
            });

        }

        public async Task<ResultadoTransaccionEntity<DetalleSociedadSapEntity>> GetDetalleSociedad()
        {
            var response = new DetalleSociedadSapEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<DetalleSociedadSapEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLE_SOCIEDAD, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<DetalleSociedadSapEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Datos obtenidos con éxito ..!";
                    resultTransaccion.data = response;
                }
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }
    }
}

