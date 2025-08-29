using System;
using System.IO;
using System.Data;
using Net.Connection;
using Net.CrossCotting;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net.Business.Entities.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
namespace Net.Data.Web
{
    public class PickingListRepository : RepositoryBase<PickingListEntity>, IPickingListRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly IConfiguration _configuration;
        private readonly IConnectionSap _connectionSap;

        // STORED PROCEDURE
        const string DB_ESQUEMA = "";
        const string SP_GET_LIST_BY_DOCENTRY = DB_ESQUEMA + "VEN_GetListPackListByDocEntry";
        const string SP_GET_LIST_ITEM_BY_DOCENTRY = DB_ESQUEMA + "VEN_GetListPackListItemByDocEntry";

        public PickingListRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = GetType().Name;
            _connectionSap = new ConnectionSap();
        }

        public async Task<ResultadoTransaccionEntity<MemoryStream>> GetListPickingPdfByDocEntry(int docEntry)
        {
            var listpackingListSap = new List<PickingItem1Entity>();
            var listpackingListItemSap = new List<PickingItem2Entity>();
            var resultadoTransaccion = new ResultadoTransaccionEntity<MemoryStream>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            resultadoTransaccion.NombreMetodo = _metodoName;
            resultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.GetConnectionSQL()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_BY_DOCENTRY, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@DocEntry", docEntry));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            listpackingListSap = (List<PickingItem1Entity>)context.ConvertTo<PickingItem1Entity>(reader);
                        }
                    }


                    iTextSharp.text.Document doc = new iTextSharp.text.Document();
                    doc.SetPageSize(iTextSharp.text.PageSize.A4);
                    doc.SetMargins(15f, 10f, 120f, 15f);
                    MemoryStream ms = new MemoryStream();
                    iTextSharp.text.pdf.PdfWriter write = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                    write.ViewerPreferences = iTextSharp.text.pdf.PdfWriter.PageModeUseOutlines;
                    // Our custom Header and Footer is done using Event Handler
                    PageEventHelperPicking pageEventHelperSolicitudViaje = new PageEventHelperPicking();
                    write.PageEvent = pageEventHelperSolicitudViaje;

                    // Colocamos la fuente que deseamos que tenga el documento
                    iTextSharp.text.pdf.BaseFont helvetica = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, true);
                    // Titulo
                    iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 11f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoItem = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNormal = new iTextSharp.text.Font(helvetica, 11f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.Black);
                    iTextSharp.text.Font parrafoNegroItalic = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.Black);

                    // Define the page header
                    pageEventHelperSolicitudViaje.Title = "PICKING LIST";
                    pageEventHelperSolicitudViaje.Cliente = listpackingListSap.Count == 0 ? "" : listpackingListSap[0].CardName;
                    pageEventHelperSolicitudViaje.Contenedor = listpackingListSap.Count == 0 ? "" : listpackingListSap[0].Contenedor;

                    try
                    {
                        doc.Open();
                    }
                    catch (Exception)
                    {
                        throw;
                    }


                    foreach (var packingListSap in listpackingListSap)
                    {
                        //============================
                        //Tabla: 2
                        var tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 100f }) { WidthPercentage = 100 };
                        //Línea 1
                        var c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(packingListSap.ItemName, parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);

                        doc.Add(tbl);

                        using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_ITEM_BY_DOCENTRY, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@DocEntry", docEntry));
                            cmd.Parameters.Add(new SqlParameter("@ItemCode", packingListSap.ItemCode));

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                listpackingListItemSap = (List<PickingItem2Entity>)context.ConvertTo<PickingItem2Entity>(reader);
                            }
                        }

                        //============================
                        //Tabla: 3
                        tbl = new iTextSharp.text.pdf.PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100 };

                        foreach (var packingListItemSap in listpackingListItemSap)
                        {
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(packingListItemSap.CodeBar1, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(packingListItemSap.CodeBar2, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(packingListItemSap.CodeBar3, parrafoItem)) { BorderWidth = 1, Padding = 5 };
                            tbl.AddCell(c1);
                            c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(packingListItemSap.CodeBar4, parrafoItem)) { BorderWidth = 1, Padding = 5 };
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
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(listpackingListItemSap[0].TotalItem.ToString(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Peso Total", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(":", parrafoNegro)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);
                        c1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(listpackingListItemSap[0].PesoTotal.ToString(), parrafoNormal)) { BorderWidth = 0, PaddingBottom = 10, PaddingTop = 10 };
                        tbl.AddCell(c1);

                        doc.Add(tbl);
                    }


                    write.Close();
                    doc.Close();
                    ms.Seek(0, SeekOrigin.Begin);
                    var file = ms;

                    resultadoTransaccion.IdRegistro = 0;
                    resultadoTransaccion.ResultadoCodigo = 0;
                    resultadoTransaccion.ResultadoDescripcion = "Se generó correctamente el archivo.s";
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

    public class PageEventHelperPicking : iTextSharp.text.pdf.PdfPageEventHelper
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
