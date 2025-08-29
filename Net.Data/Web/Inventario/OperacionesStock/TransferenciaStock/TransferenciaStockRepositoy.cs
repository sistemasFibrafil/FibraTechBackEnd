using System;
using SAPbobsCOM;
using System.Linq;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
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
    public class TransferenciaStockRepositoy : RepositoryBase<TransferenciaStockEntity>, ITransferenciaStockRepositoy
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
        const string SP_GET_BY_ID = DB_ESQUEMA + "INV_GetTransferenciaStockById";
        const string SP_GET_LIST_DETALLE_BY_ID = DB_ESQUEMA + "INV_GetTransferenciaStockDetalleById";
        const string SP_GET_LIST_FILTRO = DB_ESQUEMA + "INV_GetListTransferenciaStockByFiltro";

        const string SP_SET_CREATE = DB_ESQUEMA + "INV_SetTransferenciaStockCreate";
        const string SP_SET_DETALLE_CREATE = DB_ESQUEMA + "INV_SetTransferenciaStockDetalleCreate";
        const string SP_SET_DATOS_SAP_UPDATE = DB_ESQUEMA + "INV_SetTransferenciaStockDatosSapUpdate";
        const string SP_SET_DETALLE_DATOS_SAP_UPDATE = DB_ESQUEMA + "INV_SetTransferenciaStockDetalleDatosSapUpdate";
        const string SP_SET_LECTURA_UPDATE = DB_ESQUEMA + "INV_SetLecturaUpdate";

        const string SP_SET_UPDATE = DB_ESQUEMA + "INV_SetTransferenciaStockUpdate";



        public TransferenciaStockRepositoy(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = GetType().Name;
            _connectionSap = new ConnectionSap();
            _cnnDiApiSap = Utilidades.GetConDiApiSap(configuration, "EntornoConnectionDiApiSap:Entorno");
        }


        public async Task<ResultadoTransaccionEntity<TransferenciaStockEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<TransferenciaStockEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<TransferenciaStockEntity>();

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
                            response = (List<TransferenciaStockEntity>)context.ConvertTo<TransferenciaStockEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<TransferenciaStockEntity>> GetById(int id)
        {
            var response = new TransferenciaStockEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<TransferenciaStockEntity>();

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
                            response = context.Convert<TransferenciaStockEntity>(reader);
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_DETALLE_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response.Linea = (List<TransferenciaStockDetalleEntity>)context.ConvertTo<TransferenciaStockDetalleEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<TransferenciaStockEntity>> SetCreate(TransferenciaStockEntity value)
        {
            var responde = new TransferenciaStockEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<TransferenciaStockEntity>();

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

                        StockTransfer documentIns = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oStockTransfer);
                        StockTransfer documentQry = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oStockTransfer);


                        #region <<< CREACIÓN DE TRANSFERENCIA DE STOCK >>>
                        // Creación de la transferencia de stock en la base de datos LOCAL
                        using (SqlCommand cmd = new SqlCommand(SP_SET_CREATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            // CABECERA
                            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@TipDocumento", value.TipDocumento));
                            cmd.Parameters.Add(new SqlParameter("@SerDocumento", value.SerDocumento));
                            cmd.Parameters.Add(new SqlParameter("@DocDate", value.DocDate));
                            cmd.Parameters.Add(new SqlParameter("@DocDueDate", value.DocDueDate));
                            cmd.Parameters.Add(new SqlParameter("@TaxDate", value.TaxDate));
                            // CLIENTE
                            cmd.Parameters.Add(new SqlParameter("@CardCode", value.CardCode));
                            cmd.Parameters.Add(new SqlParameter("@CardName", value.CardName));
                            cmd.Parameters.Add(new SqlParameter("@CntctCode", value.CntctCode));
                            cmd.Parameters.Add(new SqlParameter("@Address", value.Address));
                            // ALMACÉN
                            cmd.Parameters.Add(new SqlParameter("@Filler", value.Filler));
                            cmd.Parameters.Add(new SqlParameter("@ToWhsCode", value.ToWhsCode));
                            // TRANSPORTISTA
                            cmd.Parameters.Add(new SqlParameter("@CodTipTransporte", value.CodTipTransporte));
                            cmd.Parameters.Add(new SqlParameter("@CodTipDocTransportista", value.CodTipDocTransportista));
                            cmd.Parameters.Add(new SqlParameter("@NumTipoDocTransportista", value.NumTipoDocTransportista));
                            cmd.Parameters.Add(new SqlParameter("@NomTransportista", value.NomTransportista));
                            cmd.Parameters.Add(new SqlParameter("@NumPlaVehTransportista", value.NumPlaVehTransportista));
                            // CONDUCTOR
                            cmd.Parameters.Add(new SqlParameter("@CodTipDocConductor", value.CodTipDocConductor));
                            cmd.Parameters.Add(new SqlParameter("@NumTipoDocConductor", value.NumTipoDocConductor));
                            cmd.Parameters.Add(new SqlParameter("@NomConductor", value.NomConductor));
                            cmd.Parameters.Add(new SqlParameter("@ApeConductor", value.ApeConductor));
                            cmd.Parameters.Add(new SqlParameter("@NomComConductor", value.NomComConductor));
                            cmd.Parameters.Add(new SqlParameter("@NumLicConductor", value.NumLicConductor));
                            // OTROS
                            cmd.Parameters.Add(new SqlParameter("@CodTipTraslado", value.CodTipTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodMotTraslado", value.CodMotTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodTipSalida", value.CodTipSalida));
                            //PIE
                            cmd.Parameters.Add(new SqlParameter("@SlpCode", value.SlpCode));
                            cmd.Parameters.Add(new SqlParameter("@NumBulto", value.NumBulto));
                            cmd.Parameters.Add(new SqlParameter("@TotKilo", value.TotKilo));
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


                        #region <<< REGISTRAMOS EL DETALLE DE LA TRANSFERENCIA DE SCTOCK >>>
                        // De la LECTURA se suma y se agrupa
                        var transferenciaDetalle = value.Linea
                        .GroupBy
                        (p => new
                        {
                            p.Id,
                            p.Line,
                            p.IdBase,
                            p.LineBase,
                            p.BaseType,
                            p.BaseEntry,
                            p.BaseLine,
                            p.Read,
                            p.ItemCode,
                            p.Dscription,
                            p.FromWhsCod,
                            p.WhsCode,
                            p.CodTipOperacion,
                            p.UnitMsr,
                            p.IdUsuarioCreate
                        })
                        .Select
                        (g => new TransferenciaStockDetalleEntity
                        {
                            Id = g.Key.Id,
                            Line = g.Key.Line,
                            IdBase = g.Key.IdBase,
                            LineBase = g.Key.LineBase,
                            BaseType = g.Key.BaseType,
                            BaseEntry = g.Key.BaseEntry,
                            BaseLine = g.Key.BaseLine,
                            Read = g.Key.Read,
                            ItemCode = g.Key.ItemCode,
                            Dscription = g.Key.Dscription,
                            FromWhsCod = g.Key.FromWhsCod,
                            WhsCode = g.Key.WhsCode,
                            CodTipOperacion = g.Key.CodTipOperacion,
                            UnitMsr = g.Key.UnitMsr,
                            Quantity = g.Sum(p => p.Quantity),
                            OpenQty = g.Sum(p => p.OpenQty),
                            IdUsuarioCreate = g.Key.IdUsuarioCreate
                        })
                        .OrderBy(x => x.BaseEntry)
                        .ThenBy(x => x.BaseLine)
                        .ToList();


                        using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_CREATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;

                            foreach (var linea in transferenciaDetalle)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", linea.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", SqlDbType.Int)).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(new SqlParameter("@IdBase", linea.IdBase));
                                cmd.Parameters.Add(new SqlParameter("@LineBase", linea.LineBase));
                                cmd.Parameters.Add(new SqlParameter("@BaseType", linea.BaseType));
                                cmd.Parameters.Add(new SqlParameter("@BaseEntry", linea.BaseEntry));
                                cmd.Parameters.Add(new SqlParameter("@BaseLine", linea.BaseLine));
                                cmd.Parameters.Add(new SqlParameter("@Read", linea.Read));
                                cmd.Parameters.Add(new SqlParameter("@ItemCode", linea.ItemCode));
                                cmd.Parameters.Add(new SqlParameter("@Dscription", linea.Dscription));
                                cmd.Parameters.Add(new SqlParameter("@FromWhsCod", linea.FromWhsCod));
                                cmd.Parameters.Add(new SqlParameter("@WhsCode", linea.WhsCode));
                                cmd.Parameters.Add(new SqlParameter("@CodTipOperacion", linea.CodTipOperacion));
                                cmd.Parameters.Add(new SqlParameter("@UnitMsr", linea.UnitMsr));
                                cmd.Parameters.Add(new SqlParameter("@Quantity", linea.Quantity));
                                cmd.Parameters.Add(new SqlParameter("@OpenQty", linea.OpenQty));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", linea.IdUsuarioCreate));

                                await cmd.ExecuteNonQueryAsync();

                                linea.Line = (int)cmd.Parameters["@Line"].Value;
                            }
                        }
                        #endregion

                        #endregion


                        #region <<< SAP >>>

                        #region <<< CREACIÓN DE TRANSFERENCIA DE STOCK EN SAP >>>
                        // Creacion de la transferencia de stock en la base de datos SAP B1
                        // ===========================================================================================
                        // CABECERA
                        // ===========================================================================================
                        documentIns.DocDate = value.DocDate;
                        documentIns.DueDate = value.DocDueDate;
                        documentIns.TaxDate = value.TaxDate;
                        documentIns.DocObjectCode = BoObjectTypes.oStockTransfer;
                        // ===========================================================================================
                        // SOCIO DE NEGOCIO
                        // ===========================================================================================
                        documentIns.CardCode = value.CardCode;
                        documentIns.CardName = value.CardName;
                        documentIns.ContactPerson = value.CntctCode;
                        documentIns.Address = value.Address;
                        // ===========================================================================================
                        // SUNAT
                        // ===========================================================================================
                        if (value.TipDocumento != "") documentIns.UserFields.Fields.Item("U_BPP_MDTD").Value = value.TipDocumento;
                        if (value.SerDocumento != "") documentIns.UserFields.Fields.Item("U_BPP_MDSD").Value = value.SerDocumento;
                        if (value.NumDocumento != "") documentIns.UserFields.Fields.Item("U_BPP_MDCD").Value = value.NumDocumento;
                        // ALMACEN
                        // ===========================================================================================
                        documentIns.FromWarehouse = value.Filler;
                        documentIns.ToWarehouse = value.ToWhsCode;
                        // ===========================================================================================
                        // TRANSPORTISTA
                        // ===========================================================================================
                        if (value.CodTipTransporte != "") documentIns.UserFields.Fields.Item("U_FIB_TIP_TRANS").Value = value.CodTipTransporte;
                        if (value.CodTipDocTransportista != "") documentIns.UserFields.Fields.Item("U_FIB_TIPDOC_TRA").Value = value.CodTipDocTransportista;
                        if (value.NumTipoDocTransportista != "") documentIns.UserFields.Fields.Item("U_BPP_MDRT").Value = value.NumTipoDocTransportista;
                        if (value.NomTransportista != "") documentIns.UserFields.Fields.Item("U_BPP_MDNT").Value = value.NomTransportista;
                        if (value.NumPlaVehTransportista != "") documentIns.UserFields.Fields.Item("U_BPP_MDVC").Value = value.NumPlaVehTransportista;
                        // ===========================================================================================
                        // OTROS
                        // ===========================================================================================
                        if (value.CodTipDocConductor != "") documentIns.UserFields.Fields.Item("U_FIB_TIPDOC_COND").Value = value.CodTipDocConductor;
                        if (value.NumTipoDocConductor != "") documentIns.UserFields.Fields.Item("U_FIB_NUMDOC_COD").Value = value.NumTipoDocConductor;
                        if (value.NomConductor != "") documentIns.UserFields.Fields.Item("U_FIB_NOM_COND").Value = value.NomConductor;
                        if (value.ApeConductor != "") documentIns.UserFields.Fields.Item("U_FIB_APE_COND").Value = value.ApeConductor;
                        if (value.NomComConductor != "") documentIns.UserFields.Fields.Item("U_BPP_MDFN").Value = value.NomComConductor;
                        if (value.NumLicConductor != "") documentIns.UserFields.Fields.Item("U_BPP_MDFC").Value = value.NumLicConductor;
                        // ===========================================================================================
                        // OTROS
                        // ===========================================================================================
                        if (value.CodTipTraslado != "") documentIns.UserFields.Fields.Item("U_FIB_TIP_TRAS").Value = value.CodTipTraslado;
                        if (value.CodMotTraslado != "") documentIns.UserFields.Fields.Item("U_BPP_MDMT").Value = value.CodMotTraslado;
                        if (value.CodTipSalida != "") documentIns.UserFields.Fields.Item("U_BPP_MDTS").Value = value.CodTipSalida;
                        // ===========================================================================================
                        // PIE
                        // ===========================================================================================
                        documentIns.SalesPersonCode = value.SlpCode;
                        if (value.NumBulto != 0) documentIns.UserFields.Fields.Item("U_FIB_NBULTOS").Value = value.NumBulto.ToString();
                        if (value.TotKilo != 0) documentIns.UserFields.Fields.Item("U_FIB_KG").Value = value.TotKilo.ToString();
                        documentIns.JournalMemo = value.JrnlMemo;
                        documentIns.Comments = value.Comments;

                        // ===========================================================================================
                        // DETALLE
                        // ===========================================================================================
                        foreach (var linea in transferenciaDetalle)
                        {
                            documentIns.Lines.BaseType = InvBaseDocTypeEnum.InventoryTransferRequest;
                            documentIns.Lines.BaseEntry = linea.BaseEntry;
                            documentIns.Lines.BaseLine = linea.BaseLine;
                            documentIns.Lines.ItemCode = linea.ItemCode;
                            documentIns.Lines.ItemDescription = linea.Dscription;
                            documentIns.Lines.FromWarehouseCode = linea.FromWhsCod;
                            documentIns.Lines.WarehouseCode = linea.WhsCode;
                            documentIns.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = linea.CodTipOperacion;
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


                        #region <<< ACTUALIZAR LA TRANSFERENCIA DE STOCK CON DATOS OBTENIDOS DE SAP >>>
                        using (SqlCommand cmd = new SqlCommand(SP_SET_DATOS_SAP_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add(new SqlParameter("@Id", value.Id));
                            cmd.Parameters.Add(new SqlParameter("@DocEntry", documentQry.DocEntry));
                            cmd.Parameters.Add(new SqlParameter("@DocNum", documentQry.DocNum));
                            cmd.Parameters.Add(new SqlParameter("@TipDocumento", documentQry.UserFields.Fields.Item("U_BPP_MDTD").Value));
                            cmd.Parameters.Add(new SqlParameter("@SerDocumento", documentQry.UserFields.Fields.Item("U_BPP_MDSD").Value));
                            cmd.Parameters.Add(new SqlParameter("@NumDocumento", documentQry.UserFields.Fields.Item("U_BPP_MDCD").Value));

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


                        #region <<< ACTUALIZAMOS EL ESTADO DE LA LECTURA Y LA RELACION ENTRE LA SOLICITUD Y LA TRANSFERENCIA DE STOCK >>>
                        using (SqlCommand cmd = new SqlCommand(SP_SET_LECTURA_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;

                            foreach (var linea in value.Linea.Where(x => x.Read == "Y"))
                            {
                                for (int i = 0; i < documentQry.Lines.Count; i++)
                                {
                                    // SE OBTIENE POR LINEA PARA ACTUALIZAR EL DB LOCAL
                                    documentQry.Lines.SetCurrentLine(i);

                                    if (linea.BaseEntry == documentQry.Lines.BaseEntry && linea.BaseLine == documentQry.Lines.BaseLine)
                                    {
                                        cmd.Parameters.Clear();
                                        cmd.Parameters.Add(new SqlParameter("@IdLectura", linea.IdLectura));
                                        cmd.Parameters.Add(new SqlParameter("@TargetType", value.ObjType));
                                        cmd.Parameters.Add(new SqlParameter("@TrgetEntry", documentQry.Lines.DocEntry));
                                        cmd.Parameters.Add(new SqlParameter("@TrgetLine", documentQry.Lines.LineNum));
                                        await cmd.ExecuteNonQueryAsync();
                                    }
                                }
                            }
                        }
                        #endregion


                        // SI NO SALE ERROR EN SAP, SE CONFIRMA CREACION EN LA BASE LOCAL
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
        public async Task<ResultadoTransaccionEntity<TransferenciaStockEntity>> SetUpdate(TransferenciaStockEntity value)
        {
            var responde = new TransferenciaStockEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<TransferenciaStockEntity>();

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
                        
                        StockTransfer documentUpd = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oStockTransfer);
                        StockTransfer documentQry = RepositoryBaseSap.oCompany.GetBusinessObject(BoObjectTypes.oStockTransfer);

                       
                        #region <<< CREACIÓN DE TRANSFERENCIA DE STOCK >>>
                        // Creación de la solicitud de traslado en la base de datos LOCAL
                        using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            // CABECERA
                            cmd.Parameters.Add(new SqlParameter("@Id", value.Id));
                            cmd.Parameters.Add(new SqlParameter("@DocDueDate", value.DocDueDate));
                            // CLIENTE
                            // ALMACÉN
                            // TRANSPORTISTA
                            cmd.Parameters.Add(new SqlParameter("@CodTipTransporte", value.CodTipTransporte));
                            cmd.Parameters.Add(new SqlParameter("@CodTipDocTransportista", value.CodTipDocTransportista));
                            cmd.Parameters.Add(new SqlParameter("@NumTipoDocTransportista", value.NumTipoDocTransportista));
                            cmd.Parameters.Add(new SqlParameter("@NomTransportista", value.NomTransportista));
                            cmd.Parameters.Add(new SqlParameter("@NumPlaVehTransportista", value.NumPlaVehTransportista));
                            // CONDUCTOR
                            cmd.Parameters.Add(new SqlParameter("@CodTipDocConductor", value.CodTipDocConductor));
                            cmd.Parameters.Add(new SqlParameter("@NumTipoDocConductor", value.NumTipoDocConductor));
                            cmd.Parameters.Add(new SqlParameter("@NomConductor", value.NomConductor));
                            cmd.Parameters.Add(new SqlParameter("@ApeConductor", value.ApeConductor));
                            cmd.Parameters.Add(new SqlParameter("@NomComConductor", value.NomComConductor));
                            cmd.Parameters.Add(new SqlParameter("@NumLicConductor", value.NumLicConductor));
                            // OTROS
                            cmd.Parameters.Add(new SqlParameter("@CodTipTraslado", value.CodTipTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodMotTraslado", value.CodMotTraslado));
                            cmd.Parameters.Add(new SqlParameter("@CodTipSalida", value.CodTipSalida));
                            //PIE
                            cmd.Parameters.Add(new SqlParameter("@SlpCode", value.SlpCode));
                            cmd.Parameters.Add(new SqlParameter("@NumBulto", value.NumBulto));
                            cmd.Parameters.Add(new SqlParameter("@TotKilo", value.TotKilo));
                            cmd.Parameters.Add(new SqlParameter("@JrnlMemo", value.JrnlMemo));
                            cmd.Parameters.Add(new SqlParameter("@Comments", value.Comments));
                            //USUARIO
                            //USUARIO
                            cmd.Parameters.Add(new SqlParameter("@IdUsuarioUpdate", value.IdUsuarioUpdate));

                            await cmd.ExecuteNonQueryAsync();
                        }
                        #endregion


                        #region <<< SAP >>>
                        
                        #region <<< CREACIÓN DE TRANSFERENCIA DE STOCK EN SAP >>>
                        if (documentUpd.GetByKey(value.DocEntry))
                        {
                            // Creacion de la solicitud de traslado en la base de datos SAP B1
                            // ===========================================================================================
                            // CABECERA
                            // ===========================================================================================
                            documentUpd.DueDate = value.DocDueDate;
                            documentUpd.DocObjectCode = BoObjectTypes.oInventoryTransferRequest;
                            // ===========================================================================================
                            // SOCIO DE NEGOCIO
                            // ===========================================================================================
                            // ===========================================================================================
                            // SUNAT
                            // ===========================================================================================
                            // ALMACEN
                            // ===========================================================================================
                            // ===========================================================================================
                            // TRANSPORTISTA
                            // ===========================================================================================
                            if (value.CodTipTransporte != "") documentUpd.UserFields.Fields.Item("U_FIB_TIP_TRANS").Value = value.CodTipTransporte;
                            if (value.CodTipDocTransportista != "") documentUpd.UserFields.Fields.Item("U_FIB_TIPDOC_TRA").Value = value.CodTipDocTransportista;
                            if (value.NumTipoDocTransportista != "") documentUpd.UserFields.Fields.Item("U_BPP_MDRT").Value = value.NumTipoDocTransportista;
                            if (value.NomTransportista != "") documentUpd.UserFields.Fields.Item("U_BPP_MDNT").Value = value.NomTransportista;
                            if (value.NumPlaVehTransportista != "") documentUpd.UserFields.Fields.Item("U_BPP_MDVC").Value = value.NumPlaVehTransportista;
                            // ===========================================================================================
                            // OTROS
                            // ===========================================================================================
                            if (value.CodTipDocConductor != "") documentUpd.UserFields.Fields.Item("U_FIB_TIPDOC_COND").Value = value.CodTipDocConductor;
                            if (value.NumTipoDocConductor != "") documentUpd.UserFields.Fields.Item("U_FIB_NUMDOC_COD").Value = value.NumTipoDocConductor;
                            if (value.NomConductor != "") documentUpd.UserFields.Fields.Item("U_FIB_NOM_COND").Value = value.NomConductor;
                            if (value.ApeConductor != "") documentUpd.UserFields.Fields.Item("U_FIB_APE_COND").Value = value.ApeConductor;
                            if (value.NomComConductor != "") documentUpd.UserFields.Fields.Item("U_BPP_MDFN").Value = value.NomComConductor;
                            if (value.NumLicConductor != "") documentUpd.UserFields.Fields.Item("U_BPP_MDFC").Value = value.NumLicConductor;
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
                            if (value.NumBulto != 0) documentUpd.UserFields.Fields.Item("U_FIB_NBULTOS").Value = value.NumBulto.ToString();
                            if (value.TotKilo != 0) documentUpd.UserFields.Fields.Item("U_FIB_KG").Value = value.TotKilo.ToString();
                            documentUpd.JournalMemo = value.JrnlMemo;
                            documentUpd.Comments = value.Comments;

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
                            resultTransaccion.ResultadoDescripcion = "No existe la transferencia de stock en SAP Business One.";
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
    }
}
