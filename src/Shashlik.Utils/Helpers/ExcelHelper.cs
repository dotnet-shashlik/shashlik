using System;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using ExcelDataReader.Exceptions;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using Shashlik.Utils.Extensions;

namespace Shashlik.Utils.Helpers
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
                using var reader = ExcelReaderFactory.CreateReader(excelStream);
                do
                {
                    while (reader.Read())
                    {
                    }
                } while (reader.NextResult());

                return reader.AsDataSet();
            }
            catch (HeaderException)
            {
                // 文件格式不正确
                return null;
            }
        }

        /// <summary>
        /// excel内容转换为dataset数据集,**NPOI读取效率太低效,建议使用<see cref="ToDataSet"/>  **
        /// </summary>
        /// <param name="excelStream">excel文件数据流</param>
        /// <returns></returns>
        public static DataSet ToDataSetWithNPOI(Stream excelStream)
        {
            var ds = new DataSet();
            IWorkbook xssWorkbook;
            try
            {
                if (excelStream.CanSeek && excelStream.Position != 0)
                    excelStream.Seek(0, SeekOrigin.Begin);
                xssWorkbook = new SXSSFWorkbook(new XSSFWorkbook(excelStream));
            }
            catch (ZipException)
            {
                if (excelStream.CanSeek && excelStream.Position != 0)
                    excelStream.Seek(0, SeekOrigin.Begin);
                xssWorkbook = new HSSFWorkbook(excelStream);
            }
            catch
            {
                return null;
            }

            for (int i = 0; i < xssWorkbook.NumberOfSheets; i++)
            {
                var sheet = xssWorkbook.GetSheetAt(i);

                var table = new DataTable(sheet.SheetName);
                if (sheet.LastRowNum == 0)
                {
                    ds.Tables.Add(table);
                    continue;
                }

                for (int j = 0; j <= sheet.LastRowNum; j++)
                {
                    var row = sheet.GetRow(j);
                    if (row == null)
                    {
                        table.Rows.Add();
                        continue;
                    }

                    if (table.Columns.Count < row.LastCellNum)
                    {
                        for (int k = table.Columns.Count; k <= row.LastCellNum - 1; k++)
                        {
                            table.Columns.Add($"Column{k}");
                        }
                    }

                    var values = new object[row.LastCellNum];
                    for (int k = 0; k < row.LastCellNum; k++)
                    {
                        var cell = row.GetCell(k);
                        if (cell == null)
                            continue;

                        values[k] = cell.ToString();
                    }

                    table.Rows.Add(values);
                }

                ds.Tables.Add(table);
            }

            return ds;
        }

        /// <summary>
        /// 使用模板导出到excel
        /// </summary>
        /// <param name="template">模板文件</param>
        /// <param name="outputStream">导出到的数据流</param>
        /// <param name="table">数据表</param>
        /// <param name="beginRowNumber">从第几行开始写,索引从0开始</param>
        /// <param name="beginColNumber">从第几列开始,索引从0开始</param>
        public static void WriteTo(Stream template, Stream outputStream, DataTable table, int beginRowNumber = 0,
            int beginColNumber = 0)
        {
            var workbook = new XSSFWorkbook(template);
            var sheet = workbook.GetSheetAt(0);

            var rowNumber = beginRowNumber;
            var row = sheet.CreateRow(rowNumber);
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var col = table.Columns[i];
                row.CreateCell(i + beginColNumber).SetCellValue(col.ColumnName);
            }
            rowNumber++;
            for (var i = 0; i < table.Rows.Count; i++)
            {
                row = sheet.CreateRow(rowNumber);
                var rowData = table.Rows[i];
                for (var j = 0; j < table.Columns.Count; j++)
                {
                    var value = rowData[j] == null || rowData[j] == DBNull.Value ? "" : rowData[j].ToString();
                    row.CreateCell(j + beginColNumber).SetCellValue(value);
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