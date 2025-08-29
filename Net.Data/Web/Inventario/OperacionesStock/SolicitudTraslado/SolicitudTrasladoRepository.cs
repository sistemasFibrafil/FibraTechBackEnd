using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Business.Entities.Web;
using Net.Connection;
using Net.CrossCotting;
using SAPbobsCOM;
namespace Net.Data.Web
{
    public class SolicitudTrasladoRepository : RepositoryBase<SolicitudTrasladoEntity>, ISolicitudTrasladoRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IConfiguration _configuration;
        private readonly IConnectionSap _connectionSap;
        private readonly ConnectionSapEntity _cnnDiApiSap;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_BY_ID = DB_ESQUEMA + "INV_GetSolicitudTrasladoById";
        const string SP_GET_LIST_DETALLE_BY_ID = DB_ESQUEMA + "INV_GetSolicitudTrasladoDetalleById";
        const string SP_GET_LIST_FILTRO = DB_ESQUEMA + "INV_GetListSolicitudTrasladoByFiltro";
        const string SP_GET_DETALLE_BY_ID_LINE = DB_ESQUEMA + "INV_GetSolicitudTrasladoDetalleByIdAndLine";

        const string SP_GET_TO_TRANSFERENCIA = DB_ESQUEMA + "INV_GetSolicitudTrasladoToTransferencia";
        const string SP_GET_LIST_DETALLE_TO_TRANSFERENCIA = DB_ESQUEMA + "INV_GetSolicitudTrasladoDetalleToTransferencia";

        const string SP_SET_CREATE = DB_ESQUEMA + "INV_SetSolicitudTrasladoCreate";
        const string SP_SET_DETALLE_CREATE = DB_ESQUEMA + "INV_SetSolicitudTrasladoDetalleCreate";
        const string SP_SET_DATOS_SAP_UPDATE = DB_ESQUEMA + "INV_SetSolicitudTrasladoDatosSapUpdate";
        const string SP_SET_DETALLE_DATOS_SAP_UPDATE = DB_ESQUEMA + "INV_SetSolicitudTrasladoDetalleDatosSapUpdate";

        const string SP_SET_UPDATE = DB_ESQUEMA + "INV_SetSolicitudTrasladoUpdate";
        const string SP_SET_DETALLE_UPDATE = DB_ESQUEMA + "INV_SetSolicitudTrasladoDetalleUpdate";

        const string SP_SET_CLOSE = DB_ESQUEMA + "INV_SetSolicitudTrasladoClose";
        const string SP_SET_DETALLE_DELETE = DB_ESQUEMA + "INV_SetSolicitudTrasladoDetalleDelete";

        const string SP_GET_EXISTE_ALAMCEN = DB_ESQUEMA + "GES_GetAlmaceExisteByCodeSede";


        public SolicitudTrasladoRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = GetType().Name;
            _connectionSap = new ConnectionSap();
            _cnnDiApiSap = Utilidades.GetConDiApiSap(configuration, "EntornoConnectionDiApiSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<SolicitudTrasladoEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@Estado", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@Numero", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SolicitudTrasladoEntity>)context.ConvertTo<SolicitudTrasladoEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
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
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> GetById(int id)
        {
            var response = new SolicitudTrasladoEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<SolicitudTrasladoEntity>(reader);
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_DETALLE_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response.Linea = (List<SolicitudTrasladoDetalleEntity>)context.ConvertTo<SolicitudTrasladoDetalleEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoDetalleEntity>> GetListById(int id)
        {
            var response = new List<SolicitudTrasladoDetalleEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoDetalleEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_DETALLE_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SolicitudTrasladoDetalleEntity>)context.ConvertTo<SolicitudTrasladoDetalleEntity>(reader);
                        }
                    }

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultTransaccion.dataList = response;
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
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoToTransferenciaEntity>> GetSolicitudTrasladoToTransferencia(int id)
        {
            var response = new SolicitudTrasladoToTransferenciaEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoToTransferenciaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_TO_TRANSFERENCIA, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<SolicitudTrasladoToTransferenciaEntity>(reader);
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_DETALLE_TO_TRANSFERENCIA, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response.Linea = (List<SolicitudTrasladoDetalleToTransferenciaEntity>)context.ConvertTo<SolicitudTrasladoDetalleToTransferenciaEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoDetalleEntity>> GetSolicitudTrasladoDetalleByIdAndLine(FilterRequestEntity value)
        {
            var response = new SolicitudTrasladoDetalleEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoDetalleEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLE_BY_ID_LINE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", value.Id1));
                        cmd.Parameters.Add(new SqlParameter("@Line", value.Id2));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<SolicitudTrasladoDetalleEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetCreate(SolicitudTrasladoEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            TimeSpan timeout = TimeSpan.FromSeconds(1800);

            using (CommittableTransaction transaction = new CommittableTransaction(timeout))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                    {
                        await conn.OpenAsync();
                        conn.EnlistTransaction(transaction);

                        // Se abre la conexión SAP con DI API
                        _connectionSap.ConnectToCompany(_cnnDiApiSap);

                        StockTransfer documentIns = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);
                        StockTransfer documentQry = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);
                        

                        #region <<< CREACIÓN DE SOLICITUD DE TRASLADO >>>
                        // Creación de la solicitud de traslado en la base de datos LOCAL
                        using (SqlCommand cmd = new SqlCommand(SP_SET_CREATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            // CABECERA
                            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@DocDate", value.DocDate));
                            cmd.Parameters.Add(new SqlParameter("@DocDueDate", value.DocDueDate));
                            cmd.Parameters.Add(new SqlParameter("@TaxDate", value.TaxDate));
                            cmd.Parameters.Add(new SqlParameter("@Read", value.Read));
                            // CLIENTE
                            cmd.Parameters.Add(new SqlParameter("@CardCode", value.CardCode));
                            cmd.Parameters.Add(new SqlParameter("@CardName", value.CardName));
                            cmd.Parameters.Add(new SqlParameter("@CntctCode", value.CntctCode));
                            cmd.Parameters.Add(new SqlParameter("@Address", value.Address));
                            // ALMACÉN
                            cmd.Parameters.Add(new SqlParameter("@Filler", value.Filler));
                            cmd.Parameters.Add(new SqlParameter("@ToWhsCode", value.ToWhsCode));
                            // Otros
                            cmd.Parameters.Add(new SqlParameter("@CodTipTraslado", value.CodTipTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodMotTraslado", value.CodMotTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodTipSalida", value.CodTipSalida));
                            //PIE
                            cmd.Parameters.Add(new SqlParameter("@SlpCode", value.SlpCode));
                            cmd.Parameters.Add(new SqlParameter("@JrnlMemo", value.JrnlMemo));
                            cmd.Parameters.Add(new SqlParameter("@Comments", value.Comments));
                            //USUARIO
                            cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", value.IdUsuarioCreate));

                            await cmd.ExecuteNonQueryAsync();

                            value.Id = (int)cmd.Parameters["@Id"].Value;
                        }


                        #region <<< ACTUALIZAMOS EL ID >>>
                        foreach (var linea in value.Linea)
                        {
                            linea.Id = value.Id;
                        }
                        #endregion


                        using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_CREATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;

                            foreach (var linea in value.Linea)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", linea.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", SqlDbType.Int)).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(new SqlParameter("@ItemCode", linea.ItemCode));
                                cmd.Parameters.Add(new SqlParameter("@Dscription", linea.Dscription));
                                cmd.Parameters.Add(new SqlParameter("@FromWhsCod", linea.FromWhsCod));
                                cmd.Parameters.Add(new SqlParameter("@WhsCode", linea.WhsCode));
                                cmd.Parameters.Add(new SqlParameter("@UnitMsr", linea.UnitMsr));
                                cmd.Parameters.Add(new SqlParameter("@Quantity", linea.Quantity));
                                cmd.Parameters.Add(new SqlParameter("@OpenQtyRding", linea.OpenQtyRding));
                                cmd.Parameters.Add(new SqlParameter("@OpenQty", linea.OpenQty));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", linea.IdUsuarioCreate));

                                await cmd.ExecuteNonQueryAsync();

                                linea.Line = (int)cmd.Parameters["@Line"].Value;
                            }
                        }
                        #endregion


                        #region <<< SAP >>>

                        #region <<< CREACIÓN DE SOLICITUD DE TRASLADO EN SAP >>>
                        // Creacion de la solicitud de traslado en la base de datos SAP B1
                        // ===========================================================================================
                        // CABECERA
                        // ===========================================================================================
                        documentIns.DocDate = value.DocDate;
                        documentIns.DueDate = value.DocDueDate;
                        documentIns.TaxDate = value.TaxDate;
                        documentIns.DocObjectCode = BoObjectTypes.oInventoryTransferRequest;
                        // ===========================================================================================
                        // SOCIO DE NEGOCIO
                        // ===========================================================================================
                        documentIns.CardCode = value.CardCode;
                        documentIns.CardName = value.CardName;
                        documentIns.ContactPerson = value.CntctCode;
                        documentIns.Address = value.Address;
                        // ===========================================================================================
                        // OTROS
                        // ===========================================================================================
                        documentIns.FromWarehouse = value.Filler;
                        documentIns.ToWarehouse = value.ToWhsCode;
                        if (value.CodTipTraslado != "") documentIns.UserFields.Fields.Item("U_FIB_TIP_TRAS").Value = value.CodTipTraslado;
                        if (value.CodMotTraslado != "") documentIns.UserFields.Fields.Item("U_BPP_MDMT").Value = value.CodMotTraslado;
                        if (value.CodTipSalida != "") documentIns.UserFields.Fields.Item("U_BPP_MDTS").Value = value.CodTipSalida;

                        // ===========================================================================================
                        // PIE
                        // ===========================================================================================
                        documentIns.SalesPersonCode = value.SlpCode;
                        documentIns.JournalMemo = value.JrnlMemo;
                        documentIns.Comments = value.Comments;

                        // ===========================================================================================
                        // DETALLE
                        // ===========================================================================================
                        foreach (var linea in value.Linea)
                        {
                            documentIns.Lines.ItemCode = linea.ItemCode;
                            documentIns.Lines.ItemDescription = linea.Dscription;
                            documentIns.Lines.FromWarehouseCode = linea.FromWhsCod;
                            documentIns.Lines.WarehouseCode = linea.WhsCode;
                            documentIns.Lines.Quantity = (double)linea.Quantity;
                            // ===========================================================================================
                            // Relación entre la OV de la base local y SAP
                            // ===========================================================================================
                            documentIns.Lines.UserFields.Fields.Item("U_FIB_BASETYPE").Value = -1;
                            documentIns.Lines.UserFields.Fields.Item("U_FIB_BASEENTRY").Value = linea.Id;
                            documentIns.Lines.UserFields.Fields.Item("U_FIB_BASELINENUM").Value = linea.Line;
                            // ===========================================================================================
                            // Relación entre la OV de la base local y SAP
                            // ===========================================================================================
                            documentIns.Lines.Add();
                        }

                        var reg = documentIns.Add();

                        if (reg != 0)
                        {
                            // SE CANCELA LA CREACIÓN
                            transaction.Rollback();

                            if (RepositoryBaseSap.oCompany is not null)
                            {
                                if (RepositoryBaseSap.oCompany.Connected)
                                {
                                    _connectionSap.DisConnectToCompany();
                                }
                            }
                            resultTransaccion.IdRegistro = -1;
                            resultTransaccion.ResultadoCodigo = -1;
                            resultTransaccion.ResultadoDescripcion = RepositoryBaseSap.oCompany.GetLastErrorDescription();
                            return resultTransaccion;
                        }
                        else
                        {
                            var docEntryTmp = RepositoryBaseSap.oCompany.GetNewObjectKey();
                            value.DocEntry = docEntryTmp == null ? 0 : int.Parse(docEntryTmp);
                            documentQry.GetByKey(value.DocEntry);
                        }
                        #endregion


                        #endregion


                        #region <<< ACTUALIZAR LA SOLICITUD DE TRASLADO CON DATOS OBTENIDOS DE SAP >>>
                        using (SqlCommand cmd = new SqlCommand(SP_SET_DATOS_SAP_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add(new SqlParameter("@Id", value.Id));
                            cmd.Parameters.Add(new SqlParameter("@DocEntry", documentQry.DocEntry));
                            cmd.Parameters.Add(new SqlParameter("@DocNum", documentQry.DocNum));

                            await cmd.ExecuteNonQueryAsync();
                        }

                        using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_DATOS_SAP_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;

                            for (int i = 0; i < documentQry.Lines.Count; i++)
                            {
                                // SE OBTIENE POR LINEA PARA ACTUALIZAR EL DB LOCAL
                                documentQry.Lines.SetCurrentLine(i);

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", documentQry.Lines.UserFields.Fields.Item("U_FIB_BASEENTRY").Value));
                                cmd.Parameters.Add(new SqlParameter("@Line", documentQry.Lines.UserFields.Fields.Item("U_FIB_BASELINENUM").Value));
                                cmd.Parameters.Add(new SqlParameter("@DocEntry", documentQry.Lines.DocEntry));
                                cmd.Parameters.Add(new SqlParameter("@LineNum", documentQry.Lines.LineNum));
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        #endregion


                        // SI NO SALE ERROR EN SAP, SE CONFIRMA LA CREACIÓN EN LA BASE LOCAL
                        transaction.Commit();

                        if (RepositoryBaseSap.oCompany is not null)
                        {
                            if (RepositoryBaseSap.oCompany.Connected)
                            {
                                _connectionSap.DisConnectToCompany();
                            }
                        }

                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito ..!";
                        return resultTransaccion;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    
                    if (RepositoryBaseSap.oCompany is not null)
                    {
                        if (RepositoryBaseSap.oCompany.Connected)
                        {
                            _connectionSap.DisConnectToCompany();
                        }
                    }

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    return resultTransaccion;
                }
            }
        }
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetUpdate(SolicitudTrasladoEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            TimeSpan timeout = TimeSpan.FromSeconds(1800);

            using (CommittableTransaction transaction = new CommittableTransaction(timeout))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                    {
                        await conn.OpenAsync();
                        conn.EnlistTransaction(transaction);

                        _connectionSap.ConnectToCompany(_cnnDiApiSap);
                        
                        StockTransfer documentUpd = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);
                        StockTransfer documentQry = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);

                        
                        #region <<< ACTUALIZACIÓN DE SOLICITUD DE TRASLADO >>>
                        // Creación de la solicitud de traslado en la base de datos LOCAL
                        using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            // CABECERA
                            cmd.Parameters.Add(new SqlParameter("@Id", value.Id));
                            cmd.Parameters.Add(new SqlParameter("@DocDate", value.DocDate));
                            cmd.Parameters.Add(new SqlParameter("@DocDueDate", value.DocDueDate));
                            cmd.Parameters.Add(new SqlParameter("@TaxDate", value.TaxDate));
                            cmd.Parameters.Add(new SqlParameter("@Read", value.Read));
                            // SOCIO DE NEGOCIOS
                            // ALMACÉN
                            cmd.Parameters.Add(new SqlParameter("@Filler", value.Filler));
                            cmd.Parameters.Add(new SqlParameter("@ToWhsCode", value.ToWhsCode));
                            // OTROS
                            cmd.Parameters.Add(new SqlParameter("@CodTipTraslado", value.CodTipTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodMotTraslado", value.CodMotTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodTipSalida", value.CodTipSalida));
                            //PIE
                            cmd.Parameters.Add(new SqlParameter("@SlpCode", value.SlpCode));
                            cmd.Parameters.Add(new SqlParameter("@JrnlMemo", value.JrnlMemo));
                            cmd.Parameters.Add(new SqlParameter("@Comments", value.Comments));
                            //USUARIO
                            cmd.Parameters.Add(new SqlParameter("@IdUsuarioUpdate", value.IdUsuarioUpdate));

                            await cmd.ExecuteNonQueryAsync();
                        }

                        // SE CREA
                        foreach (var linea in value.Linea.Where(x => x.Record == 1))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_CREATE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", linea.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", SqlDbType.Int)).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(new SqlParameter("@ItemCode", linea.ItemCode));
                                cmd.Parameters.Add(new SqlParameter("@Dscription", linea.Dscription));
                                cmd.Parameters.Add(new SqlParameter("@FromWhsCod", linea.FromWhsCod));
                                cmd.Parameters.Add(new SqlParameter("@WhsCode", linea.WhsCode));
                                cmd.Parameters.Add(new SqlParameter("@UnitMsr", linea.UnitMsr));
                                cmd.Parameters.Add(new SqlParameter("@Quantity", linea.Quantity));
                                cmd.Parameters.Add(new SqlParameter("@OpenQtyRding", linea.OpenQtyRding));
                                cmd.Parameters.Add(new SqlParameter("@OpenQty", linea.OpenQty));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", linea.IdUsuarioCreate));

                                await cmd.ExecuteNonQueryAsync();

                                linea.Line = (int)cmd.Parameters["@Line"].Value;
                            }
                        }

                        // SE ACTUALIZA
                        foreach (var linea in value.Linea.Where(x => x.Record == 2 && x.LineStatus == "01"))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_UPDATE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", linea.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", linea.Line));
                                cmd.Parameters.Add(new SqlParameter("@ItemCode", linea.ItemCode));
                                cmd.Parameters.Add(new SqlParameter("@Dscription", linea.Dscription));
                                cmd.Parameters.Add(new SqlParameter("@FromWhsCod", linea.FromWhsCod));
                                cmd.Parameters.Add(new SqlParameter("@WhsCode", linea.WhsCode));
                                cmd.Parameters.Add(new SqlParameter("@UnitMsr", linea.UnitMsr));
                                cmd.Parameters.Add(new SqlParameter("@Quantity", linea.Quantity));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioUpdate", linea.IdUsuarioUpdate));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // SE ELIMINA
                        foreach (var linea in value.Linea.Where(x => x.Record == 3))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_DELETE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", linea.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", linea.Line));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        #endregion


                        #region <<< SAP >>>
                        
                        #region <<< ACTUALIZACIÓN DE SOLICITUD DE TRASLADO EN SAP >>>
                        if (documentUpd.GetByKey(value.DocEntry))
                        {
                            // Creacion de la solicitud de traslado en la base de datos SAP B1
                            // ===========================================================================================
                            // CABECERA
                            // ===========================================================================================
                            documentUpd.DocDate = value.DocDate;
                            documentUpd.DueDate = value.DocDueDate;
                            documentUpd.TaxDate = value.TaxDate;
                            documentUpd.DocObjectCode = BoObjectTypes.oInventoryTransferRequest;
                            // ===========================================================================================
                            // SOCIO DE NEGOCIO
                            // ===========================================================================================

                            // ===========================================================================================
                            // ALMACEN
                            // ===========================================================================================
                            documentUpd.FromWarehouse = value.Filler;
                            documentUpd.ToWarehouse = value.ToWhsCode;
                            // ===========================================================================================
                            // OTROS
                            // ===========================================================================================
                            if (value.CodTipTraslado != "") documentUpd.UserFields.Fields.Item("U_FIB_TIP_TRAS").Value = value.CodTipTraslado;
                            if (value.CodMotTraslado != "") documentUpd.UserFields.Fields.Item("U_BPP_MDMT").Value = value.CodMotTraslado;
                            if (value.CodTipSalida != "") documentUpd.UserFields.Fields.Item("U_BPP_MDTS").Value = value.CodTipSalida;

                            // ===========================================================================================
                            // PIE
                            // ===========================================================================================
                            documentUpd.SalesPersonCode = value.SlpCode;
                            documentUpd.JournalMemo = value.JrnlMemo;
                            documentUpd.Comments = value.Comments;

                            // ===========================================================================================
                            // DETALLE
                            // ===========================================================================================
                            // SE CREA
                            foreach (var linea in value.Linea.Where(x => x.Record == 1))
                            {
                                documentUpd.Lines.Add();
                                documentUpd.Lines.ItemCode = linea.ItemCode;
                                documentUpd.Lines.ItemDescription = linea.Dscription;
                                documentUpd.Lines.FromWarehouseCode = linea.FromWhsCod;
                                documentUpd.Lines.WarehouseCode = linea.WhsCode;
                                documentUpd.Lines.Quantity = (double)linea.Quantity;
                                // ===========================================================================================
                                // Relación entre la OV de la base local y SAP
                                // ===========================================================================================
                                documentUpd.Lines.UserFields.Fields.Item("U_FIB_BASETYPE").Value = -1;
                                documentUpd.Lines.UserFields.Fields.Item("U_FIB_BASEENTRY").Value = linea.Id;
                                documentUpd.Lines.UserFields.Fields.Item("U_FIB_BASELINENUM").Value = linea.Line;
                                // ===========================================================================================
                                // Relación entre la OV de la base local y SAP
                                // ===========================================================================================
                            }

                            // SE ACTUALIZA
                            foreach (var linea in value.Linea.Where(x => x.Record == 2 && x.LineStatus == "01"))
                            {
                                documentUpd.Lines.SetCurrentLine(linea.LineNum);
                                documentUpd.Lines.ItemCode = linea.ItemCode;
                                documentUpd.Lines.ItemDescription = linea.Dscription;
                                documentUpd.Lines.FromWarehouseCode = linea.FromWhsCod;
                                documentUpd.Lines.WarehouseCode = linea.WhsCode;
                                documentUpd.Lines.Quantity = (double)linea.Quantity;
                            }

                            // SE ELEMINA
                            foreach (var linea in value.Linea.Where(x => x.Record == 3))
                            {
                                documentUpd.Lines.SetCurrentLine(linea.LineNum);
                                documentUpd.Lines.Delete();
                            }

                            var reg = documentUpd.Update();

                            if (reg != 0)
                            {
                                transaction.Rollback();
                                
                                if (RepositoryBaseSap.oCompany is not null)
                                {
                                    if (RepositoryBaseSap.oCompany.Connected)
                                    {
                                        _connectionSap.DisConnectToCompany();
                                    }
                                }
                                resultTransaccion.IdRegistro = -1;
                                resultTransaccion.ResultadoCodigo = -1;
                                resultTransaccion.ResultadoDescripcion = RepositoryBaseSap.oCompany.GetLastErrorDescription();
                                return resultTransaccion;
                            }
                            else
                            {
                                documentQry.GetByKey(value.DocEntry);
                            }

                            #region <<< ACTUALIZAR LA SOLICITUD DE TRASLADO CON DATOS OBTENIDOS DE SAP >>>
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_DATOS_SAP_UPDATE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;

                                for (int i = 0; i < documentQry.Lines.Count; i++)
                                {
                                    // SE OBTIENE POR LINEA PARA ACTUALIZAR EL DB LOCAL
                                    documentQry.Lines.SetCurrentLine(i);

                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add(new SqlParameter("@Id", documentQry.Lines.UserFields.Fields.Item("U_FIB_BASEENTRY").Value));
                                    cmd.Parameters.Add(new SqlParameter("@Line", documentQry.Lines.UserFields.Fields.Item("U_FIB_BASELINENUM").Value));
                                    cmd.Parameters.Add(new SqlParameter("@DocEntry", documentQry.Lines.DocEntry));
                                    cmd.Parameters.Add(new SqlParameter("@LineNum", documentQry.Lines.LineNum));
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            transaction.Rollback();
                            
                            if (RepositoryBaseSap.oCompany is not null)
                            {
                                if (RepositoryBaseSap.oCompany.Connected)
                                {
                                    _connectionSap.DisConnectToCompany();
                                }
                            }
                            resultTransaccion.IdRegistro = -1;
                            resultTransaccion.ResultadoCodigo = -1;
                            resultTransaccion.ResultadoDescripcion = "No existe la solicitud en SAP Business One.";
                            return resultTransaccion;
                        }
                        #endregion

                        #endregion

                        // SI NO SALE ERROR EN SAP, SE CONFIRMA LA ACTUALIZACION EN LA BASE LOCAL
                        transaction.Commit();

                        if (RepositoryBaseSap.oCompany is not null)
                        {
                            if (RepositoryBaseSap.oCompany.Connected)
                            {
                                _connectionSap.DisConnectToCompany();
                            }
                        }

                        resultTransaccion.IdRegistro = 0;
                        resultTransaccion.ResultadoCodigo = 0;
                        resultTransaccion.ResultadoDescripcion = "Registro procesado con éxito ..!";
                        return resultTransaccion;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    
                    if (RepositoryBaseSap.oCompany is not null)
                    {
                        if (RepositoryBaseSap.oCompany.Connected)
                        {
                            _connectionSap.DisConnectToCompany();
                        }
                    }

                    resultTransaccion.IdRegistro = -1;
                    resultTransaccion.ResultadoCodigo = -1;
                    resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    return resultTransaccion;
                }
            }
        }
        public async Task<ResultadoTransaccionEntity<SolicitudTrasladoEntity>> SetClose(SolicitudTrasladoEntity value)
        {
            var resultadoTransaccion = new ResultadoTransaccionEntity<SolicitudTrasladoEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            TimeSpan timeout = TimeSpan.FromSeconds(1800);

            using (CommittableTransaction transaction = new CommittableTransaction(timeout))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                    {
                        await conn.OpenAsync();
                        conn.EnlistTransaction(transaction);

                        _connectionSap.ConnectToCompany(_cnnDiApiSap);
                        
                        StockTransfer documentQry = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);

                        
                        #region <<< CERRAR LA SOLICITUD DE TRASLADO >>>
                        // Creación de la solicitud de traslado en la base de datos LOCAL
                        using (SqlCommand cmd = new SqlCommand(SP_SET_CLOSE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            // CABECERA
                            cmd.Parameters.Add(new SqlParameter("@Id", value.Id));
                            cmd.Parameters.Add(new SqlParameter("@IdUsuarioClose", value.IdUsuarioClose));

                            await cmd.ExecuteNonQueryAsync();
                        }
                        #endregion


                        #region <<< SAP >>>
                            
                        #region <<< CERRAR LA SOLICITUD DE TRASLADO EN SAP >>>
                        // Creacion de la solicitud de traslado en la base de datos SAP B1
                        // ===========================================================================================
                        // CABECERA
                        // ===========================================================================================
                        documentQry.GetByKey(value.DocEntry);

                        var reg = documentQry.Close();

                        if (reg != 0)
                        {
                            transaction.Rollback();
                            
                            if (RepositoryBaseSap.oCompany is not null)
                            {
                                if (RepositoryBaseSap.oCompany.Connected)
                                {
                                    _connectionSap.DisConnectToCompany();
                                }
                            }
                            resultadoTransaccion.IdRegistro = -1;
                            resultadoTransaccion.ResultadoCodigo = -1;
                            resultadoTransaccion.ResultadoDescripcion = RepositoryBaseSap.oCompany.GetLastErrorDescription();
                            return resultadoTransaccion;
                        }
                        #endregion

                        #endregion


                        // SI NO SALE ERROR EN SAP, SE CONFIRMA LA ELIMINACION EN LA BASE LOCAL
                        transaction.Commit();

                        if (RepositoryBaseSap.oCompany is not null)
                        {
                            if (RepositoryBaseSap.oCompany.Connected)
                            {
                                _connectionSap.DisConnectToCompany();
                            }
                        }

                        resultadoTransaccion.IdRegistro = 0;
                        resultadoTransaccion.ResultadoCodigo = 0;
                        resultadoTransaccion.ResultadoDescripcion = "Registro procesado con éxito ..!";
                        return resultadoTransaccion;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    
                    if (RepositoryBaseSap.oCompany is not null)
                    {
                        if (RepositoryBaseSap.oCompany.Connected)
                        {
                            _connectionSap.DisConnectToCompany();
                        }
                    }

                    resultadoTransaccion.IdRegistro = -1;
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    return resultadoTransaccion;
                }
            }
        }
    }
}
