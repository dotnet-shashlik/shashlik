﻿using System;
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
        private static bool _dbInit;

        public PostgreSqlXmlRepository(PostgreSqlDataProtectionOptions options)
        {
            Options = options;
            ConnectionString = options.ConnectionString;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            if (!_dbInit)
                InitDb();
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            if (!_dbInit)
                InitDb();
            using var conn = new NpgsqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM \"{Options.Scheme}\".\"{Options.TableName}\";";
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
            if (!_dbInit)
                InitDb();
            using var conn = new NpgsqlConnection(ConnectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            using var cmd = conn.CreateCommand();
            var sql = $"insert into \"{Options.Scheme}\".\"{Options.TableName}\"(\"Xml\",\"CreateTime\") values(@xml,@now);";
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
CREATE SCHEMA IF NOT EXISTS ""{Options.Scheme}"";

CREATE TABLE IF NOT EXISTS ""{Options.TableName}""(
	""Id"" SERIAL PRIMARY KEY,
	""Xml"" VARCHAR(4000) NOT NULL,
	""CreateTime"" TIMESTAMP NOT NULL
);";
            cmd.CommandText = batchSql;
            cmd.ExecuteNonQuery();
            _dbInit = true;
        }
    }
}