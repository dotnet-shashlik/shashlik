using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using MySqlConnector;

namespace Shashlik.DataProtector.MySql
{
    /// <summary>
    /// An XML repository backed by a Redis list entry.
    /// </summary>
    public class MySqlXmlRepository : IXmlRepository
    {
        private MySqlDataProtectorOptions Option { get; }
        private string ConnectionString { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public MySqlXmlRepository(MySqlDataProtectorOptions option)
        {
            Option = option;
            ConnectionString = option.ConnectionString;
            InitDb();
        }

        /// <inheritdoc />
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
            cmd.CommandText = $"SELECT * FROM {Option.TableName};";
            var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            foreach (DataRow row in table.Rows)
            {
                var xml = row[0].ToString();
                yield return XElement.Parse(xml);
            }
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            using var conn = new MySqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using var cmd = conn.CreateCommand();
            var sql = $@"insert into `{Option.TableName}`(xml,createtime) values(@xml,now());";
            cmd.CommandText = sql;
            cmd.Parameters.Add(new MySqlParameter("@xml", MySqlDbType.String)
                {Value = element.ToString(SaveOptions.DisableFormatting)});
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="connString"></param>
        private void InitDb()
        {
            using var conn = new MySqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using var cmd = conn.CreateCommand();
            // 创建架构和数据表
            var batchSql = $@"
CREATE TABLE IF NOT EXISTS `{Option.TableName}`(
	`id` INT AUTO_INCREMENT NOT NULL,
    `xml` VARCHAR(4000) NOT NULL,
	`createtime` DATETIME NOT NULL,
    PRIMARY KEY (`id`)
);
";
            cmd.CommandText = batchSql;
            cmd.ExecuteNonQuery();
        }
    }
}