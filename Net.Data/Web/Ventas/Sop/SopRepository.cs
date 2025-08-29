using System;
using System.IO;
using System.Data;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using System.Transactions;
using Net.Business.Entities;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using Microsoft.Data.SqlClient;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
namespace Net.Data.Web
{
    public class SopRepository : RepositoryBase<SopEntity>, ISopRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_SET_CREATE = DB_ESQUEMA + "VEN_SetSopCreate";
        const string SP_SET_UPDATE = DB_ESQUEMA + "VEN_SetSopUpdate";
        const string SP_SET_DELETE = DB_ESQUEMA + "VEN_SetSopDelete";
        const string SP_SET_DETALLE_DELETE = DB_ESQUEMA + "VEN_SetSopDetalleDelete";

        const string SP_GET_BY_ID = DB_ESQUEMA + "VEN_GetSopById";
        const string SP_GET_LIST_DETALLE_BY_ID = DB_ESQUEMA + "VEN_GetSopDetalleById";
        const string SP_SET_DETALLE_CREATE = DB_ESQUEMA + "VEN_SetSopDetalleCreate";
        const string SP_SET_DETALLE_UPDATE1 = DB_ESQUEMA + "VEN_SetSopDetalleUpdate1";
        const string SP_SET_DETALLE_UPDATE2 = DB_ESQUEMA + "VEN_SetSopDetalleUpdate2";
        const string SP_GET_LIST_FILTRO = DB_ESQUEMA + "VEN_GetListSopByFiltro";

        public SopRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
        }


        public async Task<ResultadoTransaccionEntity<SopEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<SopEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<SopEntity>();

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
                        cmd.Parameters.Add(new SqlParameter("@CodYear", value.Id1));
                        cmd.Parameters.Add(new SqlParameter("@CodMonth", value.Id2));
                        cmd.Parameters.Add(new SqlParameter("@CodWeek", value.Id3));
                        cmd.Parameters.Add(new SqlParameter("@Filter", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<SopEntity>)context.ConvertTo<SopEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SopEntity>> GetById(int id)
        {
            var response = new SopEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<SopEntity>();

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
                            response = context.Convert<SopEntity>(reader);
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_DETALLE_BY_ID, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response.Linea = (List<SopDetalleEntity>)context.ConvertTo<SopDetalleEntity>(reader);
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
        public async Task<ResultadoTransaccionEntity<SopEntity>> SetCreate(SopEntity value)
        {
            var resultadoTransaccion = new ResultadoTransaccionEntity<SopEntity>();

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

                        using (SqlCommand cmd = new SqlCommand(SP_SET_CREATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@CodYear", value.CodYear));
                            cmd.Parameters.Add(new SqlParameter("@CodMonth", value.CodMonth));
                            cmd.Parameters.Add(new SqlParameter("@CodWeek", value.CodWeek));
                            cmd.Parameters.Add(new SqlParameter("@Name", value.Name));
                            cmd.Parameters.Add(new SqlParameter("@Comments", value.Comments));
                            cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", value.IdUsuarioCreate));

                            await cmd.ExecuteNonQueryAsync();

                            value.Id = (int)cmd.Parameters["@Id"].Value;

                            foreach (var item in value.Linea)
                            {
                                item.Id = value.Id;
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_CREATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;

                            foreach (var item in value.Linea)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", item.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", SqlDbType.Int)).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(new SqlParameter("@DocEntry", item.DocEntry));
                                cmd.Parameters.Add(new SqlParameter("@LineNum", item.LineNum));
                                cmd.Parameters.Add(new SqlParameter("@DocNum", item.DocNum));
                                cmd.Parameters.Add(new SqlParameter("@DocDate", item.DocDate));
                                cmd.Parameters.Add(new SqlParameter("@CodTipDocumento", item.CodTipDocumento));
                                cmd.Parameters.Add(new SqlParameter("@NomTipDocumento", item.NomTipDocumento));
                                cmd.Parameters.Add(new SqlParameter("@CardCode", item.CardCode));
                                cmd.Parameters.Add(new SqlParameter("@CardName", item.CardName));
                                cmd.Parameters.Add(new SqlParameter("@CodOriCliente", item.CodOriCliente));
                                cmd.Parameters.Add(new SqlParameter("@NomOriCliente", item.NomOriCliente));
                                cmd.Parameters.Add(new SqlParameter("@SlpCode", item.SlpCode));
                                cmd.Parameters.Add(new SqlParameter("@SlpName", item.SlpName));
                                cmd.Parameters.Add(new SqlParameter("@ItemCode", item.ItemCode));
                                cmd.Parameters.Add(new SqlParameter("@ItemName", item.ItemName));
                                cmd.Parameters.Add(new SqlParameter("@CodLinNegocio", item.CodLinNegocio));
                                cmd.Parameters.Add(new SqlParameter("@NomLinNegocio", item.NomLinNegocio));
                                cmd.Parameters.Add(new SqlParameter("@CodGpoArticulo", item.CodGpoArticulo));
                                cmd.Parameters.Add(new SqlParameter("@NomGpoArticulo", item.NomGpoArticulo));
                                cmd.Parameters.Add(new SqlParameter("@SalUnitMsr", item.SalUnitMsr));
                                cmd.Parameters.Add(new SqlParameter("@Stock", item.Stock));
                                cmd.Parameters.Add(new SqlParameter("@QtyEarring", item.QtyEarring));
                                cmd.Parameters.Add(new SqlParameter("@PesoPromedioKg", item.PesoPromedioKg));
                                cmd.Parameters.Add(new SqlParameter("@KgEarring", item.KgEarring));
                                cmd.Parameters.Add(new SqlParameter("@Price", item.Price));
                                cmd.Parameters.Add(new SqlParameter("@LineTotEarring", item.LineTotEarring));
                                cmd.Parameters.Add(new SqlParameter("@CodConPago", item.CodConPago));
                                cmd.Parameters.Add(new SqlParameter("@NomConPago", item.NomConPago));
                                cmd.Parameters.Add(new SqlParameter("@FecEntFinal", item.FecEntFinal));
                                cmd.Parameters.Add(new SqlParameter("@FecEntProdProceso", item.FecEntProdProceso));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", item.IdUsuarioCreate));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();

                        resultadoTransaccion.IdRegistro = 0;
                        resultadoTransaccion.ResultadoCodigo = 0;
                        resultadoTransaccion.ResultadoDescripcion = "Registro actualizado con éxito ..!";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    resultadoTransaccion.IdRegistro = -1;
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }

            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<SopEntity>> SetUpdate(SopEntity value)
        {
            var resultadoTransaccion = new ResultadoTransaccionEntity<SopEntity>();

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

                        using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add(new SqlParameter("@Id", value.Id));
                            cmd.Parameters.Add(new SqlParameter("@CodYear", value.CodYear));
                            cmd.Parameters.Add(new SqlParameter("@CodMonth", value.CodMonth));
                            cmd.Parameters.Add(new SqlParameter("@CodWeek", value.CodWeek));
                            cmd.Parameters.Add(new SqlParameter("@Name", value.Name));
                            cmd.Parameters.Add(new SqlParameter("@Comments", value.Comments));
                            cmd.Parameters.Add(new SqlParameter("@IdUsuarioUpdate", value.IdUsuarioUpdate));

                            await cmd.ExecuteNonQueryAsync();
                        }

                        foreach (var item in value.Linea.Where(x => x.Record == 1))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_CREATE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", item.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", SqlDbType.Int)).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(new SqlParameter("@DocEntry", item.DocEntry));
                                cmd.Parameters.Add(new SqlParameter("@LineNum", item.LineNum));
                                cmd.Parameters.Add(new SqlParameter("@DocNum", item.DocNum));
                                cmd.Parameters.Add(new SqlParameter("@DocDate", item.DocDate));
                                cmd.Parameters.Add(new SqlParameter("@CodTipDocumento", item.CodTipDocumento));
                                cmd.Parameters.Add(new SqlParameter("@NomTipDocumento", item.NomTipDocumento));
                                cmd.Parameters.Add(new SqlParameter("@CardCode", item.CardCode));
                                cmd.Parameters.Add(new SqlParameter("@CardName", item.CardName));
                                cmd.Parameters.Add(new SqlParameter("@CodOriCliente", item.CodOriCliente));
                                cmd.Parameters.Add(new SqlParameter("@NomOriCliente", item.NomOriCliente));
                                cmd.Parameters.Add(new SqlParameter("@SlpCode", item.SlpCode));
                                cmd.Parameters.Add(new SqlParameter("@SlpName", item.SlpName));
                                cmd.Parameters.Add(new SqlParameter("@ItemCode", item.ItemCode));
                                cmd.Parameters.Add(new SqlParameter("@ItemName", item.ItemName));
                                cmd.Parameters.Add(new SqlParameter("@CodLinNegocio", item.CodLinNegocio));
                                cmd.Parameters.Add(new SqlParameter("@NomLinNegocio", item.NomLinNegocio));
                                cmd.Parameters.Add(new SqlParameter("@CodGpoArticulo", item.CodGpoArticulo));
                                cmd.Parameters.Add(new SqlParameter("@NomGpoArticulo", item.NomGpoArticulo));
                                cmd.Parameters.Add(new SqlParameter("@SalUnitMsr", item.SalUnitMsr));
                                cmd.Parameters.Add(new SqlParameter("@Stock", item.Stock));
                                cmd.Parameters.Add(new SqlParameter("@QtyEarring", item.QtyEarring));
                                cmd.Parameters.Add(new SqlParameter("@PesoPromedioKg", item.PesoPromedioKg));
                                cmd.Parameters.Add(new SqlParameter("@KgEarring", item.KgEarring));
                                cmd.Parameters.Add(new SqlParameter("@Price", item.Price));
                                cmd.Parameters.Add(new SqlParameter("@LineTotEarring", item.LineTotEarring));
                                cmd.Parameters.Add(new SqlParameter("@CodConPago", item.CodConPago));
                                cmd.Parameters.Add(new SqlParameter("@NomConPago", item.NomConPago));
                                cmd.Parameters.Add(new SqlParameter("@FecEntFinal", item.FecEntFinal));
                                cmd.Parameters.Add(new SqlParameter("@FecEntProdProceso", item.FecEntProdProceso));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", item.IdUsuarioCreate));

                                await cmd.ExecuteNonQueryAsync();

                                item.Line = (int)cmd.Parameters["@Line"].Value;
                            }
                        }

                        foreach (var item in value.Linea.Where(x => x.Record == 3))
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_UPDATE1, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", item.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", item.Line));
                                cmd.Parameters.Add(new SqlParameter("@QtyEarring", item.QtyEarring));
                                cmd.Parameters.Add(new SqlParameter("@KgEarring", item.KgEarring));
                                cmd.Parameters.Add(new SqlParameter("@LineTotEarring", item.LineTotEarring));
                                cmd.Parameters.Add(new SqlParameter("@FecEntFinal", item.FecEntFinal));
                                cmd.Parameters.Add(new SqlParameter("@FecEntProdProceso", item.FecEntProdProceso));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioUpdate", item.IdUsuarioUpdate));

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        foreach (var item in value.Linea)
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_UPDATE2, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@Id", item.Id));
                                cmd.Parameters.Add(new SqlParameter("@Line", item.Line));
                                cmd.Parameters.Add(new SqlParameter("@Order", item.Order));
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();

                        resultadoTransaccion.IdRegistro = 0;
                        resultadoTransaccion.ResultadoCodigo = 0;
                        resultadoTransaccion.ResultadoDescripcion = "Registro actualizado con éxito ..!";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    resultadoTransaccion.IdRegistro = -1;
                    resultadoTransaccion.ResultadoCodigo = -1;
                    resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }

            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<SopEntity>> SetDelete(SopEntity value)
        {
            var resultadoTransaccion = new ResultadoTransaccionEntity<SopEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(SP_SET_DELETE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", value.Id));

                        await cmd.ExecuteNonQueryAsync();
                    }

                    resultadoTransaccion.IdRegistro = 0;
                    resultadoTransaccion.ResultadoCodigo = 0;
                    resultadoTransaccion.ResultadoDescripcion = "Registro elimiado con éxito ..!";
                }
            }
            catch (Exception ex)
            {
                resultadoTransaccion.IdRegistro = -1;
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<SopDetalleEntity>> SetDeleteDetalle(SopDetalleEntity value)
        {
            var resultadoTransaccion = new ResultadoTransaccionEntity<SopDetalleEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(SP_SET_DETALLE_DELETE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@Id", value.Id));
                        cmd.Parameters.Add(new SqlParameter("@Line", value.Line));

                        await cmd.ExecuteNonQueryAsync();
                    }

                    resultadoTransaccion.IdRegistro = 0;
                    resultadoTransaccion.ResultadoCodigo = 0;
                    resultadoTransaccion.ResultadoDescripcion = "Registro elimiado con éxito ..!";
                }
            }
            catch (Exception ex)
            {
                resultadoTransaccion.IdRegistro = -1;
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultadoTransaccion;
        }
        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetSopExcelById(FilterRequestEntity value)
        {
            var ms = new MemoryStream();
            var resultadoTransaccion = new ResultadoTransaccionEntity<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Proyección - Venta" };
                    sheets.Append(sheet);

                    workbookPart.Workbook.Save();

                    SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    //Cabecera
                    Row row = new Row();
                    row.Append(
                    ExportToExcel.ConstructCell("Código de cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Tipo de documento", CellValues.String),
                    ExportToExcel.ConstructCell("Número de pedido", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de contabilización", CellValues.String),
                    ExportToExcel.ConstructCell("Código de artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Nombre de artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Stock", CellValues.String),
                    ExportToExcel.ConstructCell("Cantidad pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Kg pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Precio", CellValues.String),
                    ExportToExcel.ConstructCell("Importe pendiente", CellValues.String),
                    ExportToExcel.ConstructCell("Vendedor", CellValues.String),
                    ExportToExcel.ConstructCell("Origen de cliente", CellValues.String),
                    ExportToExcel.ConstructCell("Línea de negocio", CellValues.String),
                    ExportToExcel.ConstructCell("Grupo de artículo", CellValues.String),
                    ExportToExcel.ConstructCell("Condición de pago", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de entrega fianl", CellValues.String),
                    ExportToExcel.ConstructCell("Fecha de entrega PP", CellValues.String));
                    sheetData.AppendChild(row);

                    var objectGetList = await GetById(value.Id1);

                    //Contenido
                    foreach (var item in objectGetList.data.Linea)
                    {
                        row = new Row();
                        row.Append(
                        ExportToExcel.ConstructCell(item.CardCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.CardName, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomTipDocumento, CellValues.String),
                        ExportToExcel.ConstructCell(item.DocNum.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.DocDate.ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemCode, CellValues.String),
                        ExportToExcel.ConstructCell(item.ItemName, CellValues.String),
                        ExportToExcel.ConstructCell(item.Stock.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.QtyEarring.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.KgEarring.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.Price.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.LineTotEarring.ToString(), CellValues.Number),
                        ExportToExcel.ConstructCell(item.SlpName.ToString(), CellValues.String),
                        ExportToExcel.ConstructCell(item.NomOriCliente.ToString(), CellValues.String),
                        ExportToExcel.ConstructCell(item.NomLinNegocio, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomGpoArticulo, CellValues.String),
                        ExportToExcel.ConstructCell(item.NomConPago, CellValues.String),
                        ExportToExcel.ConstructCell(item.FecEntFinal == null ? "" : Convert.ToDateTime(item.FecEntFinal).ToString("dd/MM/yyyy"), CellValues.String),
                        ExportToExcel.ConstructCell(item.FecEntFinal == null ? "" : Convert.ToDateTime(item.FecEntProdProceso).ToString("dd/MM/yyyy"), CellValues.String));
                        sheetData.Append(row);
                    }

                    worksheetPart.Worksheet.Save();
                    document.Close();
                }

                resultadoTransaccion.IdRegistro = 0;
                resultadoTransaccion.ResultadoCodigo = 0;
                resultadoTransaccion.ResultadoDescripcion = "Archivo generado con éxito.";
                resultadoTransaccion.data = ms;
            }
            catch (Exception ex)
            {
                resultadoTransaccion.IdRegistro = -1;
                resultadoTransaccion.ResultadoCodigo = -1;
                resultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultadoTransaccion;
        }
    }
}
