using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

namespace IntegrationTests
{
    public static partial class Database
    {
        public static ConnectionTableConstraint HasTable(string tableName)
        {
            return Table("public", tableName);
        }
        public static ConnectionTableConstraint Table(string schema, string tableName)
        {
            return new ConnectionTableConstraint(schema, tableName);
        }
    }

    public sealed class ConnectionColumnConstraint : Constraint
    {
        private readonly ConnectionTableConstraint table;

        public string Schema => table.Schema;
        public string Table => table.Name;
        public string ColumnName { get; }

        public ConnectionColumnConstraint(ConnectionTableConstraint table, string columnName)
        {
            this.table = table;

            ColumnName = columnName;
            this.Description = $"{table.Schema}.{table.Name}.{columnName}";
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is Npgsql.NpgsqlConnection connection)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "select column_name from information_schema.columns where table_schema = @Schema and table_name = @Table";
                cmd.Parameters.Add(new Npgsql.NpgsqlParameter("Schema", Schema));
                cmd.Parameters.Add(new Npgsql.NpgsqlParameter("Table", Table));
                using var reader = cmd.ExecuteReader();
                var columnNames = new List<string>();

                while (reader.Read())
                {
                    var columnName = (string)reader["column_name"];

                    columnNames.Add($"{table.Schema}.{table.Name}.{columnName}");
                }

                return new ConstraintResult(this, columnNames, isSuccess: columnNames.Contains(Description));
            }
            else
            {
                return new ConstraintResult(this, null, isSuccess: false);
            }
        }
    }

    public sealed class ConnectionTableConstraint : Constraint
    {
        public string Name { get; }

        public string Schema { get; }

        public ConnectionTableConstraint(string schema, string name)
            : base(schema, name)
        {
            Schema = schema;
            Name = name;
            Description = "contains " + GetStringRepresentation();
        }

        protected override string GetStringRepresentation()
        {
            return $"{Schema}.{Name}";
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if(actual is Npgsql.NpgsqlConnection connection)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "select schemaname, tablename from pg_catalog.pg_tables where schemaname = @Schema";
                var param = cmd.Parameters.Add(new Npgsql.NpgsqlParameter("Schema", Schema));
                using var reader = cmd.ExecuteReader();
                var tableNames = new List<string>();

                while(reader.Read())
                {
                    var (item, itemSchema) = ((string)reader["tablename"], (string)reader["schemaname"]);

                    tableNames.Add($"{itemSchema}.{item}");
                }

                return new ConstraintResult(this, tableNames, isSuccess: tableNames.Contains(GetStringRepresentation()));
            }
            else
            {
                return new ConstraintResult(this, null, isSuccess: false);
            }
        }

        public ConnectionColumnConstraint Column(string name) => new ConnectionColumnConstraint(this, name);
    }
}
