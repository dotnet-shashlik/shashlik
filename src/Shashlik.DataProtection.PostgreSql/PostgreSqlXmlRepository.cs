using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Npgsql;
// ReSharper disable CheckNamespace

namespace Shashlik.DataProtection
{
    public class PostgreSqlXmlRepository : IXmlRepository
    {
        private PostgreSqlDataProtectionOptions Options { get; }
        private string ConnectionString { get; }

        public PostgreSqlXmlRepository(PostgreSqlDataProtectionOptions options)
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
            using var conn = new NpgsqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM \"{Options.TableName}\";";
            var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            foreach (DataRow row in table.Rows)
            {
                var xml = row["Xml"].ToString();
                yield return XElement.Parse(xml);
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using var cmd = conn.CreateCommand();
            var sql = $"insert into \"{Options.TableName}\"(\"Xml\",\"CreateTime\") values(@xml,@now);";
            cmd.CommandText = sql;
            cmd.Parameters.Add(new NpgsqlParameter("@xml", DbType.String)
                {Value = element.ToString(SaveOptions.DisableFormatting)});
            cmd.Parameters.Add(new NpgsqlParameter("@now", DbType.DateTime)
                {Value = DateTime.Now});
            cmd.ExecuteNonQuery();
        }

        private void InitDb()
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using var cmd = conn.CreateCommand();
            var batchSql = $@"
CREATE TABLE IF NOT EXISTS ""{Options.TableName}""(
	""Id"" SERIAL PRIMARY KEY,
	""Xml"" VARCHAR(4000) NOT NULL,
	""CreateTime"" TIMESTAMP NOT NULL
);";
            cmd.CommandText = batchSql;
            cmd.ExecuteNonQuery();
        }
    }
}