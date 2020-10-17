using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using System.IO;
using System.Data;
using Shashlik.Utils.Helpers;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test
{
    public class ExcelHelperTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ExcelHelperTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WriteTo_noTitle_test()
        {
            string fileName = "1111.xlsx";
            var col = RandomHelper.GetRandomCode(6);
            var col1 = RandomHelper.GetRandomCode(6);
            var row = new object[] {Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n")};
            var row1 = new object[] {Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n")};
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                DataTable table = new DataTable("test");
                table.Columns.Add(col);
                table.Columns.Add(col1);
                table.Rows.Add(row);
                table.Rows.Add(row1);
                ExcelHelper.WriteTo(fs, table, false);
                File.Exists(fileName).ShouldBeTrue();
            }

            var ds = ExcelHelper.ToDataSet(File.OpenRead(fileName));
            ds.Tables[0].Rows[0][0].ShouldBe(row[0]);
            ds.Tables[0].Rows[0][1].ShouldBe(row[1]);
            ds.Tables[0].Rows[1][0].ShouldBe(row1[0]);
            ds.Tables[0].Rows[1][1].ShouldBe(row1[1]);
            File.Delete(fileName);
        }

        [Fact]
        public void WriteTo_hasTitle_test()
        {
            string fileName = "2222.xlsx";
            var col = RandomHelper.GetRandomCode(6);
            var col1 = RandomHelper.GetRandomCode(6);
            var row = new object[] {Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n")};
            var row1 = new object[] {Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n")};
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                DataTable table = new DataTable("test");
                table.Columns.Add(col);
                table.Columns.Add(col1);
                table.Rows.Add(row);
                table.Rows.Add(row1);
                ExcelHelper.WriteTo(fs, table, true);
                File.Exists(fileName).ShouldBeTrue();
            }

            var ds = ExcelHelper.ToDataSet(File.OpenRead(fileName));
            ds.Tables[0].Rows[0][0].ShouldBe(col);
            ds.Tables[0].Rows[0][1].ShouldBe(col1);

            ds.Tables[0].Rows[1][0].ShouldBe(row[0]);
            ds.Tables[0].Rows[1][1].ShouldBe(row[1]);
            ds.Tables[0].Rows[2][0].ShouldBe(row1[0]);
            ds.Tables[0].Rows[2][1].ShouldBe(row1[1]);
            File.Delete(fileName);
        }

        [Fact]
        public void WriteToTemplate()
        {
            var col1 = "A2";
            var col2 = "B2";
            var col3 = "C2";
            var fileName = "3333.xlsx";
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                DataTable table = new DataTable("test");
                table.Columns.Add("A");
                table.Columns.Add("B");
                table.Columns.Add("C");
                table.Rows.Add(col1, col2, col3);
                ExcelHelper.WriteTo(File.OpenRead("HelperTests/test_template.xlsx"), fs, table, 2);
            }

            var ds = ExcelHelper.ToDataSet(File.OpenRead(fileName));

            ds.Tables[0].Rows[0][0].ShouldBe("A");
            ds.Tables[0].Rows[0][1].ShouldBe("B");
            ds.Tables[0].Rows[0][2].ShouldBe("C");

            ds.Tables[0].Rows[2][0].ShouldBe(col1);
            ds.Tables[0].Rows[2][1].ShouldBe(col2);
            ds.Tables[0].Rows[2][2].ShouldBe(col3);
            File.Delete(fileName);
        }

        [Fact]
        public void ReadWrongStream()
        {
            using var ms = new MemoryStream();
            var ds = ExcelHelper.ToDataSet(ms);
            ds.ShouldBeNull();
        }


        [Fact]
        public void ReadSuccessStream()
        {
            using var ms = new FileStream("./HelperTests/test_template.xls", FileMode.Open);
            var ds = ExcelHelper.ToDataSet(ms);

            foreach (DataTable table in ds.Tables)
            {
                table.AsEnumerable()
                    .ForEachItem(row =>
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (DataColumn col in table.Columns)
                        {
                            sb.Append(row[col]?.ToString());
                            sb.Append("\t|");
                        }

                        _testOutputHelper.WriteLine(sb.ToString());
                    });
            }
        }

        [Fact]
        public void ReadSuccessStream2()
        {
            using var ms = new FileStream("./HelperTests/test_template.xls", FileMode.Open);
            var ds = ExcelHelper.ToDataSetWithNPOI(ms);

            foreach (DataTable table in ds.Tables)
            {
                table.AsEnumerable()
                    .ForEachItem(row =>
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (DataColumn col in table.Columns)
                        {
                            sb.Append(row[col]?.ToString());
                            sb.Append("\t|");
                        }

                        _testOutputHelper.WriteLine(sb.ToString());
                    });
            }
        }
    }
}