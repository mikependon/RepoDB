<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="SQLite.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Schema.Ahtola</strong></a></span>
    <br/>
    <span style="font-size:16px;">The Ahtola schema reader and composer of RepoDB library.</span>
</div>

-----

- `AhtolaSchemaReader` (`ISchemaReader`) reads the schema of a table (columns, primary key, indexes, foreign keys, unique and check constraints) from `sqlite_master` and the `pragma` functions of the connection it is created with (the names of the constraints, the check constraints, the expressions of the generated columns and the filters of the indexes are read from the SQL text that created them). A schema is an attached database: the tables of the main database have no schema.
- The Ahtola engine ignores a parameter that is the schema argument of the `pragma` functions, so the name of the schema is written into those statements. `ATTACH` requires a file-backed primary database, and the internal tables of the engine (`__turso_internal_*`) are not part of the schema.
- `AhtolaSchemaComposer` (`ISchemaComposer`) composes the SQL statements that create the equivalent objects. SQLite can not add a foreign key to an existing table, so the foreign keys are created together with the table (`ComposeAddForeignKey` composes an empty statement, that is not executed).
- `GlobalConfiguration.Setup().UseAhtolaSchema()` (`AhtolaSchemaGlobalConfiguration`) registers the composer for the `SqliteConnection` of the driver.
