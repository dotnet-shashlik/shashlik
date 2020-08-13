using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using ExcelDataReader.Exceptions;
using NPOI.XSSF.UserModel;
using Guc.Utils.Extensions;

namespace Guc.Utils.Common
{
    /// <summary>
    /// excel帮助类
    /// </summary>
    public class ExcelHelper
    {
        static ExcelHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// excel内容转换为dataset数据集
        /// </summary>
        /// <param name="excelStream">excel文件数据流</param>
        /// <returns></returns>
        public static DataSet ToDataSet(Stream excelStream)
        {
            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(excelStream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                        }
                    } while (reader.NextResult());

                    return reader.AsDataSet();
                }
            }
            catch (HeaderException)
            {
                // 文件格式不正确
                return null;
            }
        }

        /// <summary>
        /// 使用模板导出到excel
        /// </summary>
        /// <param name="template">模板文件</param>
        /// <param name="outputStream">导出到的数据流</param>
        /// <param name="table">数据表</param>
        /// <param name="beginRowNumber">从第几行开始写,索引从0开始</param>
        /// <param name="colRowNumber">从第几列开始,索引从0开始</param>
        public static void WriteTo(Stream template, Stream outputStream, DataTable table, int beginRowNumber = 0, int colRowNumber = 0)
        {
            var workbook = new XSSFWorkbook(template);
            var sheet = workbook.GetSheetAt(0);

            int rowNumber = beginRowNumber;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = sheet.CreateRow(rowNumber);
                var rowData = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    var value = rowData[j] == null || rowData[j] == DBNull.Value ? "" : rowData[j].ToString();
                    row.CreateCell(j + colRowNumber).SetCellValue(value);
                }
                rowNumber++;
            }

            workbook.Write(outputStream);
        }

        /// <summary>
        /// 导出到excel
        /// </summary>
        /// <param name="outputStream">导出到的数据流</param>
        /// <param name="table">数据表</param>
        /// <param name="containsTitle">是否包含标题(列名)</param>
        public static void WriteTo(Stream outputStream, DataTable table, bool containsTitle)
        {

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(table.TableName.IsNullOrWhiteSpace() ? "Sheet1" : table.TableName);

            if (containsTitle)
            {
                var row = sheet.CreateRow(0);
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(table.Columns[i].ColumnName);
                }
            }

            int rowNumber = containsTitle ? 1 : 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = sheet.CreateRow(rowNumber);
                var rowData = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    var value = rowData[j] == null || rowData[j] == DBNull.Value ? "" : rowData[j].ToString();
                    row.CreateCell(j).SetCellValue(value);
                }
                rowNumber++;
            }

            workbook.Write(outputStream);
        }
    }
}
