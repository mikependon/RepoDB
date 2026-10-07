<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="SQLite.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.Turso</strong></a></span>
    <br/>
    <span style="font-size:16px;">The Turso schema reader and composer of RepoDB library.</span>
</div>

-----

- `TursoSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, indexes, foreign keys, unique and check constraints) from `sqlite_master` and the `pragma` functions of the connection it is created with (the names of the constraints, the check constraints, the expressions of the generated columns and the filters of the indexes are read from the SQL text that created them). A schema is an attached database: the tables of the main database have no schema.
- Turso keeps the names of its tables, indexes and foreign keys in lower case, so the reader takes the names that were written in the `CREATE` statements, and the size, precision and scale of the types (that the `pragma` drops) from them. ATTACH and the generated columns are experimental features of Turso that can not be switched on from the connection, so there is no non-default schema and no computed column, and the `pragma` functions are called without a schema argument. The internal tables of the engine (`__turso_internal_*`) are not part of the schema. A table whose name contains a double quote can not be read, as the engine fails on it.
- `TursoSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. SQLite can not add a foreign key to an existing table, so the foreign keys are created together with the table (`ComposeAddForeignKey` composes an empty statement, that is not executed).
- `GlobalConfiguration.Setup().UseTursoSchema()` (`TursoSchemaGlobalConfiguration`) registers the composer for the `SqliteConnection` of the driver.
