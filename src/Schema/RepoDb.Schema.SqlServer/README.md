<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="SQLServer.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.SqlServer</strong></a></span>
    <br/>
    <span style="font-size:16px;">The SQL Server schema reader and composer of RepoDB library.</span>
</div>

-----

- `SqlServerSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, indexes, foreign keys, unique and check constraints) from the `sys.*` catalog views using the connection supplied to each operation.
- `SqlServerSchemaComposer` (`ISchemaComposer`) composes the T-SQL statements that create the equivalent objects.
- `GlobalConfiguration.Setup().UseSqlServerSchema()` (`SqlServerSchemaGlobalConfiguration`) registers the reader and composer for `SqlConnection`.
