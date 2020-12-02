using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using MySqlConnector;

// ReSharper disable CheckNamespace

namespace Shashlik.DataProtection
{
    public class MySqlXmlRepository : IXmlRepository
    {
        private MySqlDataProtectionOptions Options { get; }
        private string ConnectionString { get; }

        public MySqlXmlRepository(MySqlDataProtectionOptions options)
        {
            Options = options;
            ConnectionString = options.ConnectionString;
            InitDb();
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            using var conn = new MySqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM `{Options.TableName}`;";
            var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            foreach (DataRow row in table.Rows)
            {
                var xml = row["xml"].ToString();
                yield return XElement.Parse(xml);
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using var conn = new MySqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using var cmd = conn.CreateCommand();
            var sql = $@"insert into `{Options.TableName}`(`xml`,`createtime`) values(@xml,now());";
            cmd.CommandText = sql;
            cmd.Parameters.Add(new MySqlParameter("@xml", MySqlDbType.String)
                {Value = element.ToString(SaveOptions.DisableFormatting)});
            cmd.ExecuteNonQuery();
        }

        private void InitDb()
        {
            if (Options.EnableAutoCreateDataBase)
                CreateDataBaseIfNoExists();
            using var conn = new MySqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using var cmd = conn.CreateCommand();
            // 创建架构和数据表
            var batchSql = $@"
CREATE TABLE IF NOT EXISTS `{Options.TableName}`(
	`id` INT AUTO_INCREMENT NOT NULL,
    `xml` VARCHAR(4000) NOT NULL,
	`createtime` DATETIME NOT NULL,
    PRIMARY KEY (`id`)
);
";
            cmd.CommandText = batchSql;
            cmd.ExecuteNonQuery();
        }

        private void CreateDataBaseIfNoExists()
        {
            var builder = new MySqlConnectionStringBuilder(ConnectionString);
            var database = builder.Database;
            // ReSharper disable once AssignNullToNotNullAttribute
            builder.Database = null;
            var newConnStr = builder.ToString();
            using var conn = new MySqlConnection(newConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{database}` DEFAULT CHARACTER  SET utf8mb4 COLLATE =utf8mb4_general_ci;";
            cmd.ExecuteNonQuery();
        }
    }
}