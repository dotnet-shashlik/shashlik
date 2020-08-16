using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Common;
using System.IO;
using System.Data;

namespace Shashlik.Utils.Test
{
    public class ExcelHelperTests
    {
        [Fact]
        public void WriteTo_noTitle_test()
        {
            string fileName = "1111.xlsx";
            var col = RandomHelper.GetRandomCode(6);
            var col1 = RandomHelper.GetRandomCode(6);
            var row = new object[] { Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n") };
            var row1 = new object[] { Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n") };
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
            var row = new object[] { Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n") };
            var row1 = new object[] { Guid.NewGuid().ToString("n"), Guid.NewGuid().ToString("n") };
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
    }
}
