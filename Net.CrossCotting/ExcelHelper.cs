using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
namespace Net.CrossCotting
{
    public static class ExcelHelper
    {
        public static Stylesheet CreateStylesheet()
        {
            return new Stylesheet
            (
                // 1) Fonts
                new Fonts
                (
                    new Font(), // índice 0: default
                    new Font
                    (   // índice 1: negrita
                        new Bold(),
                        new Color() { Rgb = HexBinaryValue.FromString("000000") }, // blanco
                        new FontSize() { Val = 11 }
                    )
                ),
                // 2) Fills
                new Fills
                (
                    new Fill(new PatternFill() { PatternType = PatternValues.None }),            // 0
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }),         // 1
                    new Fill(new PatternFill
                    (                                                                            // 2: azul sólido
                        new ForegroundColor { Rgb = HexBinaryValue.FromString("FFFF00") }
                    )
                    { PatternType = PatternValues.Solid })
                ),
                // 3) Borders
                new Borders
                (
                    new Border(),
                    new Border
                    (   // 1: thin border alrededor
                        new LeftBorder { Style = BorderStyleValues.Thin },
                        new RightBorder { Style = BorderStyleValues.Thin },
                        new TopBorder { Style = BorderStyleValues.Thin },
                        new BottomBorder { Style = BorderStyleValues.Thin }
                    )
                ),
                // 4) CellStyleFormats (requerido, al menos uno)
                new CellStyleFormats(new CellFormat()),
                // 5) CellFormats: referencian font, fill y border
                new CellFormats
                (
                    /* 0 */
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0, ApplyBorder = true },
                    /* 1 */
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, ApplyNumberFormat = false },
                    /* 2 */
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, ApplyFont = true, ApplyBorder = true },
                    /* 3 */
                    new CellFormat() { NumberFormatId = 14, BorderId = 1, ApplyNumberFormat = true, ApplyBorder = true },
                    /* 4 */
                    new CellFormat() { NumberFormatId = 4, BorderId = 1, ApplyNumberFormat = true, ApplyBorder = true },
                    // 5: encabezados con wrap text y borde
                    new CellFormat
                    (
                        new Alignment
                        {
                            WrapText = true,
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        }
                    )
                    { FontId = 1, FillId = 2, BorderId = 1, ApplyAlignment = true, ApplyBorder = true }
                ),

                new CellStyles(new CellStyle() { Name = "Normal", FormatId = 0, BuiltinId = 0 })
            );
        }

        public static Cell CreateTextCell(string col, uint row, string text, uint styleIndex)
        {
            return new Cell()
            {
                CellReference = $"{col}{row}",
                DataType = CellValues.String,
                CellValue = new CellValue(text),
                StyleIndex = styleIndex
            };
        }

        public static Cell CreateNumberCell(string col, uint row, decimal value, uint styleIndex)
        {
            return new Cell()
            {
                CellReference = $"{col}{row}",
                DataType = CellValues.Number,
                CellValue = new CellValue(value.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                StyleIndex = styleIndex
            };
        }

        public static Cell CreateDateCell(string col, uint row, DateTime date, uint styleIndex)
        {
            // OpenXML almacena fechas como número de serie
            double oaDate = date.ToOADate();
            return new Cell()
            {
                CellReference = $"{col}{row}",
                CellValue = new CellValue(oaDate.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                DataType = CellValues.Number,
                StyleIndex = styleIndex
            };
        }
    }
}