using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace IntegrationTests;

public static class Database
{
    public static ConnectionTableConstraint HasTable(string tableName)
    {
        return Table("public", tableName);
    }

    private static ConnectionTableConstraint Table(string schema, string tableName)
    {
        return new ConnectionTableConstraint(schema, tableName);
    }
}

public sealed class ConnectionColumnConstraint(ConnectionTableConstraint table, string columnName) : Constraint
{
    public override string Description => $"{table.Schema}.{table.Name}.{columnName}";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is Npgsql.NpgsqlConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select column_name from information_schema.columns where table_schema = @Schema and table_name = @Table";
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("Schema", table.Schema));
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("Table", table.Name));
            using var reader = cmd.ExecuteReader();
            var columnNames = new List<string>();

            while (reader.Read())
            {
                var column = (string)reader["column_name"];

                columnNames.Add($"{table.Schema}.{table.Name}.{column}");
            }

            return new ConstraintResult(this, columnNames, isSuccess: columnNames.Contains(Description));
        }
        else
        {
            return new ConstraintResult(this, null, isSuccess: false);
        }
    }
}

public sealed class ConnectionTableConstraint(string schema, string name) : Constraint(schema, name)
{
    public string Name => name;

    public string Schema => schema;

    public override string Description { get; } = "contains " + MakeStringRepresentation(schema, name);

    static string MakeStringRepresentation(string schema, string name) => $"{schema}.{name}";

    protected override string GetStringRepresentation() => MakeStringRepresentation(schema, name);

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if(actual is Npgsql.NpgsqlConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select schemaname, tablename from pg_catalog.pg_tables where schemaname = @Schema";
            _ = cmd.Parameters.Add(new Npgsql.NpgsqlParameter("Schema", Schema));
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

    public ConnectionColumnConstraint Column(string columnName) => new ConnectionColumnConstraint(this, columnName);
}
