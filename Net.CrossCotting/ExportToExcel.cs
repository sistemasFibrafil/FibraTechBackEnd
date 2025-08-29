using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
namespace Net.CrossCotting
{
    public static class ExportToExcel
    {
        public static Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
    }
}
