<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="AuroraDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.AuroraDbPostgreSql</strong></a></span>
    <br/>
    <span style="font-size:16px;">The Amazon Aurora Amazon Aurora PostgreSQL schema reader and composer of RepoDB library.</span>
</div>

-----

- `AuroraDbPostgreSqlSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, indexes, foreign keys, unique and check constraints) from the `pg_catalog` using the connection supplied to each operation. PostgreSQL 12 or later is expected, and the names of the tables are case-sensitive.
- `AuroraDbPostgreSqlSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects.
- `GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema()` (`AuroraDbPostgreSqlSchemaGlobalConfiguration`) registers the reader and composer for `AuroraDbConnection`.
