using System;
using System.IO;
using System.Data;
using System.Linq;
using Net.Connection;
using System.Transactions;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Net.Data.Web
{
    public class LecturaRepository : RepositoryBase<LecturaEntity>, ILecturaRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_BY_FILTRO = DB_ESQUEMA + "INV_GetListLecturaByFiltro";
        const string SP_GET_LIST_BY_BASETYPE_AND_BASEENTRY = DB_ESQUEMA + "INV_GetListLecturaByBaseTypeBaseEntry";
        const string SP_GET_LIST_BY_BASETYPE_BASEENTRY_BASELINE_FILTRO = DB_ESQUEMA + "INV_GetListLecturaByBaseTypeBaseEntryBaseLineFiltro";
        const string SP_GET_LIST_BY_TARGETTYPE_TRGETENTRY_TRGETLINE_FILTRO = DB_ESQUEMA + "INV_GetListLecturaByTargetTypeTrgetEntryTrgetLineFiltro";

        const string SP_SET_CREATE = DB_ESQUEMA + "INV_SetLecturaCreate";
        const string SP_SET_DELETE1 = DB_ESQUEMA + "INV_SetLecturaDeleteMassive";   
        const string SP_SET_DELETE2 = DB_ESQUEMA + "INV_SetLecturaDelete";

        const string SP_GET_LIST_COPY_TO_TRANSFERENCIA = DB_ESQUEMA + "INV_GetLecturaCopyToTransferencia";
        const string SP_GET_LIST_COPY_TO_TRANSFERENCIA_DETALLE = DB_ESQUEMA + "INV_GetListLecturaCopyToTransferenciaDetalle";


        const string SP_GET_LIST_PACKING_LIST = DB_ESQUEMA + "INV_GetPackingListByTargetTypeTrgetEntry";
        const string SP_GET_LIST_PACKING_LIST_DETALLE = DB_ESQUEMA + "INV_GetPackingListDetalleByTargetTypeTrgetEntry";


        public LecturaRepository(IConnectionSQL context)
            : base(context)
        {
            _aplicacionName = GetType().Name;
        }

        public async Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByFiltro(FilterRequestEntity value)
        {
            var response = new List<LecturaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<LecturaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@BaseType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@Estado", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@Numero", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<LecturaEntity>)context.ConvertTo<LecturaEntity>(reader);
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

        public async Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByBaseTypeAndBaseEntry(FilterRequestEntity value)
        {
            var response = new List<LecturaEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<LecturaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_BASETYPE_AND_BASEENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@BaseType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@BaseEntry", value.Id1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<LecturaEntity>)context.ConvertTo<LecturaEntity>(reader);
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

        public async Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByBaseTypeBaseEntryBaseLineFiltro(FilterRequestEntity value)
        {
            var response = new List<LecturaEntity>();
            var resultadoTransaccion = new ResultadoTransaccionEntity<LecturaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_BASETYPE_BASEENTRY_BASELINE_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@FI", value.Dat1));
                        cmd.Parameters.Add(new SqlParameter("@FF", value.Dat2));
                        cmd.Parameters.Add(new SqlParameter("@BaseType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@BaseEntry", value.Id1));
                        cmd.Parameters.Add(new SqlParameter("@BaseLine", value.Id2));
                        cmd.Parameters.Add(new SqlParameter("@Return", value.Cod2));
                        cmd.Parameters.Add(new SqlParameter("@DocStatus", value.Cod3));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<LecturaEntity>)context.ConvertTo<LecturaEntity>(reader);
                        }
                    }

                    resultadoTransaccion.IdRegistro = 0;
                    resultadoTransaccion.ResultadoCodigo = 0;
                    resultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultadoTransaccion.dataList = response;
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

        public async Task<ResultadoTransaccionEntity<LecturaEntity>> GetListByTargetTypeTrgetEntryTrgetLineFiltro(FilterRequestEntity value)
        {
            var response = new List<LecturaEntity>();
            var resultadoTransaccion = new ResultadoTransaccionEntity<LecturaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_TARGETTYPE_TRGETENTRY_TRGETLINE_FILTRO, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@TargetType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@TrgetEntry", value.Id1));
                        cmd.Parameters.Add(new SqlParameter("@TrgetLine", value.Id2));
                        cmd.Parameters.Add(new SqlParameter("@Filtro", value.Text1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<LecturaEntity>)context.ConvertTo<LecturaEntity>(reader);
                        }
                    }

                    resultadoTransaccion.IdRegistro = 0;
                    resultadoTransaccion.ResultadoCodigo = 0;
                    resultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    resultadoTransaccion.dataList = response;
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

        public async Task<ResultadoTransaccionEntity<LecturaEntity>> SetCreate(LecturaEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<LecturaEntity>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

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
                            using (SqlCommand cmd = new SqlCommand(SP_SET_CREATE, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 0;
                                cmd.Parameters.Add(new SqlParameter("@BaseType", value.BaseType));
                                cmd.Parameters.Add(new SqlParameter("@BaseEntry", value.BaseEntry));
                                cmd.Parameters.Add(new SqlParameter("@FromWhsCod", value.FromWhsCod));
                                cmd.Parameters.Add(new SqlParameter("@Barcode", value.Barcode));
                                cmd.Parameters.Add(new SqlParameter("@IdUsuarioCreate", value.IdUsuarioCreate));

                                await cmd.ExecuteNonQueryAsync();
                            }

                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = "Registro creado con éxito ..!";

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            resultTransaccion.IdRegistro = -1;
                            resultTransaccion.ResultadoCodigo = -1;
                            resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                        }
                    }
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

        public async Task<ResultadoTransaccionEntity<LecturaEntity>> SetDeleteMassive(LecturaEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<LecturaEntity>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

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
                            using (SqlCommand cmdItem = new SqlCommand(SP_SET_DELETE1, conn))
                            {
                                cmdItem.CommandType = CommandType.StoredProcedure;
                                cmdItem.CommandTimeout = 0;
                                cmdItem.Parameters.Add(new SqlParameter("@BaseType", value.BaseType));
                                cmdItem.Parameters.Add(new SqlParameter("@BaseEntry", value.BaseEntry));
                                cmdItem.Parameters.Add(new SqlParameter("@BaseLine", value.BaseLine));
                                cmdItem.Parameters.Add(new SqlParameter("@Return", value.Return));
                                cmdItem.Parameters.Add(new SqlParameter("@DocStatus", value.DocStatus));

                                await cmdItem.ExecuteNonQueryAsync();
                            }

                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = "Registro elminado con éxito ..!";

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            resultTransaccion.IdRegistro = -1;
                            resultTransaccion.ResultadoCodigo = -1;
                            resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                        }
                    }
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

        public async Task<ResultadoTransaccionEntity<LecturaEntity>> SetDelete(LecturaEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<LecturaEntity>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

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
                            using (SqlCommand cmdItem = new SqlCommand(SP_SET_DELETE2, conn))
                            {
                                cmdItem.CommandType = CommandType.StoredProcedure;
                                cmdItem.CommandTimeout = 0;
                                cmdItem.Parameters.Add(new SqlParameter("@Id", value.Id));

                                await cmdItem.ExecuteNonQueryAsync();
                            }

                            resultTransaccion.IdRegistro = 0;
                            resultTransaccion.ResultadoCodigo = 0;
                            resultTransaccion.ResultadoDescripcion = "Registro elminado con éxito ..!";

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            resultTransaccion.IdRegistro = -1;
                            resultTransaccion.ResultadoCodigo = -1;
                            resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
                        }
                    }
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

        public async Task<ResultadoTransaccionEntity<LecturaCopyToTransferenciaEntity>> GetLecturaCopyToTransferencia(LecturaCopyToTransferenciaFindEntity value)
        {
            var response = new LecturaCopyToTransferenciaEntity();
            var resultTransaccion = new ResultadoTransaccionEntity<LecturaCopyToTransferenciaEntity>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_COPY_TO_TRANSFERENCIA, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@BaseType", value.BaseType));
                        cmd.Parameters.Add(new SqlParameter("@IdBase", value.IdBase));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<LecturaCopyToTransferenciaEntity>(reader);
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_COPY_TO_TRANSFERENCIA_DETALLE, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        foreach (var linea in value.Linea)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@IdBase", linea.IdBase));
                            cmd.Parameters.Add(new SqlParameter("@LineBase", linea.LineBase));
                            cmd.Parameters.Add(new SqlParameter("@BaseType", linea.BaseType));
                            cmd.Parameters.Add(new SqlParameter("@Read", linea.Read));
                            cmd.Parameters.Add(new SqlParameter("@Return", linea.Return));

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                var lista = (List<LecturaCopyToTransferenciaDetalleEntity1>)context.ConvertTo<LecturaCopyToTransferenciaDetalleEntity1>(reader);

                                
                                foreach (var line in lista)
                                {
                                    response.Linea1.Add(line);
                                }
                            }
                        }
                    }

                    response.Linea2 = response.Linea1
                    .GroupBy
                    (p => new
                    {
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
                        p.NomTipOperacion,
                        p.UnitMsr,
                    })
                    .Select
                    (g => new LecturaCopyToTransferenciaDetalleEntity2
                    {
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
                        NomTipOperacion = g.Key.NomTipOperacion,
                        UnitMsr = g.Key.UnitMsr,
                        Quantity = g.Sum(p => p.Quantity),
                        OpenQty = g.Sum(p => p.OpenQty),
                        Bulto = g.Sum(p => p.Bulto),
                        Peso = g.Sum(p => p.Peso),
                    })
                    .OrderBy(x=>x.BaseEntry)
                    .ThenBy(x=>x.BaseLine)
                    .ToList();

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

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetPackingListPdfByTargetTypeTrgetEntry(FilterRequestEntity value)
        {
            var packingList = new List<PackingListEntity>();
            var packingListDetalle = new List<PackingListDetalleEntity>();
            var resultadoTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_PACKING_LIST, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@TargetType", value.Cod1));
                        cmd.Parameters.Add(new SqlParameter("@TrgetEntry", value.Id1));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            packingList = (List<PackingListEntity>)context.ConvertTo<PackingListEntity>(reader);
                        }
                    }

                    if(packingList.Count == 0)
                    {
                        resultadoTransaccion.IdRegistro = -1;
                        resultadoTransaccion.ResultadoCodigo = -1;
                        resultadoTransaccion.ResultadoDescripcion = "No exiten lecturas para generar el packing list ..!";
                    }


                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    doc.SetPageSize(iTextSharp.text.PageSize.A4);
                    if(string.IsNullOrEmpty(packingList[0].CardName))
                    {
                        doc.SetMargins(15f, 10f, 70f, 15f);
                    }
                    else
                    {
                        doc.SetMargins(15f, 10f, 120f, 15f);
                    }
                    MemoryStream ms = new MemoryStream();
                    iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                    write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                    // Our custom Header and Footer is done using Event Handler
                    PageEventHelperPacking packing = new PageEventHelperPacking();
                    write.PageEvent = packing;

                    // Colocamos la fuente que deseamos que tenga el documento
                    iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                    // Titulo
                    iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 11f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoItem = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 11f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNegroItalic = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.Black);

                    // Define the page header
                    packing.Title = "PACKING LIST";
                    packing.Cliente = packingList.Count == 0 ? "" : packingList[0].CardName;
                    packing.Contenedor = packingList.Count == 0 ? "" : packingList[0].Contenedor;

                    try
                    {
                        doc.Open();
                    }
                    catch (Exception)
                    {
                        throw;
                    }


                    foreach (var linea1 in packingList)
                    {
                        //============================
                        //Tabla: 2
                        var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 100f }) { WidthPercentage = 100 };
                        //Línea 1
                        var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(linea1.Dscription, parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);

                        doc.Add(tbl);

                        using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_PACKING_LIST_DETALLE, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@TargetType", linea1.TargetType));
                            cmd.Parameters.Add(new SqlParameter("@TrgetEntry", linea1.TrgetEntry));
                            cmd.Parameters.Add(new SqlParameter("@TrgetLine", linea1.TrgetLine));

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                packingListDetalle = (List<PackingListDetalleEntity>)context.ConvertTo<PackingListDetalleEntity>(reader);
                            }
                        }

                        //============================
                        //Tabla: 3
                        tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100 };

                        foreach (var linea2 in packingListDetalle)
                        {
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(linea2.Barcode1, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(linea2.Barcode2, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(linea2.Barcode3, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(linea2.Barcode4, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                        }

                        doc.Add(tbl);

                        //============================
                        //Tabla: 4
                        tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 15f, 2f, 15f, 36f, 15f, 2f, 15f }) { WidthPercentage = 100 };
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Total Items", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(packingListDetalle[0].TotalItem.ToString(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Peso Total", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(packingListDetalle[0].PesoTotal.ToString(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);

                        doc.Add(tbl);
                    }


                    write.Close();
                    doc.Close();
                    ms.Seek(0, SeekOrigin.Begin);
                    var file = ms;

                    resultadoTransaccion.IdRegistro = 0;
                    resultadoTransaccion.ResultadoCodigo = 0;
                    resultadoTransaccion.ResultadoDescripcion = "Se generó correctamente el archivos";
                    resultadoTransaccion.data = file;
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
    }


    public class PageEventHelperPacking : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;
        DateTime PrintTime = DateTime.Now;

        #region Properties
        public string Title { get; set; }
        public string Cliente { get; set; }
        public string Contenedor { get; set; }
        #endregion

        // we override the onOpenDocument method
        public override void OnOpenDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            try
            {
                bfTitulo = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA_BOLD, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                bfTexto = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1252, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                PrintTime = DateTime.Now;
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (iTextSharp.text.DocumentException)
            {
            }
            catch (IOException)
            {
            }
        }

        public override void OnStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnStartPage(writer, document);
            iTextSharp.text.Rectangle pageSize = document.PageSize;
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(bfTitulo, 11f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);

            if (Title != string.Empty)
            {
                //Titulo
                cb.BeginText();
                cb.SetFontAndSize(bfTitulo, 25);
                cb.SetTextMatrix(pageSize.GetRight(380), pageSize.GetTop(55));
                cb.ShowText(Title);
                cb.EndText();

                //Logo
                var pathLogo = Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");

                var logo = iTextSharp.text.Image.GetInstance(pathLogo);

                logo.ScaleToFit(100f, 35f);
                logo.SetAbsolutePosition(document.Left, pageSize.GetTop(55));
                cb.AddImage(logo);

                ////=========================
                /// INICIO: CLIENTE
                ////=========================
                if (!string.IsNullOrEmpty(Cliente))
                { 
                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetTop(90));
                    cb.ShowText("CLIENTE");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(105), pageSize.GetTop(90));
                    cb.ShowText(":");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTexto, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(115), pageSize.GetTop(90));
                    cb.ShowText(Cliente);
                    cb.EndText();
                
                    ////=========================
                    /// FIN: CLIENTE
                    ////=========================


                    ////=========================
                    /// INICIO: CONTENEDOR
                    ////=========================
                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetTop(110));
                    cb.ShowText("CONTENEDOR");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTitulo, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(105), pageSize.GetTop(110));
                    cb.ShowText(":");
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bfTexto, 12);
                    cb.SetTextMatrix(pageSize.GetLeft(115), pageSize.GetTop(110));
                    cb.ShowText(Contenedor);
                    cb.EndText();
                    ////=========================
                    /// FIN: CONTENEDOR
                    ////=========================
                }
            }
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            /*
                =====================================================
                Codigo para que el número de página muestre en el pie
                =====================================================
            */
            int pageN = writer.PageNumber;
            string text = "Página " + pageN + "/";
            float len = bfTexto.GetWidthPoint(text, 8);
            iTextSharp.text.Rectangle pageSize = document.PageSize;
            cb.SetRgbColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bfTexto, 8);
            cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(footerTemplate, pageSize.GetLeft(15) + len, pageSize.GetBottom(30));
        }
        public override void OnCloseDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);

            /*
               =====================================================
               Codigo para que el número de página muestre en el pie
               =====================================================
           */
            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bfTexto, 8);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText("" + (writer.PageNumber - 1));
            footerTemplate.EndText();
        }
    }
}
