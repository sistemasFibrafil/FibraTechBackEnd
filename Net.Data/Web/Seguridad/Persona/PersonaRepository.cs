using System;
using System.Data;
using Net.Connection;
using System.Transactions;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Web
{
    public class PersonaRepository : RepositoryBase<PersonaEntity>, IPersonaRepository
    {
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IConfiguration _configuration;

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "SEG_GetPersonaAll";
        const string SP_GET_ID = DB_ESQUEMA + "SEG_GetPersonaPorId";
        const string SP_GET_USUARIO_ALMACEN_ID = DB_ESQUEMA + "SEG_GetUsuarioAlmacenPorId";
        const string SP_GET_USUARIO_NEGOCIO_ID = DB_ESQUEMA + "SEG_GetUsuarioNegocioPorId";
        const string SP_GET_USUARIO_TABLA_ID = DB_ESQUEMA + "SEG_GetUsuarioTablaPorId";
        const string SP_GET_ID_USUARIO = DB_ESQUEMA + "SEG_GetUsuarioPorPersona";
        const string SP_INSERT = DB_ESQUEMA + "SEG_SetPersonaInsert";
        const string SP_INSERT_USUARIO = DB_ESQUEMA + "SEG_SetUsuarioInsert";
        const string SP_MERGE_USUARIO_ALMACEN = DB_ESQUEMA + "SEG_SetUsuarioAlmacenMerge";
        const string SP_MERGE_USUARIO_NEGOCIO = DB_ESQUEMA + "SEG_SetUsuarioNegocioMerge";
        const string SP_MERGE_USUARIO_TABLA = DB_ESQUEMA + "SEG_SetUsuarioTablaMerge";
        const string SP_DELETE = DB_ESQUEMA + "SEG_SetPersonaDelete";
        const string SP_UPDATE = DB_ESQUEMA + "SEG_SetPersonaUpdate";
        const string SP_UPDATE_USUARIO = DB_ESQUEMA + "SEG_SetUsuarioUpdate";

        public PersonaRepository(IConnectionSQL context)
            : base(context)
        {
        }

        public Task<IEnumerable<PersonaEntity>> GetAll(PersonaEntity entidad)
        {
            return Task.Run(() => FindAll(entidad, SP_GET));
        }
        public Task<PersonaEntity> GetById(PersonaEntity entidad)
        {
            var objListPrincipal = Task.Run(() =>
            {
                PersonaEntity p = context.ExecuteSqlViewId<PersonaEntity>(SP_GET_ID, entidad);
                UsuarioEntity usu = context.ExecuteSqlViewId<UsuarioEntity>(SP_GET_ID_USUARIO, new UsuarioEntity { IdPersona = entidad.IdPersona });

                //Inicializamos la listas
                usu.ListUsuarioAlmacen = new List<UsuarioAlmacenEntity>();
                usu.ListUsuarioNegocio = new List<UsuarioNegocioEntity>();
                usu.ListUsuarioTabla = new List<UsuarioTablaEntity>();

                //usu.ListUsuarioAlmacen = context.ExecuteSqlViewAll<BE_UsuarioAlmacen>(SP_GET_USUARIO_ALMACEN_ID, new BE_Usuario { IdUsuario = usu.IdUsuario }).ToList();
                //usu.ListUsuarioNegocio = context.ExecuteSqlViewAll<BE_UsuarioNegocio>(SP_GET_USUARIO_NEGOCIO_ID, new BE_Usuario { IdUsuario = usu.IdUsuario }).ToList();
                //usu.ListUsuarioTabla = context.ExecuteSqlViewAll<BE_UsuarioTabla>(SP_GET_USUARIO_TABLA_ID, new BE_Usuario { IdUsuario = usu.IdUsuario }).ToList();

                usu.ClaveOrigen = usu.Clave;
                usu.Clave = usu.Clave;
                p.EntidadUsuario = usu;
                return p;
            });
            return objListPrincipal;
        }
        public async Task<ResultadoTransaccionEntity<PersonaEntity>> Create(PersonaEntity value)
        {

            ResultadoTransaccionEntity<PersonaEntity> vResultadoTransaccion = new ResultadoTransaccionEntity<PersonaEntity>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    using (CommittableTransaction transaction = new CommittableTransaction())
                    {
                        await conn.OpenAsync();
                        conn.EnlistTransaction(transaction);

                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                SqlParameter oParam = new SqlParameter("@IdPersona", value.IdPersona);
                                oParam.SqlDbType = SqlDbType.Int;
                                oParam.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(oParam);

                                cmd.Parameters.Add(new SqlParameter("@Nombre", value.Nombre));
                                cmd.Parameters.Add(new SqlParameter("@ApellidoPaterno", value.ApellidoPaterno));
                                cmd.Parameters.Add(new SqlParameter("@ApellidoMaterno", value.ApellidoMaterno));
                                cmd.Parameters.Add(new SqlParameter("@NroDocumento", value.NroDocumento));
                                cmd.Parameters.Add(new SqlParameter("@NroTelefono", value.NroTelefono));
                                cmd.Parameters.Add(new SqlParameter("@CodSede", value.CodSede));
                                cmd.Parameters.Add(new SqlParameter("@IsNotRestAlmacen", value.IsNotRestAlmacen));
                                cmd.Parameters.Add(new SqlParameter("@FlgActivo", value.FlgActivo));
                                cmd.Parameters.Add(new SqlParameter("@CodCentroCosto", value.CodCentroCosto));
                                cmd.Parameters.Add(new SqlParameter("@CodCentro", value.CodCentro));
                                cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                                cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                                await cmd.ExecuteNonQueryAsync();

                                value.IdPersona = (int)cmd.Parameters["@IdPersona"].Value;
                                value.EntidadUsuario.IdPersona = value.IdPersona;
                            }

                            ParametroSistemaRepository parametroSistemaRepository = new ParametroSistemaRepository(context);
                            ParametroSistemaEntity resultadoTransaccionParametrosSistema = await parametroSistemaRepository.GetById(new ParametroSistemaEntity { IdParametrosSistema = 1 });

                            if (resultadoTransaccionParametrosSistema.TipoAutenticacion.Equals("AUTO-ACTIVE-DIRECTORY"))
                            {
                                UsuarioRepository usuarioRepository = new UsuarioRepository(context);
                                ResultadoTransaccionEntity<bool> resultadoTransaccionEXisteUsuarioAD = await usuarioRepository.ValidaExisteUsuarioDirectorioActivo(value.EntidadUsuario.Usuario);

                                if (resultadoTransaccionEXisteUsuarioAD.ResultadoCodigo == -1)
                                {
                                    transaction.Rollback();
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionEXisteUsuarioAD.ResultadoDescripcion;
                                    return vResultadoTransaccion;
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand(SP_INSERT_USUARIO, conn))
                            {
                                cmd.Parameters.Clear();
                                cmd.CommandType = CommandType.StoredProcedure;

                                SqlParameter oParam = new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario);
                                oParam.SqlDbType = SqlDbType.Int;
                                oParam.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(oParam);

                                cmd.Parameters.Add(new SqlParameter("@IdPersona", value.EntidadUsuario.IdPersona));
                                cmd.Parameters.Add(new SqlParameter("@IdPerfil", value.EntidadUsuario.IdPerfil));
                                cmd.Parameters.Add(new SqlParameter("@Usuario", value.EntidadUsuario.Usuario));
                                cmd.Parameters.Add(new SqlParameter("@Clave", value.EntidadUsuario.ClaveOrigen));
                                cmd.Parameters.Add(new SqlParameter("@Email", value.EntidadUsuario.Email));
                                cmd.Parameters.Add(new SqlParameter("@Imagen", value.EntidadUsuario.Imagen));
                                cmd.Parameters.Add(new SqlParameter("@Firma", value.EntidadUsuario.Firma));
                                cmd.Parameters.Add(new SqlParameter("@ThemeDark", value.EntidadUsuario.ThemeDark));
                                cmd.Parameters.Add(new SqlParameter("@ThemeColor", value.EntidadUsuario.ThemeColor));
                                cmd.Parameters.Add(new SqlParameter("@TypeMenu", value.EntidadUsuario.TypeMenu));
                                cmd.Parameters.Add(new SqlParameter("@CodVendedorSAP", value.EntidadUsuario.CodVendedorSAP));
                                cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                                cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                                await cmd.ExecuteNonQueryAsync();

                                value.EntidadUsuario.IdUsuario = (int)cmd.Parameters["@IdUsuario"].Value;
                            }

                            //if (value.EntidadUsuario.ListUsuarioAlmacen.Count > 0)
                            //{
                            //    foreach (var item in value.EntidadUsuario.ListUsuarioAlmacen)
                            //    {
                            //        using (SqlCommand cmd = new SqlCommand(SP_MERGE_USUARIO_ALMACEN, conn))
                            //        {
                            //            cmd.Parameters.Clear();
                            //            cmd.CommandType = CommandType.StoredProcedure;

                            //            cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario));
                            //            cmd.Parameters.Add(new SqlParameter("@WarehouseCode", item.WarehouseCode));
                            //            cmd.Parameters.Add(new SqlParameter("@WarehouseDefault", item.WarehouseDefault));
                            //            cmd.Parameters.Add(new SqlParameter("@FlgActivo", item.FlgActivo));
                            //            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                            //            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                            //            await cmd.ExecuteNonQueryAsync();
                            //        }
                            //    }
                            //}

                            //if (value.EntidadUsuario.ListUsuarioNegocio.Count > 0)
                            //{
                            //    foreach (var item in value.EntidadUsuario.ListUsuarioNegocio)
                            //    {
                            //        using (SqlCommand cmd = new SqlCommand(SP_MERGE_USUARIO_NEGOCIO, conn))
                            //        {
                            //            cmd.Parameters.Clear();
                            //            cmd.CommandType = CommandType.StoredProcedure;

                            //            cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario));
                            //            cmd.Parameters.Add(new SqlParameter("@GroupCode", item.GroupCode));
                            //            cmd.Parameters.Add(new SqlParameter("@FlgActivo", item.FlgActivo));
                            //            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                            //            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                            //            await cmd.ExecuteNonQueryAsync();
                            //        }
                            //    }
                            //}

                            //if (value.EntidadUsuario.ListUsuarioTabla.Count > 0)
                            //{
                            //    foreach (var item in value.EntidadUsuario.ListUsuarioTabla)
                            //    {
                            //        using (SqlCommand cmd = new SqlCommand(SP_MERGE_USUARIO_TABLA, conn))
                            //        {
                            //            cmd.Parameters.Clear();
                            //            cmd.CommandType = CommandType.StoredProcedure;

                            //            cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario));
                            //            cmd.Parameters.Add(new SqlParameter("@Tabla", item.Tabla));
                            //            cmd.Parameters.Add(new SqlParameter("@CodigoTabla", item.CodigoTabla));
                            //            cmd.Parameters.Add(new SqlParameter("@FlgActivo", item.FlgActivo));
                            //            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                            //            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                            //            await cmd.ExecuteNonQueryAsync();
                            //        }
                            //    }
                            //}


                            vResultadoTransaccion.IdRegistro = (int)value.EntidadUsuario.IdUsuario;
                            vResultadoTransaccion.ResultadoCodigo = 0;
                            vResultadoTransaccion.ResultadoDescripcion = "Se realizo con Exito...!!!";

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<PersonaEntity>> Update(PersonaEntity value)
        {

            ResultadoTransaccionEntity<PersonaEntity> vResultadoTransaccion = new ResultadoTransaccionEntity<PersonaEntity>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
            {
                using (CommittableTransaction transaction = new CommittableTransaction())
                {
                    await conn.OpenAsync();
                    conn.EnlistTransaction(transaction);

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SP_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@IdPersona", value.IdPersona));
                            cmd.Parameters.Add(new SqlParameter("@Nombre", value.Nombre));
                            cmd.Parameters.Add(new SqlParameter("@ApellidoPaterno", value.ApellidoPaterno));
                            cmd.Parameters.Add(new SqlParameter("@ApellidoMaterno", value.ApellidoMaterno));
                            cmd.Parameters.Add(new SqlParameter("@NroDocumento", value.NroDocumento));
                            cmd.Parameters.Add(new SqlParameter("@NroTelefono", value.NroTelefono));
                            cmd.Parameters.Add(new SqlParameter("@CodSede", value.CodSede));
                            cmd.Parameters.Add(new SqlParameter("@IsNotRestAlmacen", value.IsNotRestAlmacen));
                            cmd.Parameters.Add(new SqlParameter("@FlgActivo", value.FlgActivo));
                            cmd.Parameters.Add(new SqlParameter("@CodCentroCosto", value.CodCentroCosto));
                            cmd.Parameters.Add(new SqlParameter("@CodCentro", value.CodCentro));
                            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                            await cmd.ExecuteNonQueryAsync();
                        }

                        using (SqlCommand cmd = new SqlCommand(SP_UPDATE_USUARIO, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario));
                            cmd.Parameters.Add(new SqlParameter("@IdPerfil", value.EntidadUsuario.IdPerfil));

                            cmd.Parameters.Add(new SqlParameter("@Clave", value.EntidadUsuario.ClaveOrigen));
                            cmd.Parameters.Add(new SqlParameter("@Email", value.EntidadUsuario.Email));
                            cmd.Parameters.Add(new SqlParameter("@Imagen", value.EntidadUsuario.Imagen));
                            cmd.Parameters.Add(new SqlParameter("@Firma", value.EntidadUsuario.Firma));
                            cmd.Parameters.Add(new SqlParameter("@ThemeDark", value.EntidadUsuario.ThemeDark));
                            cmd.Parameters.Add(new SqlParameter("@ThemeColor", value.EntidadUsuario.ThemeColor));
                            cmd.Parameters.Add(new SqlParameter("@TypeMenu", value.EntidadUsuario.TypeMenu));
                            cmd.Parameters.Add(new SqlParameter("@CodVendedorSAP", value.EntidadUsuario.CodVendedorSAP));
                            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                            await cmd.ExecuteNonQueryAsync();
                        }

                        //if (value.EntidadUsuario.ListUsuarioAlmacen.Count > 0)
                        //{
                        //    foreach (var item in value.EntidadUsuario.ListUsuarioAlmacen)
                        //    {
                        //        using (SqlCommand cmd = new SqlCommand(SP_MERGE_USUARIO_ALMACEN, conn))
                        //        {
                        //            cmd.Parameters.Clear();
                        //            cmd.CommandType = CommandType.StoredProcedure;

                        //            cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario));
                        //            cmd.Parameters.Add(new SqlParameter("@WarehouseCode", item.WarehouseCode));
                        //            cmd.Parameters.Add(new SqlParameter("@WarehouseDefault", item.WarehouseDefault));
                        //            cmd.Parameters.Add(new SqlParameter("@FlgActivo", item.FlgActivo));
                        //            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                        //            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                        //            await cmd.ExecuteNonQueryAsync();
                        //        }
                        //    }
                        //}

                        //if (value.EntidadUsuario.ListUsuarioNegocio.Count > 0)
                        //{
                        //    foreach (var item in value.EntidadUsuario.ListUsuarioNegocio)
                        //    {
                        //        using (SqlCommand cmd = new SqlCommand(SP_MERGE_USUARIO_NEGOCIO, conn))
                        //        {
                        //            cmd.Parameters.Clear();
                        //            cmd.CommandType = CommandType.StoredProcedure;

                        //            cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario));
                        //            cmd.Parameters.Add(new SqlParameter("@GroupCode", item.GroupCode));
                        //            cmd.Parameters.Add(new SqlParameter("@FlgActivo", item.FlgActivo));
                        //            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                        //            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                        //            await cmd.ExecuteNonQueryAsync();
                        //        }
                        //    }
                        //}

                        //if (value.EntidadUsuario.ListUsuarioTabla.Count > 0)
                        //{
                        //    foreach (var item in value.EntidadUsuario.ListUsuarioTabla)
                        //    {
                        //        using (SqlCommand cmd = new SqlCommand(SP_MERGE_USUARIO_TABLA, conn))
                        //        {
                        //            cmd.Parameters.Clear();
                        //            cmd.CommandType = CommandType.StoredProcedure;

                        //            cmd.Parameters.Add(new SqlParameter("@IdUsuario", value.EntidadUsuario.IdUsuario));
                        //            cmd.Parameters.Add(new SqlParameter("@Tabla", item.Tabla));
                        //            cmd.Parameters.Add(new SqlParameter("@CodigoTabla", item.CodigoTabla));
                        //            cmd.Parameters.Add(new SqlParameter("@FlgActivo", item.FlgActivo));
                        //            cmd.Parameters.Add(new SqlParameter("@RegUsuario", value.RegUsuario));
                        //            cmd.Parameters.Add(new SqlParameter("@RegEstacion", value.RegEstacion));

                        //            await cmd.ExecuteNonQueryAsync();
                        //        }
                        //    }
                        //}

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "Se realizo con Exito...!!!";

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                        return vResultadoTransaccion;
                    }
                }
            }

            return vResultadoTransaccion;

        }
        public Task<ResultadoTransaccionEntity<PersonaEntity>> Delete(PersonaEntity entidad)
        {
            ResultadoTransaccionEntity<PersonaEntity> vResultadoTransaccion = new ResultadoTransaccionEntity<PersonaEntity>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            return Task.Run(() =>
            {
                try
                {
                    Delete(entidad, SP_DELETE);

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Se realizo con Exito...!!!";
                    return vResultadoTransaccion;
                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    return vResultadoTransaccion;
                }
            });
        }
    }
}