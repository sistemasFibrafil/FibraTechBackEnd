using System;
using System.IO;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Net.Business.Entities.Sap;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Sap
{
    public class TransferenciaStockSapRepository : RepositoryBase<TransferenciaStockSapEntity>, ITransferenciaStockSapRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly string _cnxSap;
        private readonly IConfiguration _configuration;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_TRANSFERENCIASTOCK_BY_DOCENTRY = DB_ESQUEMA + "INV_GetTransferenciaStockByDocEntry";
        const string SP_GET_LIST_TRANSFERENCIASTOCK_DETALLE_BY_DOCENTRY = DB_ESQUEMA + "INV_GetListTransferenciaStockDetalleByDocEntry";


        public TransferenciaStockSapRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = GetType().Name;
            _cnxSap = Utilidades.GetCon(configuration, "EntornoConnectionSap:Entorno");
        }



        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetTransferenciaStockPdfByDocEntry(int id)
        {
            var header = new TransferenciaStockQrySapEntity();
            var linea = new List<TransferenciaStockDetalleQrySapEntity>();
            var resultTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultTransaccion.NombreMetodo = _metodoName;
            resultTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxSap))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_TRANSFERENCIASTOCK_BY_DOCENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            header = context.Convert<TransferenciaStockQrySapEntity>(reader);
                        }
                    }

                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                    doc.SetMargins(10f, 10f, 70f, 10f);
                    MemoryStream ms = new MemoryStream();
                    iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                    write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                    // Our custom Header and Footer is done using Event Handler
                    var pageEventHelperTransferencia = new PageEventHelperTransferencia();
                    write.PageEvent = pageEventHelperTransferencia;

                    // Colocamos la fuente que deseamos que tenga el documento
                    iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                    // Titulo
                    iTextSharp.text.Font parrafoLinea = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoHerderNegrita = new iTextSharp.text.Font(helvetica, 8.5f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoHeaderDeatailNegrita = new iTextSharp.text.Font(helvetica, 7f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoDetail = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNegrita = new iTextSharp.text.Font(helvetica, 6.5f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

                    // Define the page header
                    pageEventHelperTransferencia.Title = header.Title;
                    pageEventHelperTransferencia.SubTitle = header.SubTitle;
                    pageEventHelperTransferencia.Codigo = header.Codigo;
                    pageEventHelperTransferencia.Version = header.Version;
                    pageEventHelperTransferencia.Vigencia = header.Vigencia;

                    doc.Open();


                    //============================
                    //TABLA: 1
                    var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 20f, 20f, 20f, 20f, 20f }) { WidthPercentage = 100 };
                    //COLUMNAS
                    var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Fecha: " + header.TaxDate.ToString("dd/MM/yyyy"), parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Sede Origen: " + header.SedeOrigen, parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Sede Destino: " + header.SedeDestino, parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Tipo: " + header.TipoTraslado, parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("# SAP: " + header.DocNum.ToString(), parrafoHerderNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 12;
                    c1.PaddingBottom = 20;
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    doc.Add(tbl);


                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_TRANSFERENCIASTOCK_DETALLE_BY_DOCENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@DocEntry", id));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            linea = (List<TransferenciaStockDetalleQrySapEntity>)context.ConvertTo<TransferenciaStockDetalleQrySapEntity>(reader);
                        }
                    }

                    //============================
                    //TABLA: 2 - Cabecera del deatalle
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 5f, 16f, 62f, 6f, 6f, 5f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("# SOL", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ITEM CODE", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ITEM NAME", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ALMACEN O.", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("ALMACEN D.", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("CANTIDAD", parrafoHeaderDeatailNegrita));
                    c1.BorderWidth = 1;
                    c1.PaddingTop = 5;
                    c1.PaddingBottom = 7;
                    c1.BackgroundColor = new iTextSharp.text.BaseColor(255, 165, 122);
                    c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    foreach (var item in linea)
                    {
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.NumSolicitud.ToString(), parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.ItemCode, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.ItemName, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.FromWhsCod, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.WhsCode, parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(item.Quantity.ToString("N2"), parrafoDetail));
                        c1.BorderWidth = 1;
                        c1.PaddingBottom = 4;
                        c1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);
                    }

                    doc.Add(tbl);

                    doc.Add(new iTextSharp.text.Phrase("\n\n", parrafoLinea));

                    //TABLA: 3 - Observaciones
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 7f, 93f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Observaciones: ", parrafoNegrita));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header.Comments, parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                    doc.Add(new iTextSharp.text.Phrase("\n", parrafoLinea));

                    //TABLA: 4 - Datos de los responsables
                    // LINEA: 1
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 1f, 35f, 4f, 12f, 1f, 35f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Entregado por", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Recibido por", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingBottom = 5;
                    tbl.AddCell(c1);
                    // LINEA: 2
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Nombres y Apellidos", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Nombres y Apellidos", parrafoNegrita));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    c1.PaddingTop = 5;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                    doc.Add(new iTextSharp.text.Phrase("\n\n", parrafoLinea));

                    //TABLA: 5 - Firmas
                    // LINEA: 1
                    tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 1f, 35f, 4f, 12f, 1f, 35f }) { WidthPercentage = 100 };
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Firma", parrafoNegrita));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Firma", parrafoNegrita));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNormal));
                    c1.BorderWidth = 0;
                    tbl.AddCell(c1);
                    c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal));
                    c1.BorderWidth = 0;
                    c1.BorderWidthBottom = 1;
                    tbl.AddCell(c1);

                    doc.Add(tbl);


                    write.Close();
                    doc.Close();
                    ms.Seek(0, SeekOrigin.Begin);
                    var file = ms;

                    resultTransaccion.IdRegistro = 0;
                    resultTransaccion.ResultadoCodigo = 0;
                    resultTransaccion.ResultadoDescripcion = "Se generó correctamente el archivo.s";
                    resultTransaccion.data = file;
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



    public class PageEventHelperTransferencia : iTextSharp.text.pdf.PdfPageEventHelper
    {
        iTextSharp.text.pdf.PdfContentByte cb;
        iTextSharp.text.pdf.PdfTemplate headerTemplate, footerTemplate;
        iTextSharp.text.pdf.BaseFont bfTitulo = null;
        iTextSharp.text.pdf.BaseFont bfTexto = null;
        DateTime PrintTime = DateTime.Now;

        #region Properties
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Codigo { get; set; }
        public string Version { get; set; }
        public string Vigencia { get; set; }
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
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(100, 100);
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
            iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(bfTitulo, 7f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
            iTextSharp.text.Font parrafoTitulo = new iTextSharp.text.Font(bfTitulo, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.White);
            iTextSharp.text.Font parrafoSubTitulo = new iTextSharp.text.Font(bfTitulo, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);

            if (Title != string.Empty)
            {
                //Logo
                var pathLogo = Path.Combine(Environment.CurrentDirectory, "logos", "fibrafil-logo.jpg");
                var logo = iTextSharp.text.Image.GetInstance(pathLogo);
                logo.ScaleToFit(100f, 50f);
                logo.SetAbsolutePosition(pageSize.GetLeft(12), pageSize.GetTop(65));
                cb.AddImage(logo);

                // Código
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(22));
                cb.ShowText("Código");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(22));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de código
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(22));
                cb.ShowText(Codigo);
                cb.EndText();

                // Versión
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(35));
                cb.ShowText("Versión");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(35));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de versión
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(35));
                cb.ShowText(Version);
                cb.EndText();

                // Vigencia
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(48));
                cb.ShowText("Vigencia");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(48));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de vigencia
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(48));
                cb.ShowText(Vigencia);
                cb.EndText();

                // Página
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(61));
                cb.ShowText("Página");
                cb.EndText();
                // :
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(65), pageSize.GetTop(61));
                cb.ShowText(":");
                cb.EndText();
                //: Valor de página
                int pageN = writer.PageNumber;
                string text = "" + pageN + " de ";
                cb.BeginText();
                cb.SetFontAndSize(bfTexto, 8f);
                cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetTop(61));
                cb.ShowText(text);
                cb.EndText();

                float len = bfTexto.GetWidthPoint(text, 8f);
                cb.AddTemplate(headerTemplate, pageSize.GetRight(60) + len, pageSize.GetTop(61));


                /*
                 ================================================
                 TABLA: CABERCERA
                ================================================
                */

                var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 12f, 76f, 12f });
                tbl.TotalWidth = pageSize.Width - 18;

                // LINEA 1
                var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthBottom = 0, BorderWidth = 1,PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(Title, parrafoTitulo)) { BorderWidthLeft = 0, BorderWidthRight = 0,BorderWidthBottom = 0, BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, BackgroundColor = new iTextSharp.text.BaseColor(255, 103, 43) };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthBottom = 0, BorderWidth = 1, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                //// LINEA 2
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthTop = 0, BorderWidth = 1, PaddingTop = 14, PaddingBottom = 14 };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(SubTitle, parrafoSubTitulo)) { BorderWidthLeft = 0, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidth = 1, PaddingTop = 14, PaddingBottom = 14, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidthTop = 0, BorderWidth = 1, PaddingTop = 14, PaddingBottom = 14 };
                tbl.AddCell(c1);
                tbl.WriteSelectedRows(0, -1, pageSize.GetLeft(10), pageSize.GetTop(10), cb);
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
            //int pageN = writer.PageNumber;
            //string text = "Página " + pageN + "/";
            //float len = bfTexto.GetWidthPoint(text, 8);
            //iTextSharp.text.Rectangle pageSize = document.PageSize;
            //cb.SetRgbColorFill(100, 100, 100);
            //cb.BeginText();
            //cb.SetFontAndSize(bfTexto, 8);
            //cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(30));
            //cb.ShowText(text);
            //cb.EndText();
            //cb.AddTemplate(footerTemplate, pageSize.GetLeft(15) + len, pageSize.GetBottom(30));
        }
        public override void OnCloseDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);
            /*
                ==========================================================
                Codigo para que el número de página muestre en la cabecera
                ==========================================================
            */
            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bfTexto, 8);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            /*
               =====================================================
               Codigo para que el número de página muestre en el pie
               =====================================================
           */
            //footerTemplate.BeginText();
            //footerTemplate.SetFontAndSize(bfTexto, 8);
            //footerTemplate.SetTextMatrix(0, 0);
            //footerTemplate.ShowText("" + (writer.PageNumber - 1));
            //footerTemplate.EndText();
        }
    }
}
