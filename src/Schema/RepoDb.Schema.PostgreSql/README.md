<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="PostgreSQL.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.PostgreSql</strong></a></span>
    <br/>
    <span style="font-size:16px;">The PostgreSQL schema reader and composer of RepoDB library.</span>
</div>

-----

- `PostgreSqlSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, indexes, foreign keys, unique and check constraints) from the `pg_catalog` of the connection it is created with. PostgreSQL 12 or later is expected, and the names of the tables are case-sensitive.
- `PostgreSqlSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects.
- `GlobalConfiguration.Setup().UsePostgreSqlSchema()` (`PostgreSqlSchemaGlobalConfiguration`) registers the composer for `NpgsqlConnection`.
