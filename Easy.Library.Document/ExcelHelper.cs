using System.Data;
using Aspose.Cells;

namespace Easy.Library.Document
{
    public class ExcelHelper
    {
        /// <summary>
        /// 根据文件路径获取DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDataTableByExcelPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            Workbook book = new Workbook(filePath);
            Worksheet worksheet = book.Worksheets[0];
            Cells cells = worksheet.Cells;
            DataTable dataTable = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            return dataTable;
        }
    }
}
